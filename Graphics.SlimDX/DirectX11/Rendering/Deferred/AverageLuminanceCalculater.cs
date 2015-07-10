using MHGameWork.TheWizards.DirectX11.Graphics;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MHGameWork.TheWizards.DirectX11.Rendering.Deferred
{
    /// <summary>
    /// Responsible for calculating the average luminance of a hdr image
    /// Info:
    /// http://mynameismjp.wordpress.com/category/directx/
    /// </summary>
    public class AverageLuminanceCalculater
    {
        private readonly DX11Game game;
        private readonly ShaderResourceView hdrImage;
        private DeviceContext context;
        private BasicShader shader;


        private FullScreenQuad quad;
        private InputLayout layout;
        private RenderTargetView luminanceRTV;
        private ShaderResourceView averageLuminanceRV;
        private RenderTargetView prevAverageLumRTV;
        private ShaderResourceView prevAverageLumRV;
        private RenderTargetView currAverageLumRTV;
        private ShaderResourceView currAverageLumRV;
        private ShaderResourceView luminanceRV;
        private Texture2D luminanceMap;
        private Texture2D averageLum1;
        private Texture2D averageLum2;

        public AverageLuminanceCalculater(DX11Game game, ShaderResourceView hdrImage)
        {
            this.game = game;
            this.hdrImage = hdrImage;
            var device = game.Device;
            context = device.ImmediateContext;

            shader = BasicShader.LoadAutoreload(game,
                                                new System.IO.FileInfo(
                                                    CompiledShaderCache.Current.RootShaderPath + "Deferred\\Luminance.fx"));

            shader.SetTechnique("CalculateLuminance");

            quad = new FullScreenQuad(device);

            layout = FullScreenQuad.CreateInputLayout(device, shader.GetCurrentPass(0));



            var desc = new Texture2DDescription
            {
                BindFlags =
                    BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R32_Float,
                Width = hdrImage.Resource.AsSurface().Description.Width,
                Height = hdrImage.Resource.AsSurface().Description.Height,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 0,
                OptionFlags = ResourceOptionFlags.GenerateMipMaps
            };
            luminanceMap = new Texture2D(device, desc);

            luminanceRTV = new RenderTargetView(device, luminanceMap);
            LuminanceRV = new ShaderResourceView(device, luminanceMap);
            AverageLuminanceRV = new ShaderResourceView(device, luminanceMap, new ShaderResourceViewDescription
                                                                           {
                                                                               Dimension = ShaderResourceViewDimension.Texture2D,
                                                                               Flags = ShaderResourceViewExtendedBufferFlags.None,
                                                                               MipLevels = 1,
                                                                               MostDetailedMip = luminanceMap.Description.MipLevels - 1,
                                                                               Format = Format.R32_Float
                                                                           });

            

            var descMini = new Texture2DDescription
            {
                BindFlags =
                    BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R32_Float,
                Width = 1,
                Height = 1,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1,
            };

            averageLum1 = new Texture2D(device, descMini);
            averageLum2 = new Texture2D(device, descMini);

            prevAverageLumRTV = new RenderTargetView(device, averageLum1);
            prevAverageLumRV = new ShaderResourceView(device, averageLum1);
            currAverageLumRTV = new RenderTargetView(device, averageLum2);
            CurrAverageLumRV = new ShaderResourceView(device, averageLum2);
        }

        public ShaderResourceView LuminanceRV
        {
            get { return luminanceRV; }
            set { luminanceRV = value; }
        }

        public ShaderResourceView AverageLuminanceRV
        {
            get { return averageLuminanceRV; }
            set { averageLuminanceRV = value; }
        }

        public ShaderResourceView CurrAverageLumRV
        {
            get { return currAverageLumRV; }
            set { currAverageLumRV = value; }
        }


        public void DrawUpdatedLogLuminance()
        {
            context.ClearState();
            context.OutputMerger.SetTargets(luminanceRTV);
            context.Rasterizer.SetViewports(new Viewport(0, 0, luminanceMap.Description.Width,
                                                         luminanceMap.Description.Height));
            shader.SetTechnique("CalculateLuminance");

            shader.Effect.GetVariableByName("hdrImage").AsResource().SetResource(hdrImage);

            shader.Apply();
            quad.Draw(layout);

            shader.Effect.GetVariableByName("hdrImage").AsResource().SetResource(null);
            shader.Apply();

            //TODO: Do not use GenerateMips, generate the manually. This might solve the NAN problem
            generateMips();


        }

        private void generateMips()
        {
            context.GenerateMips(luminanceRV);
        }

        public void DrawUpdatedAdaptedLogLuminance()
        {
            context.ClearState();
            swapAverageLumMaps();

            context.OutputMerger.SetTargets(currAverageLumRTV);
            context.Rasterizer.SetViewports(new Viewport(0, 0, 1, 1));

            shader.SetTechnique("CalculateAverage");


            shader.Effect.GetVariableByName("lastLum").AsResource().SetResource(prevAverageLumRV);
            shader.Effect.GetVariableByName("currentLum").AsResource().SetResource(AverageLuminanceRV);
            shader.Effect.GetVariableByName("g_fDt").AsScalar().Set(game.Elapsed);


            shader.Apply();
            quad.Draw(layout);

            shader.Effect.GetVariableByName("lastLum").AsResource().SetResource(null);
            shader.Effect.GetVariableByName("currentLum").AsResource().SetResource(null);

            shader.Apply();

        }

        private void swapAverageLumMaps()
        {
            var swapRTV = prevAverageLumRTV;
            var swapRV = prevAverageLumRV;
            prevAverageLumRTV = currAverageLumRTV;
            prevAverageLumRV = CurrAverageLumRV;
            currAverageLumRTV = swapRTV;
            CurrAverageLumRV = swapRV;
        }
    }
}
