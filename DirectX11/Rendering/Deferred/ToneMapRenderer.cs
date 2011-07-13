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
    public class ToneMapRenderer
    {
        private readonly DX11Game game;
        private DeviceContext context;
        private BasicShader shader;


        private FullScreenQuad quad;
        private InputLayout layout;
        private RenderTargetView averageLumRTV;
        private ShaderResourceView averageLumRV;

        public ToneMapRenderer(DX11Game game)
        {
            this.game = game;
            var device = game.Device;
            context = device.ImmediateContext;

            shader = BasicShader.LoadAutoreload(game,
                                                new System.IO.FileInfo(
                                                    "..\\..\\DirectX11\\Shaders\\Deferred\\ToneMap.fx"));

            shader.SetTechnique("Technique0");

            quad = new FullScreenQuad(device);

            layout = FullScreenQuad.CreateInputLayout(device, shader.GetCurrentPass(0));


            //Create test average rt

            var descMini = new Texture2DDescription
            {
                BindFlags =
                    BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R32_Float,
                Width = 1,
                Height = 1,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1
            };

            var averageLum = new Texture2D(device, descMini);

            averageLumRTV = new RenderTargetView(device, averageLum);
            averageLumRV = new ShaderResourceView(device, averageLum);

        }

        /// <summary>
        /// Helper function for testing, this uses the overloaded method, but renders the averageLuminance to a texture first
        /// </summary>
        /// <param name="hdrImage"></param>
        /// <param name="averageLuminance"></param>
        public void DrawTonemapped(ShaderResourceView hdrImage, float averageLuminance)
        {
            context.ClearRenderTargetView(averageLumRTV, new Color4(averageLuminance, 0, 0));
            DrawTonemapped(hdrImage, averageLumRV);



        }
        /// <summary>
        /// averageLuminance is a texture when sampled at 0,0 contains the average luminance in the r channel
        /// </summary>
        /// <param name="hdrImage"></param>
        /// <param name="averageLuminance"></param>
        public void DrawTonemapped(ShaderResourceView hdrImage, ShaderResourceView averageLuminance)
        {
            shader.Effect.GetVariableByName("finalMap").AsResource().SetResource(hdrImage);
            shader.Effect.GetVariableByName("lumAvgMap").AsResource().SetResource(averageLuminance);


            shader.Apply();
            quad.Draw(layout);

            shader.Effect.GetVariableByName("finalMap").AsResource().SetResource(null);
            shader.Apply();



        }
    }
}
