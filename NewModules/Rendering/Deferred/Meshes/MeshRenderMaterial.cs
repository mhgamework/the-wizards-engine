using MHGameWork.TheWizards.DirectX11.Graphics;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    public class MeshRenderMaterial
    {
        public MeshCoreData.Material Material;

        public ShaderResourceView DiffuseTexture;
        public BasicShader Shader;
        public Buffer PerObjectConstantBuffer;
        public MeshRenderPart[] Parts;
    }
}