using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.Meshes;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering.Deferred.Meshes;
using SlimDX;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

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
        private readonly DeferredRenderer dRenderer;
        private readonly DualContouringMeshBuilder dcMeshBuilder;
        private readonly DualContouringAlgorithm dcAlgo;
        private readonly MeshRenderDataFactory renderDataFactory;

        private List<VoxelSurface> surfaces = new List<VoxelSurface>();
        private DeferredMaterial.PerObjectConstantBuffer objectBuffer;

        public Dictionary<DCVoxelMaterial, MaterialInstance> matToInstances = new Dictionary<DCVoxelMaterial, MaterialInstance>();
        private DCVoxelMaterial defaultMaterial;


        public VoxelCustomRenderer(DX11Game game, DeferredRenderer dRenderer, DualContouringMeshBuilder dcMeshBuilder, DualContouringAlgorithm dcAlgo, MeshRenderDataFactory renderDataFactory)
        {
            this.game = game;
            this.dRenderer = dRenderer;
            this.dcMeshBuilder = dcMeshBuilder;
            this.dcAlgo = dcAlgo;
            this.renderDataFactory = renderDataFactory;


            var tex = new RAMTexture();

            defaultMaterial = new DCVoxelMaterial() { Texture = DCFiles.UVCheckerMap11_512 };

            objectBuffer = DeferredMaterial.CreatePerObjectCB(game);
        }


        public void Draw()
        {
            var materials =
                surfaces.SelectMany(s => s.MeshesWithMaterial)
                    .Select(
                        p =>
                            matToInstances.GetOrCreate(p.material,
                                () => createMaterialInstance(p)))
                    .Distinct();
            foreach (var inst in materials)
            {
                var mat = inst.Shader;

                foreach (var surface in surfaces)
                {
                    foreach (var part in surface.MeshesWithMaterial)
                    {
                        if (!part.material.Equals(inst.Material)) continue;

                        mat.SetCamera(game.Camera.View, game.Camera.Projection);
                        mat.SetToContext(game.Device.ImmediateContext);
                        mat.SetPerObjectBuffer(game.Device.ImmediateContext, objectBuffer);
                        objectBuffer.UpdatePerObjectBuffer(game.Device.ImmediateContext, surface.WorldMatrix);
                        part.renderData.Draw(game.Device.ImmediateContext);
                    }
                }
            }

        }

        private MaterialInstance createMaterialInstance(MeshWithMaterial p)
        {
            return new MaterialInstance(p.material, dRenderer, game);
        }


        public void Dispose()
        {
            //TODO: 
            //mat.Dispose();
            objectBuffer.Dispose();
        }

        public VoxelSurface CreateSurface(AbstractHermiteGrid grid, Matrix world)
        {
            VoxelSurface ret;
            ret = CreateVoxelSurfaceAsync(grid, world);

            AddSurface(ret);
            return ret;

        }

        /// <summary>
        /// Adds given voxelsurface to the renderer, throws if already added
        /// </summary>
        /// <param name="ret"></param>
        public void AddSurface(VoxelSurface ret)
        {
            if (surfaces.Contains(ret)) throw new InvalidOperationException();
            foreach (var m in ret.MeshesWithMaterial)
            {
                if (m.renderData != null) throw new InvalidOperationException();
                if ( m.material == null ) m.material = defaultMaterial;
                m.CreateMeshData(renderDataFactory);

            }
            surfaces.Add(ret);
            ret.AddedToRenderer = true;
        }


        /// <summary>
        /// Constructs a VoxelSurface that is not yet attached to the renderer, add it using AddSurface
        /// Should be thread safe.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="world"></param>
        /// <returns></returns>
        public VoxelSurface CreateVoxelSurfaceAsync(AbstractHermiteGrid grid, Matrix world)
        {
            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var materials = new List<DCVoxelMaterial>();

            var algo = new DualContouringAlgorithm();
            algo.GenerateSurface(vertices, indices, materials, grid);


            var triangleNormals = dcMeshBuilder.generateTriangleNormals(indices, vertices);

            var uniqueMaterials = materials.Distinct().ToArray();

            if (!uniqueMaterials.Any())
                return new VoxelSurface(this) { WorldMatrix = world };


            var ret = new VoxelSurface(this);
            ret.WorldMatrix = world;


            foreach (var imat in uniqueMaterials)
            {
                var mat = imat;


                var mesh = new RawMeshData(
                    indices.Where((i, index) => materials[index / 3] == mat).Select(i => vertices[i].dx()).ToArray(),
                    indices.Select((i, index) => triangleNormals[index / 3].dx()).Where((i, index) => materials[index / 3] == mat).ToArray(),
                    indices.Where((i, index) => materials[index / 3] == mat).Select(i => new Vector2().dx()).ToArray(),
                    indices.Where((i, index) => materials[index / 3] == mat).Select(i => new Vector3().dx()).ToArray()
                    );

                var actualMat = mat;
                if (actualMat == null) actualMat = defaultMaterial;
                ret.MeshesWithMaterial.Add(new MeshWithMaterial(mesh, actualMat));
            }
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


        public class MaterialInstance
        {
            public DCVoxelMaterial Material;
            public SurfaceMaterial Shader;

            public MaterialInstance(DCVoxelMaterial material, DeferredRenderer dRenderer, DX11Game gamez)
            {
                Material = material;
                Shader = new SurfaceMaterial(gamez, dRenderer.TexturePool.LoadTexture(material.Texture));
            }
        }

        public class TransformedMeshData
        {
            public Matrix World { get { return Surface.WorldMatrix; } }
            public MeshPartRenderData RenderData;
            public VoxelSurface Surface;

        }
    }

    public class VoxelSurface
    {
        public VoxelSurface(VoxelCustomRenderer customRenderer)
        {
            _customRenderer = customRenderer;
        }

        public Matrix WorldMatrix = Matrix.Identity;
        private VoxelCustomRenderer _customRenderer;
        public IEnumerable<RawMeshData> Meshes { get { return MeshesWithMaterial.Select(p => p.meshData); } }
        public List<MeshWithMaterial> MeshesWithMaterial = new List<MeshWithMaterial>();

        //public RawMeshData Mesh;


        public bool IsDestroyed { get { return _customRenderer == null; } }
        /// <summary>
        /// Internal set only
        /// </summary>
        public bool AddedToRenderer { get; set; }

        public void Delete()
        {
            if (IsDestroyed) return;
            MeshesWithMaterial = null;
            var r = _customRenderer;
            _customRenderer = null;
            r.deleteInternal(this);

        }

    }

    public class MeshWithMaterial
    {
        public RawMeshData meshData;
        public DCVoxelMaterial material;
        public MeshPartRenderData renderData;

        public MeshWithMaterial(RawMeshData meshData, DCVoxelMaterial material)
        {
            this.meshData = meshData;
            this.material = material;
        }

        public void CreateMeshData(MeshRenderDataFactory renderDataFactory)
        {
            renderData = renderDataFactory.CreateMeshPartData(meshData);
        }

    }
}