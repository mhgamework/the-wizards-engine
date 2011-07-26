using System;
using System.Collections.Generic;
using DirectX11;
using DirectX11.Graphics;
using DirectX11.Rendering.CSM;
using DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering.SSAO;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using MapFlags = SlimDX.DXGI.MapFlags;
using Resource = SlimDX.Direct3D11.Resource;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    /// <summary>
    /// This is the facade class for the Deferred Renderer. This hides the DirectX11 layer.
    /// </summary>
    public class DeferredRenderer
    {
        private readonly DX11Game game;
        private DeferredMeshRenderer meshRenderer;
        private DirectionalLightRenderer directionalLightRenderer;
        private SpotLightRenderer spotLightRenderer;
        private PointLightRenderer pointLightRenderer;

        private List<DirectionalLight> directionalLights = new List<DirectionalLight>();
        private List<PointLight> pointLights = new List<PointLight>();
        private List<SpotLight> spotLights = new List<SpotLight>();
        private CombineFinalRenderer combineFinalRenderer;
        private GBuffer gBuffer;
        private TexturePool texturePool;
        private RenderTargetView hdrImageRtv;
        private ShaderResourceView hdrImageRV;
        private AverageLuminanceCalculater calculater;
        private ToneMapRenderer toneMap;
        private DeviceContext context;
        private Texture2D tempTex;
        private HorizonSSAORenderer ssao;


        public DeferredRenderer(DX11Game game)
        {
            this.game = game;
            var device = game.Device;
            context = device.ImmediateContext;

            int width = 800;
            int height = 600;

            gBuffer = new GBuffer(game.Device, width, height);
            texturePool = new TexturePool(game);

            meshRenderer = new DeferredMeshRenderer(game, gBuffer, texturePool);

            directionalLightRenderer = new DirectionalLightRenderer(game, gBuffer);
            spotLightRenderer = new SpotLightRenderer(game, gBuffer);
            pointLightRenderer = new PointLightRenderer(game, gBuffer);

            combineFinalRenderer = new CombineFinalRenderer(game, gBuffer);

            var desc = new Texture2DDescription
            {
                BindFlags =
                    BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R16G16B16A16_Float,
                Width = 800,
                Height = 600,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1
            };
            var hdrImage = new Texture2D(device, desc);

            hdrImageRtv = new RenderTargetView(device, hdrImage);
            hdrImageRV = new ShaderResourceView(device, hdrImage);

            calculater = new AverageLuminanceCalculater(game, hdrImageRV);

            toneMap = new ToneMapRenderer(game);


            var tempDesc = new Texture2DDescription
                               {
                                   ArraySize = 1,
                                   BindFlags = BindFlags.None,
                                   CpuAccessFlags = CpuAccessFlags.Read,
                                   Format = Format.R32_Float,
                                   Height = 1,
                                   MipLevels = 1,
                                   OptionFlags = ResourceOptionFlags.None,
                                   SampleDescription = new SampleDescription(1, 0),
                                   Usage = ResourceUsage.Staging,
                                   Width = 1
                               };

            tempTex = new Texture2D(device, tempDesc);


            
            ssao = new HorizonSSAORenderer(game, 800, 600);


           



        }

        public DirectionalLight CreateDirectionalLight()
        {
            var light = new DirectionalLight();
            directionalLights.Add(light);
            return light;
        }

        public PointLight CreatePointLight()
        {
            var light = new PointLight();
            pointLights.Add(light);
            return light;
        }

        public SpotLight CreateSpotLight()
        {
            var light = new SpotLight();
            spotLights.Add(light);
            return light;
        }

        public DeferredMeshRenderElement CreateMeshElement(IMesh mesh)
        {
            return meshRenderer.AddMesh(mesh);
        }

        public void Draw()
        {
            context.ClearState();
            gBuffer.Clear();
            gBuffer.SetTargetsToOutputMerger();
            meshRenderer.Draw();

            combineFinalRenderer.ClearLightAccumulation();

            // Possibly do all shadowmap up front, even cache some shadow maps.

            for (int i = 0; i < directionalLights.Count; i++)
            {
                var l = directionalLights[i];
                var r = directionalLightRenderer;
                r.LightDirection = l.LightDirection;
                r.Color = l.Color;
                r.ShadowsEnabled = l.ShadowsEnabled;

                if (l.ShadowsEnabled)
                    updateDirectionalShadows(r);
                combineFinalRenderer.SetLightAccumulationStates();

                r.Draw();
            }

            for (int i = 0; i < pointLights.Count; i++)
            {
                var l = pointLights[i];
                var r = pointLightRenderer;
                r.LightIntensity = l.LightIntensity;
                r.LightPosition = l.LightPosition;
                r.LightRadius = l.LightRadius;
                r.Color = l.Color;
                r.ShadowsEnabled = l.ShadowsEnabled;

                if (l.ShadowsEnabled)
                    updatePointShadows(r);

                combineFinalRenderer.SetLightAccumulationStates();

                r.Draw();
            }

            for (int i = 0; i < spotLights.Count; i++)
            {
                var l = spotLights[i];
                var r = spotLightRenderer;
                r.LightIntensity = l.LightIntensity;
                r.LightPosition = l.LightPosition;
                r.LightRadius = l.LightRadius;
                r.SpotDecayExponent = l.SpotDecayExponent;
                r.SpotDirection = l.SpotDirection;
                r.SpotLightAngle = l.SpotLightAngle;
                r.Color = l.Color;
                r.ShadowsEnabled = l.ShadowsEnabled;

                if (l.ShadowsEnabled)
                {
                    r.UpdateLightCamera();
                    game.Camera = r.LightCamera;
                    r.UpdateShadowMap(meshRenderer.Draw);
                    game.Camera = game.SpecaterCamera;
                }

                combineFinalRenderer.SetLightAccumulationStates();

                r.Draw();
            }



            ssao.OnFrameRender(gBuffer.DepthRV, gBuffer.NormalRV);

            game.Device.ImmediateContext.ClearState();
            game.SetBackbuffer(); // This is to set default viewport only, because i am lazy
            
            context.OutputMerger.SetTargets(hdrImageRtv);

            combineFinalRenderer.DrawCombined(ssao.MSsaoBuffer.pSRV);

            calculater.DrawUpdatedLogLuminance();
            calculater.DrawUpdatedAdaptedLogLuminance();

            context.ClearState();
            game.SetBackbuffer();

            toneMap.DrawTonemapped(hdrImageRV, calculater.CurrAverageLumRV);

            game.TextureRenderer.Draw(hdrImageRV, new Vector2(10, 10), new Vector2(100, 100));


            //var currAv = readPixel<float>(calculater.CurrAverageLumRV.Resource);
            //var av = 1e-5 + (float)Math.Exp(readPixel<float>(calculater.AverageLuminanceRV.Resource));
            //var text = string.Format("Average: {0:00.000}  Curr: {1:00.000}", av, currAv);
            //game.AddToWindowTitle(" " + text);
            //Console.WriteLine(text);

        }

        private T readPixel<T>(Resource resource) where T : struct
        {
            context.CopySubresourceRegion(resource, Resource.CalculateSubresourceIndex(0, 0, 1),
                                          new ResourceRegion(0, 0, 0, 1, 1, 1), tempTex,
                                          Resource.CalculateSubresourceIndex(0, 0, 1), 0, 0, 0);
            var data = context.MapSubresource(tempTex,
                                              Resource.CalculateSubresourceIndex(0, 0, 1), 4, MapMode.Read,
                                              SlimDX.Direct3D11.MapFlags.None);

            var ret = data.Data.Read<T>();
            context.UnmapSubresource(tempTex, Resource.CalculateSubresourceIndex(0, 0, 1));
            return ret;
        }

        private void updatePointShadows(PointLightRenderer r)
        {
            r.UpdateShadowMap(delegate(CustomCamera lightCamera)
                                  {
                                      game.Camera = lightCamera;
                                      meshRenderer.Draw();
                                      game.Camera = game.SpecaterCamera;
                                  });
        }

        private void updateDirectionalShadows(DirectionalLightRenderer r)
        {
            r.DrawUpdatedShadowmap(delegate(OrthographicCamera lightCamera)
                                       {
                                           game.Camera = lightCamera;
                                           meshRenderer.Draw();
                                           game.Camera = game.SpecaterCamera;
                                       }, game.Camera);
        }
    }
}
