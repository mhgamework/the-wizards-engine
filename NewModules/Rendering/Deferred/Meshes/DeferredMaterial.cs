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
    /// Responsible for preparing a direct3d context for rendering deferred objects
    /// Responsible for managing the d3d resources required for this material
    /// 
    /// Note: Requires InputLayout: POSITION - NORMAL - TEXCOORD - TANGENT
    /// 
    /// TODO: per material buffer    
    /// </summary>
    public class DeferredMaterial
    {
        private readonly DX11Game game;
        private BasicShader baseShader;
        private ShaderResourceView diffuseTexture;
        private ShaderResourceView normalTexture;
        private ShaderResourceView specularTexture;
        private ShaderResourceView[] textures = new ShaderResourceView[3];
        private SamplerState sampler;
        private InputLayout inputLayout;

    
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
            if (diffuseTexture != null) list.Add(new ShaderMacro("DIFFUSE_MAPPING", "1"));
            if (normalTexture != null) list.Add(new ShaderMacro("NORMAL_MAPPING", "1"));
            if (specularTexture != null) list.Add(new ShaderMacro("SPECULAR_MAPPING", "1"));

            return list.ToArray();
        }

        private InputLayout CreateInputLayout()
        {
            return new InputLayout(game.Device, baseShader.GetCurrentPass(0).Description.Signature, DeferredMeshVertex.Elements);
        }

        public void SetCamera( Matrix view, Matrix projection )
        {
            baseShader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(view);
            baseShader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(projection);
        }

        public void SetPerObjectBuffer(DeviceContext ctx, PerObjectConstantBuffer perObject)
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

      

        public static PerObjectConstantBuffer CreatePerObjectCB( DX11Game dx11Game )
        {
            return new PerObjectConstantBuffer(dx11Game);
        }

        /// <summary>
        /// Responsible for updating and creating the per object constant buffer for this material
        /// TODO: convert this into a generic constant buffer class
        /// </summary>
        public class PerObjectConstantBuffer
        {
            private Buffer perObjectBuffer;
            private DataStream perObjectStrm;

            public PerObjectConstantBuffer(DX11Game game)
            {
                perObjectBuffer = new Buffer(game.Device, new BufferDescription
                {
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    SizeInBytes = 16 * 4, // PerObjectCB
                    Usage = ResourceUsage.Dynamic,
                    StructureByteStride = 0
                });

                //perObjectStrm = new DataStream(baseShader.Effect.GetConstantBufferByName("perObject").ConstantBuffer.Description.SizeInBytes, false, true);
                perObjectStrm = new DataStream(Marshal.SizeOf(typeof(Data)), false, true);
            }

            public Buffer Buffer
            {
                get { return perObjectBuffer; }
            }

            public void UpdatePerObjectBuffer(DeviceContext context, Matrix world)
            {
                var box = context.MapSubresource(Buffer, MapMode.WriteDiscard,
                                                 MapFlags.None);
                box.Data.Write(new Data
                {
                    WorldMatrix = Matrix.Transpose(world)
                });

                context.UnmapSubresource(Buffer, 0);
            }

            private struct Data
            {
                public Matrix WorldMatrix;
            }

            public void Dispose()
            {
                perObjectBuffer.Dispose();
                perObjectStrm.Dispose();
                perObjectBuffer = null;
                perObjectStrm = null;
            }
        }


        public void Dispose()
        {
            baseShader.Dispose();
            baseShader = null;
            inputLayout.Dispose();
            inputLayout = null;
            sampler.Dispose();
            sampler = null;
        }
    }
}