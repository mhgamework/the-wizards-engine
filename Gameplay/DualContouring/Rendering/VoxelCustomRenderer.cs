using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering.Deferred.Meshes;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring.Rendering
{
    /// <summary>
    /// GBuffer renderer for surfaces outputted by dualcontouring
    /// Intended features: 
    ///  - multi material rendering
    ///  - triplanar texture mapping
    ///  - Lod transition blending
    /// </summary>
    public class VoxelCustomRenderer : ICustomGBufferRenderer
    {
        private readonly DX11Game game;
        private readonly DualContouringMeshBuilder dcMeshBuilder;
        private readonly DualContouringAlgorithm dcAlgo;
        private readonly MeshRenderDataFactory renderDataFactory;

        private List<VoxelSurface> surfaces = new List<VoxelSurface>();
        private SurfaceMaterial mat;
        private DeferredMaterial.PerObjectConstantBuffer objectBuffer;

        public VoxelCustomRenderer(DX11Game game, DeferredRenderer dRenderer, DualContouringMeshBuilder dcMeshBuilder, DualContouringAlgorithm dcAlgo, MeshRenderDataFactory renderDataFactory)
        {
            this.game = game;
            this.dcMeshBuilder = dcMeshBuilder;
            this.dcAlgo = dcAlgo;
            this.renderDataFactory = renderDataFactory;


            var tex = new RAMTexture();

            var data = tex.GetCoreData();
            data.StorageType = TextureCoreData.TextureStorageType.Disk;
            data.DiskFilePath = TWDir.GameData.CreateSubdirectory(@"Rendering\UVChecker-map\UVCheckerMaps").GetFile("UVCheckerMap10-512.png").FullName;
            /*data.StorageType = TextureCoreData.TextureStorageType.Assembly;
            data.Assembly = Assembly.GetExecutingAssembly();
            data.AssemblyResourceName = "MHGameWork.TheWizards.Tests.OBJParser.Files.maps.BrickRound0030_7_S.jpg";*/

            mat = new SurfaceMaterial(game, dRenderer.TexturePool.LoadTexture(tex));
            objectBuffer = DeferredMaterial.CreatePerObjectCB(game);
        }


        public void Draw()
        {
            mat.SetCamera(game.Camera.View, game.Camera.Projection);
            mat.SetToContext(game.Device.ImmediateContext);
            mat.SetPerObjectBuffer(game.Device.ImmediateContext, objectBuffer);

            foreach (var surface in surfaces)
            {
                if ( surface.RenderData == null ) return;
                objectBuffer.UpdatePerObjectBuffer(game.Device.ImmediateContext, surface.WorldMatrix);
                surface.RenderData.Draw(game.Device.ImmediateContext);
            }
        }


        public void Dispose()
        {
            mat.Dispose();
            objectBuffer.Dispose();
        }

        public VoxelSurface CreateSurface(AbstractHermiteGrid grid, Matrix world)
        {

            var rawData = dcMeshBuilder.buildRawMesh(grid);
            if (rawData.Positions.Length == 0)
            {
                return new VoxelSurface(this, null) { WorldMatrix = world };
            }
            var ret = new VoxelSurface(this, renderDataFactory.CreateMeshPartData(rawData));
            ret.Mesh = rawData;
            ret.WorldMatrix = world;
            surfaces.Add(ret);
            return ret;
        }

        public void deleteInternal(VoxelSurface voxelSurface)
        {
            if (!voxelSurface.IsDestroyed) throw new InvalidOperationException();
            surfaces.Remove(voxelSurface);
        }

        public static VoxelCustomRenderer CreateDefault(GraphicsWrapper game)
        {
            return new VoxelCustomRenderer(game,
                                            game.AcquireRenderer(),
                                            new DualContouringMeshBuilder(),
                                            new DualContouringAlgorithm(),
                                            new MeshRenderDataFactory(game, null,
                                                                       game.AcquireRenderer().TexturePool));
        }
    }

    public class VoxelSurface
    {
        public VoxelSurface(VoxelCustomRenderer customRenderer, MeshPartRenderData renderData)
        {
            _customRenderer = customRenderer;
            _renderData = renderData;
        }

        public Matrix WorldMatrix;
        private VoxelCustomRenderer _customRenderer;
        private MeshPartRenderData _renderData;
        public RawMeshData Mesh;

        public MeshPartRenderData RenderData
        {
            get { return _renderData; }
        }

        public bool IsDestroyed { get { return _customRenderer == null; } }

        public void Delete()
        {
            if (IsDestroyed) return;
            var r = _customRenderer;
            if (_renderData != null)
                _renderData.Dispose();
            _renderData = null;
            _customRenderer = null;
            r.deleteInternal(this);

        }
    }

}