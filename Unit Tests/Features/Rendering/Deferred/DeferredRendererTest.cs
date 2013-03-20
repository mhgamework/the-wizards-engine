using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Deferred
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class DeferredRendererTest
    {
        [Test]
        public void TestDeferredRendererLineElement()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var renderer = new DeferredRenderer(game);

            var el = renderer.CreateLinesElement();

            el.Lines.AddBox(new BoundingBox(Vector3.Zero, MathHelper.One), new Color4(1, 0, 0));


            game.GameLoopEvent += delegate
            {
                renderer.Draw();

            };

            game.Run();
        }


        [Test]
        public void TestDeferredRendererSimple()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var renderer = new DeferredRenderer(game);

            var otherCam = new SpectaterCamera(game.Keyboard, game.Mouse, 1, 10000);

            var mesh = RenderingTestsHelper.CreateMerchantsHouseMesh(new OBJToRAMMeshConverter(new RAMTextureFactory()));

            var el = renderer.CreateMeshElement(mesh);
            var directional = renderer.CreateDirectionalLight();
            directional.ShadowsEnabled = true;
            var point = renderer.CreatePointLight();
            point.LightRadius *= 2;
            point.ShadowsEnabled = true;
            var spot = renderer.CreateSpotLight();
            spot.LightRadius *= 2;
            spot.ShadowsEnabled = true;

            int state = 0;

            var camState = false;

            game.GameLoopEvent += delegate
            {
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
                        directional.LightDirection = game.SpectaterCamera.CameraDirection;
                        break;
                    case 2:
                        point.LightPosition = game.SpectaterCamera.CameraPosition;

                        break;
                    case 3:
                        spot.LightPosition = game.SpectaterCamera.CameraPosition;
                        spot.SpotDirection = game.SpectaterCamera.CameraDirection;
                        break;
                }

                if (game.Keyboard.IsKeyPressed(Key.C))
                    camState = !camState;

                if (camState)
                {
                    game.Camera = game.SpectaterCamera;
                    renderer.DEBUG_SeperateCullCamera = null;
                }
                else
                {
                    game.Camera = otherCam;
                    renderer.DEBUG_SeperateCullCamera = game.SpectaterCamera;


                }
                game.SpectaterCamera.EnableUserInput = camState;
                otherCam.EnableUserInput = !camState;
                otherCam.Update(game.Elapsed);
                renderer.Draw();

            };

            game.Run();
        }
        [Test]
        public void TestDeferredRendererCastsShadowsField()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var renderer = new DeferredRenderer(game);

            var otherCam = new SpectaterCamera(game.Keyboard, game.Mouse, 1, 10000);

            var mesh = RenderingTestsHelper.CreateMerchantsHouseMesh(new OBJToRAMMeshConverter(new RAMTextureFactory()));


            var el = renderer.CreateMeshElement(mesh);
            el.CastsShadows = false;
            var directional = renderer.CreateDirectionalLight();
            directional.ShadowsEnabled = true;
            var point = renderer.CreatePointLight();
            point.LightRadius *= 2;
            point.ShadowsEnabled = true;
            var spot = renderer.CreateSpotLight();
            spot.LightRadius *= 2;
            spot.ShadowsEnabled = true;

            int state = 0;

            var camState = false;

            game.GameLoopEvent += delegate
            {
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
                        directional.LightDirection = game.SpectaterCamera.CameraDirection;
                        break;
                    case 2:
                        point.LightPosition = game.SpectaterCamera.CameraPosition;

                        break;
                    case 3:
                        spot.LightPosition = game.SpectaterCamera.CameraPosition;
                        spot.SpotDirection = game.SpectaterCamera.CameraDirection;
                        break;
                }

                if (game.Keyboard.IsKeyPressed(Key.C))
                    camState = !camState;

                if (camState)
                {
                    game.Camera = game.SpectaterCamera;
                    renderer.DEBUG_SeperateCullCamera = null;
                }
                else
                {
                    game.Camera = otherCam;
                    renderer.DEBUG_SeperateCullCamera = game.SpectaterCamera;


                }
                game.SpectaterCamera.EnableUserInput = camState;
                otherCam.EnableUserInput = !camState;
                otherCam.Update(game.Elapsed);
                renderer.Draw();

            };

            game.Run();
        }


    }
}