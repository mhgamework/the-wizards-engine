using System;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;

namespace MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Rendering.Deferred
{
    public class GBuffer : IDisposable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private readonly Device device;
        private Texture2D diffuse;
        private Texture2D normal;
        private Texture2D depth;
        private RenderTargetView diffuseRTV;
        private RenderTargetView normalRTV;
        public DepthStencilView DepthStencilView { get; private set; }
        private DeviceContext context;
        private Viewport viewport;


        public GBuffer(Device device, int width, int height)
        {
            Width = width;
            Height = height;
            this.device = device;
            context = device.ImmediateContext;
            var desc = new Texture2DDescription
                                           {
                                               BindFlags =
                                                   BindFlags.RenderTarget | BindFlags.ShaderResource,
                                               Format = Format.R8G8B8A8_UNorm,
                                               Width = width,
                                               Height = height,
                                               ArraySize = 1,
                                               SampleDescription = new SampleDescription(1, 0),
                                               MipLevels = 1
                                           };
            var depthDesc = new Texture2DDescription
            {
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                Format = Format.R32_Typeless,
                Width = width,
                Height = height,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1
            };

            diffuse = new Texture2D(device, desc);
            normal = new Texture2D(device, desc);
            depth = new Texture2D(device, depthDesc);




            diffuseRTV = new RenderTargetView(device, diffuse);
            normalRTV = new RenderTargetView(device, normal);

            DepthStencilView = new DepthStencilView(device, depth, new DepthStencilViewDescription
                                                                       {
                                                                           Format = Format.D32_Float,
                                                                           Flags = DepthStencilViewFlags.None,
                                                                           Dimension = DepthStencilViewDimension.Texture2D
                                                                       });

            DiffuseRV = new ShaderResourceView(device, diffuse);
            NormalRV = new ShaderResourceView(device, normal);
            DepthRV = new ShaderResourceView(device, depth, new ShaderResourceViewDescription
                                                                {
                                                                    ArraySize = 1,
                                                                    Dimension = ShaderResourceViewDimension.Texture2D,
                                                                    Format = Format.R32_Float,
                                                                    MipLevels = -1
                                                                });

            viewport = new Viewport(0, 0, width, height, 0, 1);



        }

        public ShaderResourceView DiffuseRV { get; set; }
        public ShaderResourceView NormalRV { get; set; }
        public ShaderResourceView DepthRV { get; set; }

        public void SetTargetsToOutputMerger()
        {
            context.OutputMerger.SetTargets(DepthStencilView, diffuseRTV, normalRTV);
            context.Rasterizer.SetViewports(viewport);

        }
        public void Clear()
        {
            context.ClearRenderTargetView(diffuseRTV, new Color4(0, 0, 0, 0));
            context.ClearRenderTargetView(normalRTV, new Color4(0, 0.5f, 0.5f, 0.5f));
            context.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1, 0);
        }
        /// <summary>
        /// The shader should include GBuffer.fx
        /// </summary>
        /// <param name="shader"></param>
        public void SetToShader(BasicShader shader)
        {
            shader.Effect.GetVariableByName("GBuffer_Color").AsResource().SetResource(DiffuseRV);
            shader.Effect.GetVariableByName("GBuffer_Normal").AsResource().SetResource(NormalRV);
            shader.Effect.GetVariableByName("GBuffer_Depth").AsResource().SetResource(DepthRV);

        }
        /// <summary>
        /// The shader should include GBuffer.fx
        /// </summary>
        public void UnsetFromShader(BasicShader shader)
        {
            shader.Effect.GetVariableByName("GBuffer_Color").AsResource().SetResource(null);
            shader.Effect.GetVariableByName("GBuffer_Normal").AsResource().SetResource(null);
            shader.Effect.GetVariableByName("GBuffer_Depth").AsResource().SetResource(null);

        }


        public void Dispose()
        {
            diffuse.Dispose();
            normal.Dispose();
            depth.Dispose();
            diffuseRTV.Dispose();
            normalRTV.Dispose();
            DepthStencilView.Dispose();
            DiffuseRV.Dispose();
            NormalRV.Dispose();
            DepthRV.Dispose();
        }
    }
}
