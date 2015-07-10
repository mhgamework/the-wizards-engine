using System;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using MHGameWork.TheWizards.Rendering;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.Meshes
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

            if (Shader != null)
                Shader.Dispose();
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