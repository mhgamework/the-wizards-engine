using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using SlimDX;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.Meshes
{
    /// <summary>
    /// Responsible for holding render data for a part of the scene with a single material.
    /// Responsible for rendering this part on the GPU
    /// </summary>
    public class MeshElementPart
    {
        public DeferredMaterial mat;
        public DeferredMaterial.PerObjectConstantBuffer perObject;
        private MeshPartRenderData part;

        public MeshElementPart(DeferredMaterial mat, MeshPartRenderData part)
        {
            this.mat = mat;
            this.part = part;



        }

        /// <summary>
        /// Create GPU buffers necessary for the object
        /// </summary>
        public void CreateBuffers(DX11Game game)
        {
            perObject = DeferredMaterial.CreatePerObjectCB(game);
            perObject.UpdatePerObjectBuffer(game.Device.ImmediateContext, Matrix.Identity);
        }

        public void Render(ICamera cam, DeviceContext ctx)
        {
            mat.SetCamera(cam.View, cam.Projection);

            mat.SetToContext(ctx);
            mat.SetPerObjectBuffer(ctx, perObject);

            part.Draw(ctx);
        }

        public void UpdateWorldMatrix(DeviceContext ctx, Matrix worldMatrix)
        {
            perObject.UpdatePerObjectBuffer(ctx, worldMatrix);

        }
    }
}