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

        private FrustumCuller frustumCuller;
        private DeferredMeshRenderer meshRenderer;
        private FrustumCullerView gbufferView;


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



            Vector3 radius = new Vector3(500, 1000, 500);
            frustumCuller = new FrustumCuller(new BoundingBox(-radius, radius), 8);

            gbufferView = frustumCuller.CreateView();
            meshRenderer.Culler = frustumCuller;

        }

        public DirectionalLight CreateDirectionalLight()
        {
            var light = new DirectionalLight(frustumCuller);
            directionalLights.Add(light);
            return light;
        }

        public PointLight CreatePointLight()
        {
            var light = new PointLight(frustumCuller);
            pointLights.Add(light);
            return light;
        }

        public SpotLight CreateSpotLight()
        {
            var light = new SpotLight(frustumCuller);
            spotLights.Add(light);
            return light;
        }

        public DeferredMeshRenderElement CreateMeshElement(IMesh mesh)
        {
            if (mesh == null) throw new NullReferenceException();
            var el = meshRenderer.AddMesh(mesh);

            return el;
        }

        public void Draw()
        {
            context.ClearState();


            drawGBuffer();

            combineFinalRenderer.ClearLightAccumulation();


            drawLights();

            updateSSAO();

            drawHdrImage();

            updateTonemapLuminance();

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

        private void updateTonemapLuminance()
        {
            calculater.DrawUpdatedLogLuminance();
            calculater.DrawUpdatedAdaptedLogLuminance();
        }

        private void drawHdrImage()
        {
            game.Device.ImmediateContext.ClearState();
            game.SetBackbuffer(); // This is to set default viewport only, because i am lazy

            context.OutputMerger.SetTargets(hdrImageRtv);

            combineFinalRenderer.DrawCombined(ssao.MSsaoBuffer.pSRV);
        }

        private void updateSSAO()
        {
            ssao.OnFrameRender(gBuffer.DepthRV, gBuffer.NormalRV);
        }

        private void drawLights()
        {
            // Possibly do all shadowmap up front, even cache some shadow maps.

            for (int i = 0; i < directionalLights.Count; i++)
            {
                var l = directionalLights[i];
                var r = directionalLightRenderer;
                setDirectionalLightToRenderer(r, l);

                if (l.ShadowsEnabled)
                {
                    updateDirectionalShadows(r, l.ShadowViews);
                }
                combineFinalRenderer.SetLightAccumulationStates();

                r.Draw();
            }

            for (int i = 0; i < pointLights.Count; i++)
            {
                var l = pointLights[i];
                var r = pointLightRenderer;
                setPointLightToRenderer(r, l);

                if (l.ShadowsEnabled)
                    updatePointShadows(r, l.Views);

                combineFinalRenderer.SetLightAccumulationStates();

                r.Draw();
            }

            for (int i = 0; i < spotLights.Count; i++)
            {
                var l = spotLights[i];
                var r = spotLightRenderer;
                setSpotLightToRenderer(r, l);

                if (l.ShadowsEnabled)
                {
                    updateSpotShadows(r,l.ShadowView);
                }

                combineFinalRenderer.SetLightAccumulationStates();

                r.Draw();
            }
        }

        private void setSpotLightToRenderer(SpotLightRenderer r, SpotLight l)
        {
            r.LightIntensity = l.LightIntensity;
            r.LightPosition = l.LightPosition;
            r.LightRadius = l.LightRadius;
            r.SpotDecayExponent = l.SpotDecayExponent;
            r.SpotDirection = l.SpotDirection;
            r.SpotLightAngle = l.SpotLightAngle;
            r.Color = l.Color;
            r.ShadowsEnabled = l.ShadowsEnabled;
        }

        private void setDirectionalLightToRenderer(DirectionalLightRenderer r, DirectionalLight l)
        {
            r.LightDirection = l.LightDirection;
            r.Color = l.Color;
            r.ShadowsEnabled = l.ShadowsEnabled;
        }

        private void setPointLightToRenderer(PointLightRenderer r, PointLight l)
        {
            r.LightIntensity = l.LightIntensity;
            r.LightPosition = l.LightPosition;
            r.LightRadius = l.LightRadius;
            r.Color = l.Color;
            r.ShadowsEnabled = l.ShadowsEnabled;
        }

        private void drawGBuffer()
        {
            gBuffer.Clear();
            gBuffer.SetTargetsToOutputMerger();

            drawGBufferMeshes();
        }

        private void drawGBufferMeshes()
        {
            var cullCam = DEBUG_SeperateCullCamera;
            if (cullCam == null) cullCam = game.Camera;

            //TODO: fix culling+removing of elements
            //gbufferView.UpdateVisibility(cullCam.ViewProjection);
            //setMeshRendererVisibles(gbufferView);
            meshRenderer.Draw();
        }

        private void setMeshRendererVisibles(FrustumCullerView view)
        {

            var cullables = view.GetVisibleCullables();
            setAllMeshesInvisible();
            for (int i = 0; i < cullables.Count; i++)
            {
                var c = (DeferredMeshRenderElement)cullables[i];
                c.Visible = true;
                //game.LineManager3D.AddBox(c.BoundingBox.dx(), new Color4(0, 1, 0));
            }
        }

        /// <summary>
        /// This could be optimized later on
        /// </summary>
        private void setAllMeshesInvisible()
        {
            for (int i = 0; i < meshRenderer.Elements.Count; i++)
            {
                meshRenderer.Elements[i].Visible = false;
            }
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



        private void updateDirectionalShadows(DirectionalLightRenderer r, FrustumCullerView[] views)
        {
            var mainCamera = game.Camera;
            if (DEBUG_SeperateCullCamera != null) mainCamera = DEBUG_SeperateCullCamera;
            int num = 0;
            r.DrawUpdatedShadowmap(delegate(OrthographicCamera lightCamera)
                                       {
                                           var view = views[0];
                                           num++;
                                           var oldCam = game.Camera;
                                           game.Camera = lightCamera;
                                           //TODO: fix culling+removing of elements
                                           //view.UpdateVisibility(lightCamera.ViewProjection);
                                           //setMeshRendererVisibles(view);
                                           meshRenderer.DrawDepthOnly();
                                           game.Camera = oldCam;
                                       }, mainCamera);
        }
        private void updatePointShadows(PointLightRenderer r, FrustumCullerView[] views)
        {
            int num = 0;
            r.UpdateShadowMap(delegate(CustomCamera lightCamera)
                                  {
                                      var view = views[num];
                                      num++;
                                      var oldCam = game.Camera;
                                      game.Camera = lightCamera;
                                      //TODO: fix culling+removing of elements
                                      //view.UpdateVisibility(lightCamera.ViewProjection);
                                      //setMeshRendererVisibles(view);
                                      meshRenderer.DrawDepthOnly();
                                      game.Camera = oldCam;
                                  });
        }
        private void updateSpotShadows(SpotLightRenderer r, FrustumCullerView view)
        {
            var oldCam = game.Camera;
            r.UpdateLightCamera();
            game.Camera = r.LightCamera;
            //TODO: fix culling+removing of elements
            //view.UpdateVisibility(r.LightCamera.ViewProjection);
            //setMeshRendererVisibles(view);
            r.UpdateShadowMap(meshRenderer.DrawDepthOnly);
            game.Camera = oldCam;
        }

        public DeferredMeshRenderer DEBUG_MeshRenderer { get { return meshRenderer; } }
        public FrustumCuller DEBUG_FrustumCuller { get { return frustumCuller; } }
        public ICamera DEBUG_SeperateCullCamera { get; set; }

    }
}
