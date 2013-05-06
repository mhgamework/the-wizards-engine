using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering.Deferred.Meshes;
using MHGameWork.TheWizards.Tests.Features.Rendering.Deferred;
using NUnit.Framework;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Showcase
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class ShowcaseTest
    {
        private const string ShowcaseCamera = "../../Unit Tests/Features/Rendering/Showcase/camera.xml";

        [Test]
        public void TestCompleteShowcase()
        {
            var game = createGame();

            var renderer = new DeferredRenderer(game);

            var showcase = new ShowcaseSceneBuilder();
            showcase.CreateScene(renderer);

            game.GameLoopEvent += g =>
            {
                renderer.Draw();
                if (game.Keyboard.IsKeyDown(Key.C))
                {
                    var s = createCameraSerializer();
                    using (var fs = File.OpenWrite("camera.xml"))
                        s.Serialize(fs, game.SpectaterCamera);
                }
            };

            game.Run();
        }

        private static XmlSerializer createCameraSerializer()
        {
            return new XmlSerializer(typeof(SpectaterCamera));
        }

        [Xunit.Fact]
        public void test()
        {
            var p = new ObjImporter();
        }

        [Xunit.Fact]
        [Test]
        public void TestShowcaseSingleFrame()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            var game = createGame();

            game.SpectaterCamera = loadCamera(ShowcaseCamera);
            game.Camera = game.SpectaterCamera;

            var renderer = new DeferredRenderer(game);

            var showcase = new ShowcaseSceneBuilder();
            showcase.CreateScene(renderer);

            game.GameLoopEvent += delegate
            {
                renderer.Draw();
                game.Exit();
            };

            game.Run();
        }

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return null;
        }

        private SpectaterCamera loadCamera(string showcaseCamera)
        {
            var s = createCameraSerializer();
            using (var fs = File.OpenRead(showcaseCamera))
                return (SpectaterCamera)s.Deserialize(fs);
        }

        [Test]
        public void TestShowcaseGBuffer()   
        {
            drawMeshTest(RenderingTestsHelper.CreateMeshFromObj(new OBJToRAMMeshConverter(new RAMTextureFactory()),
                                                                  TWDir.GameData +
                                                                  RenderingTestsHelper.ShowcaseOBJ,
                                                                  TWDir.GameData +
                                                                  RenderingTestsHelper.ShowcaseMTL), Matrix.Scaling(1, 1, 1));
        }




        public void drawMeshTest(IMesh mesh, Matrix worldMatrix)
        {
            var game = createGame();


            var pool = createTexturePool(game);

            var worldMesh = new WorldMesh { Mesh = mesh, WorldMatrix = worldMatrix };

            var meshes = new MeshesRenderer(new RendererResourcePool(new MeshRenderDataFactory(game, null, pool), pool, game), game);


            var ctx = game.Device.ImmediateContext;

            var gBuffer = createGBuffer(game);
            var scene = new RenderingTestsHelper.SimpleLightedScene(game, gBuffer);


            var list = new List<WorldMesh>();
            list.Add(worldMesh);

            var buffer = gBuffer;
            ctx = game.Device.ImmediateContext;
            // Non-related init code

            var point = new PointLightRenderer(game, buffer);

            point.LightRadius = 3;
            point.LightIntensity = 1;
            point.ShadowsEnabled = false;

            var angle = 0f;

            var combineFinal = new CombineFinalRenderer(game, buffer);

            var raster = new RasterizerStateDescription()
                {
                    CullMode = CullMode.Front,
                    FillMode = FillMode.Solid

                };

            var state = RasterizerState.FromDescription(game.Device, raster);
            point.LightRadius = 10;

            game.GameLoopEvent += delegate
            {
                ctx.ClearState();
                ctx.Rasterizer.State = state;
                gBuffer.Clear();
                gBuffer.SetTargetsToOutputMerger();

                meshes.DrawMeshes(list, game.Camera);

                angle += MathHelper.Pi * game.Elapsed;

                var alpha = 0.99f * game.Elapsed * 20000;

                var cam = game.Camera.ViewInverse.GetTranslation();
                var diff = cam - point.LightPosition;
                diff.Normalize();


                point.LightPosition += diff * game.Elapsed * 2;// new Vector3((float)Math.Sin(angle), (float)Math.Cos(angle), -2);

                ctx.ClearState();
                combineFinal.SetLightAccumulationStates();
                combineFinal.ClearLightAccumulation();
                point.Draw();

                ctx.ClearState();
                game.SetBackbuffer();
                ctx.Rasterizer.SetViewports(new Viewport(400, 300, 400, 300));

                combineFinal.DrawCombined();


                game.SetBackbuffer();
                GBufferTest.DrawGBuffer(game, buffer);
            };

            game.Run();
        }

        private DX11Game createGame()
        {
            var ret = new DX11Game();
            ret.InitDirectX();
            return ret;
        }

        private static TheWizards.Rendering.Deferred.TexturePool createTexturePool(DX11Game game)
        {
            return new TheWizards.Rendering.Deferred.TexturePool(game);
        }

        private static GBuffer createGBuffer(DX11Game game1)
        {
            return new GBuffer(game1.Device, 800, 600);
        }

    }
}