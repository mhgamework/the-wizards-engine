using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using DirectX11.Graphics;
using MHGameWork.TheWizards.Building;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Rendering;
using NUnit.Framework;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Tests.Building
{
    [TestFixture]
    public class BuildingTest
    {

        public static RAMMesh CreateMeshFromObj(OBJToRAMMeshConverter c, string obj, string mtl)
        {
            return RenderingTest.CreateMeshFromObj(c, obj, mtl);
        }

        [Test]
        public void TestMovementCamera()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var renderer = new DeferredRenderer(game);




            game.GameLoopEvent += delegate
                                      {
                                          game.SpectaterCamera.CameraPosition =
                                              new SlimDX.Vector3(game.SpectaterCamera.CameraPosition.X, 2,
                                                                 game.SpectaterCamera.CameraPosition.Z);

                                          renderer.Draw();

                                      };

            game.Run();

        }

        [Test]
        public void TestRenderHUD()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var renderer = new DeferredRenderer(game);
            var meshConverter = new OBJToRAMMeshConverter(new RAMTextureFactory());

            var mesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\BasicTestBlock.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\BasicTestBlock.mtl");

            var HUDRenderer = new HUDRenderer(game, renderer);
            HUDRenderer.setMesh(mesh);



            game.GameLoopEvent += delegate
            {
                game.SpectaterCamera.CameraPosition =
                    new SlimDX.Vector3(game.SpectaterCamera.CameraPosition.X, 2,
                                       game.SpectaterCamera.CameraPosition.Z);

                HUDRenderer.Update();
                renderer.Draw();
            };

            game.Run();
        }

        [Test]
        public void TestPlaceBlock()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var renderer = new DeferredRenderer(game);
            var meshConverter = new OBJToRAMMeshConverter(new RAMTextureFactory());

            var mesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\BasicTestBlock.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\BasicTestBlock.mtl");
            var planeMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.mtl");

            var HUDRenderer = new HUDRenderer(game, renderer);

            DirectionalLight light = renderer.CreateDirectionalLight();
            light.ShadowsEnabled = true;
            light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));

            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100));
            var ghost = renderer.CreateMeshElement(mesh);

            var placePos = new Vector3();



            game.GameLoopEvent += delegate
            {
                game.SpectaterCamera.CameraPosition =
                    new SlimDX.Vector3(game.SpectaterCamera.CameraPosition.X, 2,
                                       game.SpectaterCamera.CameraPosition.Z);

                Ray ray;
                Plane p = new Plane(new Vector3(0, 1, 0), 0);

                ray.Position = game.SpectaterCamera.CameraPosition;
                ray.Direction = game.SpectaterCamera.CameraDirection;

                var intersects = ray.xna().Intersects(p.xna());
                if (intersects.HasValue)
                {
                    placePos = ray.Position + intersects.Value * ray.Direction;
                }

                placePos.Y = 0;
                ghost.WorldMatrix = Matrix.Translation(placePos);

                if (game.Keyboard.IsKeyPressed(Key.NumberPad0))
                    HUDRenderer.setMesh(null);
                if (game.Keyboard.IsKeyPressed(Key.NumberPad1))
                    HUDRenderer.setMesh(mesh);
                if (game.Mouse.LeftMousePressed)
                    placeBlock(placePos);

                HUDRenderer.Update();

                renderer.Draw();

            };

            game.Run();
        }

        private void placeBlock(Vector3 pos)
        {

        }


    }
}
