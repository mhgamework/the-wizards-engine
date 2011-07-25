using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11.Graphics;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace DirectX11.Rendering.Deferred
{
    /// <summary>
    /// This draws light accumulation from a directional light (full screen)
    /// the rgb components contain diffuse, alpha contains specular
    /// </summary>
    public class CombineFinalRenderer
    {
        private readonly DX11Game game;
        private readonly GBuffer gBuffer;
        private DeviceContext context;
        private BasicShader shader;


        private FullScreenQuad quad;
        private InputLayout layout;
        private Texture2D lightAccumulationMap;
        private BlendState blendState;
        private RenderTargetView lightAccumulationRTV;

        public ShaderResourceView LightAccumulationRV { get; private set; }

        public CombineFinalRenderer(DX11Game game, GBuffer gBuffer)
        {
            this.game = game;
            this.gBuffer = gBuffer;
            var device = game.Device;
            context = device.ImmediateContext;

            shader = BasicShader.LoadAutoreload(game,
                                                new System.IO.FileInfo(
                                                    "..\\..\\DirectX11\\Shaders\\Deferred\\CombineFinal.fx"));

            shader.SetTechnique("Technique0");

            quad = new FullScreenQuad(device);

            layout = FullScreenQuad.CreateInputLayout(device, shader.GetCurrentPass(0));

            var desc = new Texture2DDescription
                                         {
                                             BindFlags =
                                                 BindFlags.RenderTarget | BindFlags.ShaderResource,
                                             Format = Format.R16G16B16A16_Float,
                                             Width = gBuffer.Width,
                                             Height = gBuffer.Height,
                                             ArraySize = 1,
                                             SampleDescription = new SampleDescription(1, 0),
                                             MipLevels = 1
                                         };
            lightAccumulationMap = new Texture2D(device, desc);

            lightAccumulationRTV = new RenderTargetView(device, lightAccumulationMap);
            LightAccumulationRV = new ShaderResourceView(device, lightAccumulationMap);

            var bsDesc = new BlendStateDescription();
            var b = new RenderTargetBlendDescription();
            b.BlendEnable = true;
            b.BlendOperation = BlendOperation.Add;
            b.BlendOperationAlpha = BlendOperation.Add;
            b.DestinationBlend = BlendOption.One;
            b.DestinationBlendAlpha = BlendOption.One;
            b.SourceBlend = BlendOption.One;
            b.SourceBlendAlpha = BlendOption.One;
            b.RenderTargetWriteMask = ColorWriteMaskFlags.All;
            bsDesc.RenderTargets[0] = b;


            blendState = BlendState.FromDescription(device, bsDesc);
        }


        public void ClearLightAccumulation()
        {
            context.ClearRenderTargetView(lightAccumulationRTV, new Color4());
        }
        public void SetLightAccumulationStates()
        {
            var viewport = new Viewport(0, 0, gBuffer.Width, gBuffer.Height);

            context.OutputMerger.SetTargets(lightAccumulationRTV);
            context.Rasterizer.SetViewports(viewport);
            context.OutputMerger.BlendState = blendState;
            context.OutputMerger.DepthStencilState = null;
        }

        public void DrawCombined()
        {
            shader.Effect.GetVariableByName("colorMap").AsResource().SetResource(gBuffer.DiffuseRV);
            shader.Effect.GetVariableByName("lightMap").AsResource().SetResource(LightAccumulationRV);


            shader.Apply();
            quad.Draw(layout);

            shader.Effect.GetVariableByName("colorMap").AsResource().SetResource(null);
            shader.Effect.GetVariableByName("lightMap").AsResource().SetResource(null);
            shader.Apply();



        }
    }
}
