using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using SlimDX;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MHGameWork.TheWizards.Rendering.Deferred.Meshes
{
    /// <summary>
    /// Responsible for rendering a set of meshes.
    /// Responsible for delegating commands to the Batcher, instanced renderer and normal renderer
    /// Uses Pools for caching
    /// Note: multiple implementations should be added for this class. 
    /// Current implementation just forwards all calls to the material without changing the meshes
    /// 
    /// Optimizations ==>
    ///  - Render per material
    ///  - batch static objects per material
    ///  - Use texture atlases
    ///  - Use perObjectCB cache for static objects
    /// </summary>
    public class MeshesRenderer
    {
        private RendererResourcePool resourcePool;
        private ICamera currentCamera;
        private DX11Game game;

        private DeferredMaterial.PerObjectConstantBuffer sharedPerObjectCB;
        private DeviceContext ctx;

        public MeshesRenderer(RendererResourcePool resourcePool, DX11Game game)
        {
            this.resourcePool = resourcePool;
            this.game = game;
            this.ctx = game.Device.ImmediateContext;
        }


        /// <summary>
        /// Not Thread safe!
        /// </summary>
        /// <param name="meshes"></param>
        /// <param name="camera"></param>
        public void DrawMeshes(List<WorldMesh> meshes, ICamera camera)
        {
            currentCamera = camera;
            foreach (var mesh in meshes)
                foreach (var part in getMeshParts(mesh))
                    drawMeshPart(part);

            currentCamera = camera;
        }

        private void drawMeshPart(WorldMeshPart part)
        {
            var material = resourcePool.GetDeferredMaterial(part.Material);
            var partData = resourcePool.GetMeshPartRenderData(part.Part);
            var perObjectCB = getPerObjectCB(part);

            perObjectCB.UpdatePerObjectBuffer(ctx, part.WorldMatrix);

            material.SetCamera(currentCamera.View, currentCamera.Projection);
            material.SetPerObjectBuffer(ctx, perObjectCB);

            material.SetToContext(ctx);

            partData.Draw(ctx);




        }

        private DeferredMaterial.PerObjectConstantBuffer getPerObjectCB(WorldMeshPart part)
        {
            return sharedPerObjectCB ?? (sharedPerObjectCB = DeferredMaterial.CreatePerObjectCB(game));
        }

        private IEnumerable<WorldMeshPart> getMeshParts(WorldMesh mesh)
        {
            foreach (var part in mesh.Mesh.GetCoreData().Parts)
            {
                yield return
                    new WorldMeshPart()
                        {
                            Material = part.MeshMaterial,
                            Part = part.MeshPart,
                            WorldMatrix = part.ObjectMatrix.dx() * mesh.WorldMatrix
                        };
            }
        }
    }

    /// <summary>
    /// Responsible for caching GPU resources
    /// Loaders for resources can be added to this class to fill the pool
    /// Currently the loaders are hardcoded
    /// </summary>
    public class RendererResourcePool
    {
        private Dictionary<object, object> pool = new Dictionary<object, object>();
        private MeshRenderDataFactory meshFactory;
        private TexturePool texturePool;
        private DX11Game game;

        public RendererResourcePool(MeshRenderDataFactory meshFactory, TexturePool texturePool, DX11Game game)
        {
            this.meshFactory = meshFactory;
            this.texturePool = texturePool;
            this.game = game;
        }

        public DeferredMaterial GetDeferredMaterial(MeshCoreData.Material material)
        {
            if (pool.ContainsKey(material)) return (DeferredMaterial)pool[material];

            var ret = new DeferredMaterial(game,
                                            material.DiffuseMap == null ? null : texturePool.LoadTexture(material.DiffuseMap),
                                            material.NormalMap == null ? null : texturePool.LoadTexture(material.NormalMap),
                                            material.SpecularMap == null ? null : texturePool.LoadTexture(material.SpecularMap));

            pool.Add(material, ret);

            return ret;


        }

        public MeshPartRenderData GetMeshPartRenderData(IMeshPart part)
        {
            if (pool.ContainsKey(part)) return (MeshPartRenderData)pool[part];

            var ret = meshFactory.CreateMeshPartData(part);

            pool.Add(part, ret);

            return ret;
        }
    }

    /// <summary>
    /// Responsible for holding info about a mesh which is in the world
    /// </summary>
    public class WorldMesh
    {
        public IMesh Mesh;
        public Matrix WorldMatrix;
    }

    public class WorldMeshPart
    {
        public IMeshPart Part;
        public MeshCoreData.Material Material;
        public Matrix WorldMatrix;

    }
}
