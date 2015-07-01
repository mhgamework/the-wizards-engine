using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Shaders;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Rendering.Deferred.Meshes
{
    /// <summary>
    /// Wraps the surfacematerial shader for dualcontouring
    /// </summary>
    public class SurfaceMaterial : IDisposable
    {
        private readonly DX11Game game;
        private BasicShader baseShader;
        /*private ShaderResourceView diffuseTexture;
        private ShaderResourceView normalTexture;
        private ShaderResourceView specularTexture;*/
        private ShaderResourceView[] textures = new ShaderResourceView[3];
        private SamplerState sampler;
        private InputLayout inputLayout;


        public SurfaceMaterial(DX11Game game, ShaderResourceView diffuseTexture)
        {
            this.game = game;


            textures[0] = diffuseTexture;
            textures[1] = null;//normalTexture;
            textures[2] = null;//specularTexture;


            baseShader = BasicShader.LoadAutoreload(game, ShaderFiles.DCSurface, delegate { }, createShaderMacros());
            baseShader.SetTechnique("DCSurface");
            //baseShader.DiffuseTexture = checkerTexture;

            var desc = new SamplerDescription()
                {
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap,
                    Filter = Filter.Anisotropic,
                    MaximumAnisotropy = 16
                };

            sampler = SamplerState.FromDescription(game.Device, desc);

            inputLayout = CreateInputLayout();


        }

        private ShaderMacro[] createShaderMacros()
        {
            var list = new List<ShaderMacro>();
            list.Add(new ShaderMacro("DIFFUSE_MAPPING", "1"));
            list.Add(new ShaderMacro("NORMAL_MAPPING", "1"));
            //if (diffuseTexture != null) list.Add(new ShaderMacro("DIFFUSE_MAPPING", "1"));
            //if (normalTexture != null) list.Add(new ShaderMacro("NORMAL_MAPPING", "1"));
            //if (specularTexture != null) list.Add(new ShaderMacro("SPECULAR_MAPPING", "1"));

            return list.ToArray();
        }

        private InputLayout CreateInputLayout()
        {
            return new InputLayout(game.Device, baseShader.GetCurrentPass(0).Description.Signature, DeferredMeshVertex.Elements);
        }

        public void SetCamera(Matrix view, Matrix projection)
        {
            baseShader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(view);
            baseShader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(projection);
        }

        public void SetPerObjectBuffer(DeviceContext ctx, DeferredMaterial.PerObjectConstantBuffer perObject)
        {
            baseShader.Effect.GetConstantBufferByName("perObject").ConstantBuffer = perObject.Buffer;
        }

        public void SetToContext(DeviceContext ctx)
        {
            baseShader.Apply();
            ctx.PixelShader.SetShaderResources(textures, 0, 3);
            ctx.PixelShader.SetSampler(sampler, 0);
            ctx.InputAssembler.InputLayout = inputLayout;

        }


        public void Dispose()
        {
            baseShader.Dispose();
            sampler.Dispose();
            inputLayout.Dispose();
            baseShader = null;
            sampler = null;
            inputLayout = null;
        }
    }
}