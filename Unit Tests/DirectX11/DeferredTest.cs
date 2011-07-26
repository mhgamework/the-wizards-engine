using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DirectX11;
using DirectX11.Graphics;
using DirectX11.Rendering.CSM;
using DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.Graphics;
using NUnit.Framework;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;
using SlimDX.DXGI;
using BasicShader = DirectX11.Graphics.BasicShader;
using Buffer = SlimDX.Direct3D11.Buffer;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;

namespace MHGameWork.TheWizards.Tests.DirectX11
{
    [TestFixture]
    public class DeferredTest
    {
        private static readonly string HdrImageDDS = TWDir.GameData.CreateSubdirectory("Core") + "\\HdrImage.dds";

        [Test]
        public void TestGBuffer()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var buffer = new GBuffer(game.Device, 300, 300);

            buffer.Clear();

            game.GameLoopEvent += delegate
                                      {

                                          DrawGBuffer(game, buffer);
                                      };

            game.Run();

        }

        public static void DrawGBuffer(DX11Game game, GBuffer buffer)
        {
            game.TextureRenderer.Draw(buffer.DiffuseRV, new Vector2(10, 10),
                                      new Vector2(300, 300));
            game.TextureRenderer.Draw(buffer.NormalRV, new Vector2(320, 10),
                                      new Vector2(300, 300));
            game.TextureRenderer.Draw(buffer.DepthRV, new Vector2(10, 320),
                                      new Vector2(300, 300));
        }

        [Test]
        public void TestRenderToGBuffer()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;


            var filledGBuffer = new TestFilledGBuffer(game, 600, 600);


            game.GameLoopEvent += delegate
                                      {
                                          filledGBuffer.DrawUpdatedGBuffer();

                                          game.SetBackbuffer();

                                          DrawGBuffer(game, filledGBuffer.GBuffer);
                                      };

            game.Run();

        }

        [Test]
        public void TestDirectionalLightAccumulation()
        {
            //TODO: add a way to show the specular in the alpha channel

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var light = new DirectionalLightRenderer(game, filledGBuffer.GBuffer);

            game.GameLoopEvent += delegate
            {
                filledGBuffer.DrawUpdatedGBuffer();

                game.SetBackbuffer();

                if (game.Keyboard.IsKeyDown(Key.C))
                {
                    game.SpecaterCamera.Enabled = false;

                    var mat = Matrix.RotationY(game.Mouse.RelativeX * game.Elapsed * 5) * Matrix.RotationX(game.Mouse.RelativeY * game.Elapsed * 5);

                    light.LightDirection = Vector3.TransformNormal(light.LightDirection, mat);


                }
                else
                {
                    game.SpecaterCamera.Enabled = true;
                }

                if (game.Keyboard.IsKeyDown(Key.I))
                    DrawGBuffer(game, filledGBuffer.GBuffer);
                else
                    light.Draw();

            };

            game.Run();
        }
        [Test]
        public void TestCascadedShadowMapping()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var csmRenderer = new CSMRenderer(game);

            var toggle = false;

            var light = new DirectionalLight();


            game.GameLoopEvent += delegate
                                      {
                                          context.ClearState();
                                          filledGBuffer.DrawUpdatedGBuffer();

                                          csmRenderer.UpdateShadowMap(delegate(OrthographicCamera lightCamera)
                                                                          {
                                                                              game.Camera = lightCamera;

                                                                              filledGBuffer.Draw();

                                                                              game.Camera = game.SpecaterCamera;
                                                                          }, light, game.SpecaterCamera);

                                          game.SetBackbuffer();

                                          if (game.Keyboard.IsKeyPressed(Key.C))
                                              toggle = !toggle;

                                          if (toggle)
                                          {

                                              light.Direction = game.SpecaterCamera.CameraDirection;
                                          }


                                          if (game.Keyboard.IsKeyDown(Key.I))
                                              DrawGBuffer(game, filledGBuffer.GBuffer);
                                          else
                                          {
                                              csmRenderer.RenderShadowOcclusion(game.SpecaterCamera, filledGBuffer.GBuffer.DepthRV);
                                              //light.Draw();
                                              game.TextureRenderer.Draw(csmRenderer.ShadowMapRV, new Vector2(10, 10), new Vector2(590, 200));

                                              for (int i = 0; i < 6; i++)
                                              {
                                                  //game.LineManager3D.AddViewFrustum(light.LightCameras[i].ViewProjection,
                                                  //new Color4(0, 1, 0));
                                              }

                                          }

                                      };

            game.Run();

        }
        [Test]
        public void TestDirectionalLightShadowing()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var light = new DirectionalLightRenderer(game, filledGBuffer.GBuffer);

            light.ShadowsEnabled = true;

            var toggle = false;


            game.GameLoopEvent += delegate
            {
                filledGBuffer.DrawUpdatedGBuffer();


                light.DrawUpdatedShadowmap(delegate(OrthographicCamera lightCamera)
                {
                    game.Camera = lightCamera;

                    filledGBuffer.Draw();

                    game.Camera = game.SpecaterCamera;
                }, game.SpecaterCamera);


                game.SetBackbuffer();

                if (game.Keyboard.IsKeyPressed(Key.C))
                    toggle = !toggle;

                if (toggle)
                {

                    light.LightDirection = game.SpecaterCamera.CameraDirection;
                }


                if (game.Keyboard.IsKeyDown(Key.I))
                    DrawGBuffer(game, filledGBuffer.GBuffer);
                else
                {
                    light.Draw();
                    game.TextureRenderer.Draw(light.CSMRenderer.ShadowMapRV, new Vector2(10, 10), new Vector2(590, 200));
                    for (int i = 0; i < 6; i++)
                    {
                        //game.LineManager3D.AddViewFrustum(light.LightCameras[i].ViewProjection,
                        //new Color4(0, 1, 0));
                    }

                }

            };

            game.Run();
        }

        [Test]
        public void TestSpotLightRendererAccumulation()
        {
            //TODO: add a way to show the specular in the alpha channel

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var light = new SpotLightRenderer(game, filledGBuffer.GBuffer);

            var toggle = false;

            game.GameLoopEvent += delegate
            {
                filledGBuffer.DrawUpdatedGBuffer();

                game.SetBackbuffer();

                if (game.Keyboard.IsKeyPressed(Key.C))
                    toggle = !toggle;

                if (toggle)
                {

                    light.SpotDirection = game.SpecaterCamera.CameraDirection;
                    light.LightPosition = game.SpecaterCamera.CameraPosition;
                }

                if (game.Keyboard.IsKeyDown(Key.I))
                    DrawGBuffer(game, filledGBuffer.GBuffer);
                else
                    light.Draw();

            };

            game.Run();
        }
        [Test]
        public void TestPointLightAccumulation()
        {
            //TODO: add a way to show the specular in the alpha channel

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var light = new PointLightRenderer(game, filledGBuffer.GBuffer);

            var toggle = false;

            game.GameLoopEvent += delegate
            {
                filledGBuffer.DrawUpdatedGBuffer();

                game.SetBackbuffer();

                if (game.Keyboard.IsKeyPressed(Key.C))
                    toggle = !toggle;

                if (toggle)
                {

                    light.LightPosition = game.SpecaterCamera.CameraPosition;
                }

                if (game.Keyboard.IsKeyDown(Key.I))
                    DrawGBuffer(game, filledGBuffer.GBuffer);
                else
                    light.Draw();

            };

            game.Run();


            light.Dispose();
            filledGBuffer.Dispose();


        }

        [Test]
        public void TestMultipleLightAccumulation()
        {
            //TODO: add a way to show the specular in the alpha channel

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var spot = new SpotLightRenderer(game, filledGBuffer.GBuffer);
            var point = new PointLightRenderer(game, filledGBuffer.GBuffer);
            var directional = new DirectionalLightRenderer(game, filledGBuffer.GBuffer);

            var state = 0;

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


            var blendState = BlendState.FromDescription(device, bsDesc);
            var depthState = DepthStencilState.FromDescription(device, new DepthStencilStateDescription
                                                                           {
                                                                               IsDepthEnabled = false,
                                                                               IsStencilEnabled = false,

                                                                           });


            game.GameLoopEvent += delegate
            {
                filledGBuffer.DrawUpdatedGBuffer();

                game.SetBackbuffer();

                if (game.Keyboard.IsKeyPressed(Key.D1))
                    state = 0;
                if (game.Keyboard.IsKeyPressed(Key.D2))
                    state = 1;
                if (game.Keyboard.IsKeyPressed(Key.D3))
                    state = 2;
                if (game.Keyboard.IsKeyPressed(Key.D4))
                    state = 3;

                switch (state)
                {
                    case 0:
                        break;
                    case 1:
                        directional.LightDirection = game.SpecaterCamera.CameraDirection;
                        break;
                    case 2:
                        point.LightPosition = game.SpecaterCamera.CameraPosition;

                        break;
                    case 3:
                        spot.LightPosition = game.SpecaterCamera.CameraPosition;
                        spot.SpotDirection = game.SpecaterCamera.CameraDirection;
                        break;
                }



                if (game.Keyboard.IsKeyDown(Key.I))
                    DrawGBuffer(game, filledGBuffer.GBuffer);
                else
                {
                    context.OutputMerger.DepthStencilState = depthState;
                    directional.Draw();
                    context.OutputMerger.BlendState = blendState;
                    spot.Draw();
                    point.Draw();
                    context.OutputMerger.BlendState = null;
                    context.OutputMerger.DepthStencilState = null;

                }

            };

            game.Run();
        }

        [Test]
        public void TestCombineFinalRenderer()
        {
            //TODO: add a way to show the specular in the alpha channel

            var game = new DX11Game();
            game.InitDirectX();

            var test = new TestCombineFinalClass(game);

            game.GameLoopEvent += delegate
                                  {
                                      test.DrawUpdatedDeferredRendering();

                                      game.Device.ImmediateContext.ClearState();
                                      game.SetBackbuffer();

                                      test.DrawCombined();
                                  };

            game.Run();
        }

        [Test]
        public void TestToneMap()
        {

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;


            var toneMap = new ToneMapRenderer(game);


            var hdrImage = Texture2D.FromFile(device, HdrImageDDS);

            var hdrImageRV = new ShaderResourceView(device, hdrImage);

            var avgLuminance = 1f;

            game.GameLoopEvent += delegate
                                      {
                                          if (game.Keyboard.IsKeyDown(Key.UpArrow))
                                              avgLuminance += game.Elapsed;
                                          if (game.Keyboard.IsKeyDown(Key.DownArrow))
                                              avgLuminance -= game.Elapsed;

                                          toneMap.DrawTonemapped(hdrImageRV, avgLuminance);

                                      };

            game.Run();
        }

        [Test]
        public void TestCalculateAverageLogLuminance()
        {

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;




            var hdrImage1 = Texture2D.FromFile(device, HdrImageDDS);

            var hdrImage1RV = new ShaderResourceView(device, hdrImage1);



            var desc = new Texture2DDescription
            {
                BindFlags =
                    BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R16G16B16A16_Float,
                Width = 300,
                Height = 300,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.GenerateMipMaps
            };
            var hdrImage = new Texture2D(device, desc);

            var hdrImageRTV = new RenderTargetView(device, hdrImage);
            var hdrImageRV = new ShaderResourceView(device, hdrImage);
            var calculater = new AverageLuminanceCalculater(game, hdrImageRV);


            game.GameLoopEvent += delegate
                                      {
                                          context.OutputMerger.SetTargets(hdrImageRTV);
                                          context.Rasterizer.SetViewports(new Viewport(0, 0, 300, 300));

                                          if (game.Keyboard.IsKeyPressed(Key.D1))
                                              game.TextureRenderer.Draw(hdrImage1RV, new Vector2(), new Vector2(300, 300));
                                          if (game.Keyboard.IsKeyPressed(Key.D2))
                                              context.ClearRenderTargetView(hdrImageRTV, new Color4(1, 1, 1, 1));

                                          calculater.DrawUpdatedLogLuminance();

                                          context.ClearState();
                                          game.SetBackbuffer();

                                          game.TextureRenderer.Draw(calculater.LuminanceRV, new Vector2(10, 10),
                                                                     new Vector2(300, 300));

                                          game.TextureRenderer.Draw(calculater.AverageLuminanceRV, new Vector2(320, 10),
                                                                    new Vector2(300, 300));


                                      };

            game.Run();
        }
        [Test]
        public void TestAdjustedAverageLuminance()
        {

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;




            var hdrImage1 = Texture2D.FromFile(device, HdrImageDDS);

            var hdrImage1RV = new ShaderResourceView(device, hdrImage1);



            var desc = new Texture2DDescription
            {
                BindFlags =
                    BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R16G16B16A16_Float,
                Width = 300,
                Height = 300,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.GenerateMipMaps
            };
            var hdrImage = new Texture2D(device, desc);

            var hdrImageRTV = new RenderTargetView(device, hdrImage);
            var hdrImageRV = new ShaderResourceView(device, hdrImage);

            var calculater = new AverageLuminanceCalculater(game, hdrImageRV);


            game.GameLoopEvent += delegate
                                  {

                                      context.OutputMerger.SetTargets(hdrImageRTV);

                                      if (game.Keyboard.IsKeyPressed(Key.D1))
                                          game.TextureRenderer.Draw(hdrImage1RV, new Vector2(), new Vector2(300, 300));
                                      if (game.Keyboard.IsKeyPressed(Key.D2))
                                          context.ClearRenderTargetView(hdrImageRTV, new Color4(1, 1, 1, 1));


                                      calculater.DrawUpdatedLogLuminance();
                                      calculater.DrawUpdatedAdaptedLogLuminance();

                                      context.ClearState();
                                      game.SetBackbuffer();

                                      game.TextureRenderer.Draw(calculater.AverageLuminanceRV, new Vector2(10, 10),
                                                                new Vector2(300, 300));
                                      game.TextureRenderer.Draw(hdrImageRV, new Vector2(320, 10),
                                                                new Vector2(300, 300));
                                      game.TextureRenderer.Draw(calculater.CurrAverageLumRV, new Vector2(10, 320),
                                                                new Vector2(270, 270));

                                  };

            game.Run();
        }

        [Test]
        public void TestAutoAdjustTonemap()
        {

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;




            var hdrImage1 = Texture2D.FromFile(device, HdrImageDDS);

            var hdrImage1RV = new ShaderResourceView(device, hdrImage1);



            var desc = new Texture2DDescription
            {
                BindFlags =
                    BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R16G16B16A16_Float,
                Width = 300,
                Height = 300,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.GenerateMipMaps
            };
            var hdrImage = new Texture2D(device, desc);

            var hdrImageRTV = new RenderTargetView(device, hdrImage);
            var hdrImageRV = new ShaderResourceView(device, hdrImage);

            var calculater = new AverageLuminanceCalculater(game, hdrImageRV);

            var toneMap = new ToneMapRenderer(game);

            game.GameLoopEvent += delegate
            {
                context.Rasterizer.SetViewports(new Viewport(0, 0, 300, 300));

                context.OutputMerger.SetTargets(hdrImageRTV);

                if (game.Keyboard.IsKeyPressed(Key.D1))
                    game.TextureRenderer.Draw(hdrImage1RV, new Vector2(), new Vector2(300, 300));
                if (game.Keyboard.IsKeyPressed(Key.D2))
                    context.ClearRenderTargetView(hdrImageRTV, new Color4(1, 1, 1, 1));


                calculater.DrawUpdatedLogLuminance();
                calculater.DrawUpdatedAdaptedLogLuminance();

                context.ClearState();
                game.SetBackbuffer();


                //game.TextureRenderer.Draw(calculater.AverageLuminanceRV, new Vector2(10, 10),
                //                          new Vector2(300, 300));
                game.TextureRenderer.Draw(hdrImageRV, new Vector2(320, 10),
                                          new Vector2(300, 300));
                game.TextureRenderer.Draw(calculater.CurrAverageLumRV, new Vector2(10, 320),
                                          new Vector2(270, 270));

                context.Rasterizer.SetViewports(new Viewport(10, 10, 300, 300));

                toneMap.DrawTonemapped(hdrImageRV, calculater.CurrAverageLumRV);


            };

            game.Run();
        }

        [Test]
        public void TestLightsToneMap()
        {

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;





            var desc = new Texture2DDescription
            {
                BindFlags =
                    BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R16G16B16A16_Float,
                Width = 800,
                Height = 600,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.GenerateMipMaps
            };
            var hdrImage = new Texture2D(device, desc);

            var hdrImageRTV = new RenderTargetView(device, hdrImage);
            var hdrImageRV = new ShaderResourceView(device, hdrImage);

            var calculater = new AverageLuminanceCalculater(game, hdrImageRV);

            var toneMap = new ToneMapRenderer(game);

            var combineFinal = new TestCombineFinalClass(game);

            game.GameLoopEvent += delegate
                                  {
                                      combineFinal.DrawUpdatedDeferredRendering();
                                      if (game.Keyboard.IsKeyDown(Key.I)) return;
                                      context.Rasterizer.SetViewports(new Viewport(0, 0, 800, 600));

                                      context.OutputMerger.SetTargets(hdrImageRTV);

                                      combineFinal.DrawCombined();



                                      calculater.DrawUpdatedLogLuminance();
                                      calculater.DrawUpdatedAdaptedLogLuminance();

                                      context.ClearState();
                                      game.SetBackbuffer();


                                      if (game.Keyboard.IsKeyDown(Key.K))
                                      {

                                          game.TextureRenderer.Draw(calculater.AverageLuminanceRV, new Vector2(10, 10),
                                                                     new Vector2(300, 300));
                                          game.TextureRenderer.Draw(hdrImageRV, new Vector2(320, 10),
                                                                    new Vector2(300, 300));
                                          game.TextureRenderer.Draw(calculater.CurrAverageLumRV, new Vector2(10, 320),
                                                                    new Vector2(270, 270));
                                      }
                                      else

                                          toneMap.DrawTonemapped(hdrImageRV, calculater.CurrAverageLumRV);


                                  };

            game.Run();
        }

        [Test]
        public void TestSpotLightRendererShadowing()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var light = new SpotLightRenderer(game, filledGBuffer.GBuffer);
            light.LightRadius *= 2;

            light.ShadowsEnabled = true;

            var toggle = false;



            game.GameLoopEvent += delegate
                                      {
                                          filledGBuffer.DrawUpdatedGBuffer();

                                          light.UpdateLightCamera();
                                          game.Camera = light.LightCamera;

                                          light.UpdateShadowMap(filledGBuffer.Draw);

                                          game.Camera = game.SpecaterCamera;


                                          game.SetBackbuffer();

                                          if (game.Keyboard.IsKeyPressed(Key.C))
                                              toggle = !toggle;

                                          if (toggle)
                                          {

                                              light.SpotDirection = game.SpecaterCamera.CameraDirection;
                                              light.LightPosition = game.SpecaterCamera.CameraPosition;
                                          }

                                          if (game.Keyboard.IsKeyDown(Key.I))
                                              DrawGBuffer(game, filledGBuffer.GBuffer);
                                          else
                                          {
                                              light.Draw();
                                              game.TextureRenderer.Draw(light.ShadowMapRv, new Vector2(10, 10), new Vector2(300, 300));
                                              game.LineManager3D.AddViewFrustum(light.LightCamera.ViewProjection,
                                                                                new Color4(1, 0, 0));
                                          }

                                      };

            game.Run();
        }

        [Test]
        public void TestPointLightRendererShadowing()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var light = new PointLightRenderer(game, filledGBuffer.GBuffer);
            light.LightRadius *= 2;

            light.ShadowsEnabled = true;

            var toggle = false;


            game.GameLoopEvent += delegate
            {
                filledGBuffer.DrawUpdatedGBuffer();

                light.UpdateShadowMap(delegate(CustomCamera lightCamera)
                                          {
                                              game.Camera = lightCamera;

                                              filledGBuffer.Draw();

                                              game.Camera = game.SpecaterCamera;
                                          });


                game.SetBackbuffer();

                if (game.Keyboard.IsKeyPressed(Key.C))
                    toggle = !toggle;

                if (toggle)
                {

                    light.LightPosition = game.SpecaterCamera.CameraPosition;
                }


                if (game.Keyboard.IsKeyDown(Key.I))
                    DrawGBuffer(game, filledGBuffer.GBuffer);
                else
                {
                    light.Draw();
                    //game.TextureRenderer.Draw(light.ShadowCubeMapRv, new Vector2(10, 10), new Vector2(300, 300));
                    for (int i = 0; i < 6; i++)
                    {
                        game.LineManager3D.AddViewFrustum(light.LightCameras[i].ViewProjection,
                                                  new Color4(0, 1, 0));
                    }

                }

            };

            game.Run();
        }



        public class TestCombineFinalClass
        {
            private readonly DX11Game game;
            public TestFilledGBuffer FilledGBuffer { get; private set; }
            private SpotLightRenderer spot;
            private PointLightRenderer point;
            private DirectionalLightRenderer directional;
            private int state;
            private CombineFinalRenderer combineFinal;
            private Texture2D hdrImage;
            private RenderTargetView hdrImageRTV;
            private ShaderResourceView hdrImageRV;
            private DeviceContext context;

            public TestCombineFinalClass(DX11Game game)
            {
                this.game = game;

                var device = game.Device;
                context = device.ImmediateContext;

                FilledGBuffer = new TestFilledGBuffer(game, 800, 600);

                spot = new SpotLightRenderer(game, FilledGBuffer.GBuffer);
                point = new PointLightRenderer(game, FilledGBuffer.GBuffer);
                directional = new DirectionalLightRenderer(game, FilledGBuffer.GBuffer);

                state = 0;


                combineFinal = new CombineFinalRenderer(game, FilledGBuffer.GBuffer);



                var desc = new Texture2DDescription
                {
                    BindFlags =
                        BindFlags.RenderTarget | BindFlags.ShaderResource,
                    Format = Format.R16G16B16A16_Float,
                    Width = FilledGBuffer.GBuffer.Width,
                    Height = FilledGBuffer.GBuffer.Height,
                    ArraySize = 1,
                    SampleDescription = new SampleDescription(1, 0),
                    MipLevels = 1
                };
                hdrImage = new Texture2D(device, desc);

                hdrImageRTV = new RenderTargetView(device, hdrImage);
                hdrImageRV = new ShaderResourceView(device, hdrImage);

            }

            public void DrawUpdatedDeferredRendering()
            {
                FilledGBuffer.DrawUpdatedGBuffer();


                if (game.Keyboard.IsKeyPressed(Key.D1))
                    state = 0;
                if (game.Keyboard.IsKeyPressed(Key.D2))
                    state = 1;
                if (game.Keyboard.IsKeyPressed(Key.D3))
                    state = 2;
                if (game.Keyboard.IsKeyPressed(Key.D4))
                    state = 3;

                switch (state)
                {
                    case 0:
                        break;
                    case 1:
                        directional.LightDirection = game.SpecaterCamera.CameraDirection;
                        break;
                    case 2:
                        point.LightPosition = game.SpecaterCamera.CameraPosition;

                        break;
                    case 3:
                        spot.LightPosition = game.SpecaterCamera.CameraPosition;
                        spot.SpotDirection = game.SpecaterCamera.CameraDirection;
                        break;
                }



                if (game.Keyboard.IsKeyDown(Key.I))
                {
                    context.ClearState();
                    game.SetBackbuffer();
                    DrawGBuffer(game, FilledGBuffer.GBuffer);

                }
                else
                {
                    combineFinal.ClearLightAccumulation();
                    combineFinal.SetLightAccumulationStates();
                    directional.Draw();
                    spot.Draw();
                    point.Draw();




                    context.ClearState();
                    game.SetBackbuffer(); // This is to set viewport, not sure this is correct

                    context.OutputMerger.SetTargets(hdrImageRTV);
                    DrawCombined();



                }



                if (game.Keyboard.IsKeyPressed(Key.O))
                {
                    Resource.SaveTextureToFile(game.Device.ImmediateContext, hdrImage, ImageFileFormat.Dds,
                                               TWDir.Test.CreateSubdirectory("Deferred") + "\\HdrImage.dds");
                }
            }

            public void DrawCombined()
            {
                combineFinal.DrawCombined();
            }
            public void DrawCombined(ShaderResourceView ambientOcclusionSRV)
            {
                combineFinal.DrawCombined(ambientOcclusionSRV);
            }
        }

        public class TestFilledGBuffer : IDisposable
        {
            private readonly DX11Game game;
            private BasicShader shader;
            private TestBox box;
            private ShaderResourceView diffuseTextureRv;

            public TestFilledGBuffer(DX11Game game, int width, int height)
            {
                this.game = game;
                GBuffer = new GBuffer(game.Device, width, height);

                var device = game.Device;
                var context = device.ImmediateContext;


                var diffuseTexture = Texture2D.FromFile(device, DirectX11Test.Wallpaper001_png);
                diffuseTextureRv = new ShaderResourceView(device, diffuseTexture);

                shader = BasicShader.LoadAutoreload(game, new FileInfo("..\\..\\DirectX11\\Shaders\\Deferred\\RenderGBuffer.fx"));

                shader.SetTechnique("Technique1");


                box = new TestBox(device, shader.GetCurrentPass(0).Description.Signature);



            }

            public GBuffer GBuffer { get; set; }

            public void DrawUpdatedGBuffer()
            {
                GBuffer.Clear();
                GBuffer.SetTargetsToOutputMerger();

                Draw();

            }

            public void Draw()
            {
                game.Device.ImmediateContext.Rasterizer.State = game.HelperStates.RasterizerShowAll;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        shader.Effect.GetVariableByName("txDiffuse").AsResource().SetResource(diffuseTextureRv);
                        shader.Effect.GetVariableByName("World").AsMatrix().SetMatrix(Matrix.Translation(8 * i, 0, 8 * j));
                        shader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
                        shader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);
                        shader.Apply();

                        box.Draw();
                    }
                }

                for (int j = 0; j < 3; j++)
                {
                    shader.Effect.GetVariableByName("txDiffuse").AsResource().SetResource(diffuseTextureRv);
                    shader.Effect.GetVariableByName("World").AsMatrix().SetMatrix(Matrix.Translation(18, 1, 8 * j));
                    shader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
                    shader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);
                    shader.Apply();

                    box.Draw();
                }
            }

            public void Dispose()
            {
                shader.Dispose();
                box.Dispose();
                diffuseTextureRv.Dispose();
                GBuffer.Dispose();
            }
        }

        /// <summary>
        /// A test box object that draws a box with position(4), normal(3), and uv(2) vertices
        /// </summary>
        public class TestBox : IDisposable
        {
            private readonly Device device;
            private Buffer vertexBuffer;
            private Buffer indexBuffer;
            private InputLayout layout;
            private DeviceContext context;

            public struct Vertex
            {
                public Vector4 Pos;
                public Vector3 Normal;
                public Vector2 UV;

                public const int SizeInBytes = 4 * (4 + 3 + 2);
                public static readonly InputElement[] Elements = new[]
                                                                {
                                                                    new InputElement("POSITION",0, SlimDX.DXGI.Format.R32G32B32A32_Float,0,0),
                                                                    new InputElement("NORMAL",0, SlimDX.DXGI.Format.R32G32B32_Float,16,0),
                                                                    new InputElement("TEXCOORD",0, SlimDX.DXGI.Format.R32G32_Float,16+12,0)
                                                                };
            }

            public TestBox(Device device, ShaderSignature signature)
            {
                this.device = device;
                context = device.ImmediateContext;

                layout = new InputLayout(device, signature, Vertex.Elements);
                TangentVertex[] vertices;
                short[] indices;
                BoxMesh.CreateUnitBoxVerticesAndIndices(out vertices, out indices);

                var verts = vertices.Select(v => new Vertex
                {
                    Pos = new Vector4(v.pos.ToSlimDX() * 5, 1),
                    Normal = v.normal.ToSlimDX(),
                    UV = v.uv.ToSlimDX()
                }).ToArray();


                using (var strm = new DataStream(verts, true, false))
                {
                    vertexBuffer = new Buffer(device, strm, new BufferDescription
                    {
                        BindFlags = BindFlags.VertexBuffer,
                        Usage = ResourceUsage.Immutable,
                        SizeInBytes = (int)strm.Length
                    });
                }

                using (var strm = new DataStream(indices, true, false))
                {
                    indexBuffer = new Buffer(device, strm, new BufferDescription
                    {
                        BindFlags = BindFlags.IndexBuffer,
                        Usage = ResourceUsage.Immutable,
                        SizeInBytes = indices.Length * 2
                    });
                }
            }

            public void Draw()
            {
                context.InputAssembler.InputLayout = layout;
                context.InputAssembler.SetVertexBuffers(0,
                                                                        new VertexBufferBinding(vertexBuffer,
                                                                                                Vertex.
                                                                                                    SizeInBytes, 0));
                context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                context.InputAssembler.SetIndexBuffer(indexBuffer, SlimDX.DXGI.Format.R16_UInt, 0);

                context.DrawIndexed(6 * 2 * 3, 0, 0);

            }

            public void Dispose()
            {
                vertexBuffer.Dispose();
                indexBuffer.Dispose();
                layout.Dispose();

            }
        }


    }

}
