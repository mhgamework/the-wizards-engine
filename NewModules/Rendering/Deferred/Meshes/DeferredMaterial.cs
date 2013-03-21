using System.Collections.Generic;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Shaders;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Rendering.Deferred.Meshes
{
    /// <summary>
    /// Responsible for preparing a direct3d context for rendering deferred objects
    /// Responsible for managing the d3d resources required for this material
    /// 
    /// Note: Requires InputLayout: POSITION - NORMAL - TEXCOORD - TANGENT
    /// 
    /// TODO: per material buffer
    /// 
    /// </summary>
    public class DeferredMaterial
    {
        private readonly DX11Game game;
        private readonly DeviceContext context;
        private BasicShader baseShader;
        private ShaderResourceView diffuseTexture;
        private ShaderResourceView normalTexture;
        private ShaderResourceView specularTexture;
        private ShaderResourceView[] textures = new ShaderResourceView[3];
        private SamplerState sampler;

        public DeferredMaterial(DX11Game game)
        {
            this.game = game;
            context = game.Device.ImmediateContext;
        }

        public DeferredMaterial(DX11Game game, ShaderResourceView diffuseTexture, ShaderResourceView normalTexture, ShaderResourceView specularTexture)
        {
            this.game = game;
            this.diffuseTexture = diffuseTexture;
            this.normalTexture = normalTexture;
            this.specularTexture = specularTexture;


            textures[0] = diffuseTexture;
            textures[1] = normalTexture;
            textures[2] = specularTexture;


            baseShader = BasicShader.LoadAutoreload(game, ShaderFiles.DeferredMesh, delegate { }, createShaderMacros());
            baseShader.SetTechnique("Technique1");
            //baseShader.DiffuseTexture = checkerTexture;

            var desc = new SamplerDescription()
                {
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    Filter = Filter.Anisotropic,
                    MaximumAnisotropy = 16
                };

            sampler = SamplerState.FromDescription(game.Device, desc);


        }

        private ShaderMacro[] createShaderMacros()
        {
            var list = new List<ShaderMacro>();
            if (diffuseTexture != null) list.Add(new ShaderMacro("DIFFUSE_MAPPING", "1"));
            if (normalTexture != null) list.Add(new ShaderMacro("NORMAL_MAPPING", "1"));
            if (specularTexture != null) list.Add(new ShaderMacro("SPECULAR_MAPPING", "1"));

            return list.ToArray();
        }


        private void setToContext()
        {
            context.PixelShader.SetShaderResources(textures, 0, 3);
            context.PixelShader.SetSampler(sampler, 0);
        }
    }
}