﻿using System;
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
            foreach (var inst in matToInstances.Values)
            {
                var mat = inst.Shader;
                mat.SetCamera(game.Camera.View, game.Camera.Projection);
                mat.SetToContext(game.Device.ImmediateContext);
                mat.SetPerObjectBuffer(game.Device.ImmediateContext, objectBuffer);
                foreach (var surface in inst.Parts)
                {
                    objectBuffer.UpdatePerObjectBuffer(game.Device.ImmediateContext, surface.World);
                    surface.RenderData.Draw(game.Device.ImmediateContext);
                }
            }

        }


        public void Dispose()
        {
            //TODO: 
            //mat.Dispose();
            objectBuffer.Dispose();
        }

        public VoxelSurface CreateSurface(AbstractHermiteGrid grid, Matrix world)
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
            surfaces.Add(ret);

            var meshes = new List<RawMeshData>();

            foreach (var imat in uniqueMaterials)
            {
                var mat = imat;
                var actualMat = mat;
                if (actualMat == null) actualMat = defaultMaterial;
                var matInstance = matToInstances.GetOrCreate(actualMat, () => new MaterialInstance(actualMat, dRenderer, game));

                var mesh = new RawMeshData(
                    indices.Where((i, index) => materials[index / 3] == mat).Select(i => vertices[i].dx()).ToArray(),
                    indices.Select((i, index) => triangleNormals[index / 3].dx()).Where((i, index) => materials[index / 3] == mat).ToArray(),
                    indices.Where((i, index) => materials[index / 3] == mat).Select(i => new Vector2().dx()).ToArray(),
                    indices.Where((i, index) => materials[index / 3] == mat).Select(i => new Vector3().dx()).ToArray()
                    );
                meshes.Add( mesh );
                matInstance.Parts.Add(new TransformedMeshData() { RenderData = renderDataFactory.CreateMeshPartData(mesh), Surface = ret });
            }
            ret.Meshes = meshes.ToArray();

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
            public List<TransformedMeshData> Parts = new List<TransformedMeshData>();
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

        public Matrix WorldMatrix;
        private VoxelCustomRenderer _customRenderer;
        public RawMeshData[] Meshes = new RawMeshData[0];
        //public RawMeshData Mesh;


        public bool IsDestroyed { get { return _customRenderer == null; } }

        public void Delete()
        {
            if (IsDestroyed) return;
            var r = _customRenderer;
            _customRenderer = null;
            r.deleteInternal(this);

        }
    }

}