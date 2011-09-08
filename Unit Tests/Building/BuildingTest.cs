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

            var factory = new BlockTypeFactory();

            var mesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\BasicTestBlock.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\BasicTestBlock.mtl");

            var HUDRenderer = new HUDRenderer(game, renderer);
            HUDRenderer.SetBlockType(factory.CreateNewBlockType(mesh));



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
            var blockFactory = new BlockFactory(renderer);
            var blockTypeFactory = new BlockTypeFactory();

            var HUDRenderer = new HUDRenderer(game, renderer);
            var placeTool = new PlaceTool(game, renderer, blockFactory);


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

            DirectionalLight light = renderer.CreateDirectionalLight();
            light.ShadowsEnabled = true;
            light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));

            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100));

            var activeType = blockTypeFactory.CreateNewBlockType(mesh);
            placeTool.SetBlockType(activeType);


            game.GameLoopEvent += delegate
            {
                game.SpectaterCamera.CameraPosition =
                    new SlimDX.Vector3(game.SpectaterCamera.CameraPosition.X, 2,
                                       game.SpectaterCamera.CameraPosition.Z);


                HUDRenderer.SetBlockType(activeType);
                placeTool.SetBlockType(activeType);

                HUDRenderer.Update();
                placeTool.Update();

                renderer.Draw();

            };

            game.Run();
        }


       [Test]
        public void TestPlayerMovement()
       {
           var game = new DX11Game();
           game.InitDirectX();

           var renderer = new DeferredRenderer(game);
           var blockFactory = new BlockFactory(renderer);
           var blockTypeFactory = new BlockTypeFactory();

           var HUDRenderer = new HUDRenderer(game, renderer);
           var placeTool = new PlaceTool(game, renderer, blockFactory);


           var playerController = new PlayerController(game, blockFactory);


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

           DirectionalLight light = renderer.CreateDirectionalLight();
           light.ShadowsEnabled = true;
           light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));

           var planeEl = renderer.CreateMeshElement(planeMesh);
           planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100));

           var activeType = blockTypeFactory.CreateNewBlockType(mesh);
           placeTool.SetBlockType(activeType);
           


           game.GameLoopEvent += delegate
           {
               playerController.Update();


               HUDRenderer.SetBlockType(activeType);

               HUDRenderer.Update();
               placeTool.Update();


               renderer.Draw();
           };

           game.Run();
       }

       [Test]
       public void TestMultipleBlockTypes()
       {
           var game = new DX11Game();
           game.InitDirectX();

           var renderer = new DeferredRenderer(game);
           var blockFactory = new BlockFactory(renderer);
           var blockTypeFactory = new BlockTypeFactory();

           var HUDRenderer = new HUDRenderer(game, renderer);
           var placeTool = new PlaceTool(game, renderer, blockFactory);


           var playerController = new PlayerController(game, blockFactory);


           var meshConverter = new OBJToRAMMeshConverter(new RAMTextureFactory());
           var brickMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                        "\\BasicTestBlock.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                        "\\BasicTestBlock.mtl");
           var woodMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\WoodPlankBlock").FullName +
                                        "\\WoodPlankBlock.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\WoodPlankBlock").FullName +
                                        "\\WoodPlankBlock.mtl");

           blockTypeFactory.CreateNewBlockType(brickMesh);
           blockTypeFactory.CreateNewBlockType(woodMesh);

           var planeMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                        "\\Plane.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                        "\\Plane.mtl");

           DirectionalLight light = renderer.CreateDirectionalLight();
           light.ShadowsEnabled = true;
           light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));

           var planeEl = renderer.CreateMeshElement(planeMesh);
           planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100));

           BlockType activeType = null;




           game.GameLoopEvent += delegate
           {
               playerController.Update();
               if (game.Keyboard.IsKeyPressed(Key.NumberPad0))
                   activeType = null;
               if (game.Keyboard.IsKeyPressed(Key.NumberPad1))
                   activeType = blockTypeFactory.TypeList[0];
               if (game.Keyboard.IsKeyPressed(Key.NumberPad2))
                   activeType = blockTypeFactory.TypeList[1];


               HUDRenderer.SetBlockType(activeType);
               placeTool.SetBlockType(activeType);

               HUDRenderer.Update();
               placeTool.Update();


               renderer.Draw();
           };

           game.Run();
       }

    }
}
