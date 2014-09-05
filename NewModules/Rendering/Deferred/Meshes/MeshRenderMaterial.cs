using MHGameWork.TheWizards.DirectX11.Graphics;
using SlimDX.Direct3D11;
using StillDesign.PhysX;
using IDisposable = System.IDisposable;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    public class MeshRenderMaterial : IDisposable
    {
        public MeshCoreData.Material Material;

        public ShaderResourceView DiffuseTexture;
        public BasicShader Shader;
        public Buffer PerObjectConstantBuffer;
        public MeshRenderPart[] Parts;

        public void Dispose()
        {
            Material = null;
            DiffuseTexture = null;
            Shader = null;

            if (PerObjectConstantBuffer != null)
                PerObjectConstantBuffer.Dispose();
            PerObjectConstantBuffer = null;

            if (Parts != null)
                foreach (var p in Parts) p.Dispose();
            Parts = null;
        }
    }
}