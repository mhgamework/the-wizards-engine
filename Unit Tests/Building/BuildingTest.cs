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
            HUDRenderer.SetBlockType(factory.CreateNewBlockType(mesh, new BlockLayout()));



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
            var blockPlaceLogic = new BlockPlaceLogic(blockTypeFactory, blockFactory);

            var HUDRenderer = new HUDRenderer(game, renderer);
            var placeTool = new PlaceTool(game, renderer, blockFactory, blockPlaceLogic);


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

            var activeType = blockTypeFactory.CreateNewBlockType(mesh, new BlockLayout());
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
            var blockPlaceLogic = new BlockPlaceLogic(blockTypeFactory, blockFactory);

            var HUDRenderer = new HUDRenderer(game, renderer);
            var placeTool = new PlaceTool(game, renderer, blockFactory, blockPlaceLogic);


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

            var activeType = blockTypeFactory.CreateNewBlockType(mesh, new BlockLayout());
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
            var blockPlaceLogic = new BlockPlaceLogic(blockTypeFactory, blockFactory);


            var HUDRenderer = new HUDRenderer(game, renderer);
            var placeTool = new PlaceTool(game, renderer, blockFactory, blockPlaceLogic);


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

            blockTypeFactory.CreateNewBlockType(brickMesh, new BlockLayout());
            blockTypeFactory.CreateNewBlockType(woodMesh, new BlockLayout());

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

        [Test]
        public void TestLoadWallBlocks()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var renderer = new DeferredRenderer(game);
            var blockFactory = new BlockFactory(renderer);
            var blockTypeFactory = new BlockTypeFactory();
            var blockPlaceLogic = new BlockPlaceLogic(blockTypeFactory, blockFactory);

            var meshConverter = new OBJToRAMMeshConverter(new RAMTextureFactory());
            var planeMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.mtl");

            loadBasicWalls(meshConverter, blockTypeFactory);

            var HUDRenderer = new HUDRenderer(game, renderer);
            var placeTool = new PlaceTool(game, renderer, blockFactory, blockPlaceLogic);


            var playerController = new PlayerController(game, blockFactory);

            BlockType activeType = blockTypeFactory.TypeList[0];


            DirectionalLight light = renderer.CreateDirectionalLight();
            light.ShadowsEnabled = true;
            light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));

            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100));


            game.GameLoopEvent += delegate
            {
                playerController.Update();

                if (game.Keyboard.IsKeyPressed(Key.NumberPad0))
                    activeType = null;
                if (game.Keyboard.IsKeyPressed(Key.NumberPad1))
                    activeType = blockTypeFactory.TypeList[0];
                if (game.Keyboard.IsKeyPressed(Key.NumberPad2))
                    activeType = blockTypeFactory.TypeList[1];
                if (game.Keyboard.IsKeyPressed(Key.NumberPad3))
                    activeType = blockTypeFactory.TypeList[2];
                if (game.Keyboard.IsKeyPressed(Key.NumberPad4))
                    activeType = blockTypeFactory.TypeList[3];
                if (game.Keyboard.IsKeyPressed(Key.NumberPad5))
                    activeType = blockTypeFactory.TypeList[4];

                HUDRenderer.SetBlockType(activeType);
                placeTool.SetBlockType(activeType);

                HUDRenderer.Update();
                placeTool.Update();


                renderer.Draw();
            };

            game.Run();
        }

        [Test]
        public void TestBlockPlaceLogic()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var renderer = new DeferredRenderer(game);
            var blockFactory = new BlockFactory(renderer);
            var blockTypeFactory = new BlockTypeFactory();
            var blockPlaceLogic = new BlockPlaceLogic(blockTypeFactory, blockFactory);

            var HUDRenderer = new HUDRenderer(game, renderer);
            var placeTool = new PlaceTool(game, renderer, blockFactory, blockPlaceLogic);


            var playerController = new PlayerController(game, blockFactory);


            var meshConverter = new OBJToRAMMeshConverter(new RAMTextureFactory());
            var halfWallMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\HalfWall").FullName +
                                         "\\HalfWall.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\HalfWall").FullName +
                                         "\\HalfWall.mtl");

            var blockMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                        "\\BasicTestBlock.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                        "\\BasicTestBlock.mtl");

            var planeMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.mtl");

            var partZh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partMinZ = new MeshCoreData.Part
                           {
                               MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                               MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                               ObjectMatrix = Matrix.RotationY((float)Math.PI).xna()
                           };
            var partMinZh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partX = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * 0.5f).xna()
            };
            var partXh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * 0.5f).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partMinX = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * -0.5f).xna()
            };
            var partMinXh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * -0.5f).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };


            Point3[] basicBlockLayoutList = { new Point3(0, 0, 0) };
            var basicBlockLayout = new BlockLayout(basicBlockLayoutList);

            var wallStraight = new RAMMesh();
            wallStraight.GetCoreData().Parts.Add(halfWallMesh.GetCoreData().Parts[0]);
            wallStraight.GetCoreData().Parts.Add(partZh);
            wallStraight.GetCoreData().Parts.Add(partMinZ);
            wallStraight.GetCoreData().Parts.Add(partMinZh);
            Point3[] straightLayoutList = { new Point3(-1, -1, 0), new Point3(-1, 0, 0), new Point3(-1, 1, 0), 
                                            new Point3(0, -1, 0), new Point3(0, 1, 0),
                                            new Point3(1, -1, 0), new Point3(1, 0, 0), new Point3(1, 1, 0),
                                            new Point3(0,0,0) };
            var straightLayout = new BlockLayout(straightLayoutList);

            var wallBend = new RAMMesh();
            wallBend.GetCoreData().Parts.Add(halfWallMesh.GetCoreData().Parts[0]);
            wallBend.GetCoreData().Parts.Add(partZh);
            wallBend.GetCoreData().Parts.Add(partX);
            wallBend.GetCoreData().Parts.Add(partXh);
            Point3[] bendLayoutList = { new Point3(-1, -1, 0), new Point3(-1, 0, 0), new Point3(-1, 1, 0), 
                                            new Point3(0, -1, 0), new Point3(0, 1, 0),
                                            new Point3(0, -1, 1), new Point3(0, 0, 1), new Point3(0, 1, 1)};
            var bendLayout = new BlockLayout(bendLayoutList);

            var wallT = new RAMMesh();
            wallT.GetCoreData().Parts.Add(halfWallMesh.GetCoreData().Parts[0]);
            wallT.GetCoreData().Parts.Add(partZh);
            wallT.GetCoreData().Parts.Add(partX);
            wallT.GetCoreData().Parts.Add(partXh);
            wallT.GetCoreData().Parts.Add(partMinZ);
            wallT.GetCoreData().Parts.Add(partMinZh);
            Point3[] tLayoutList = { new Point3(-1, -1, 0), new Point3(-1, 0, 0), new Point3(-1, 1, 0), 
                                            new Point3(0, -1, 0), new Point3(0, 1, 0),
                                            new Point3(0, -1, 1), new Point3(0, 0, 1), new Point3(0, 1, 1),
                                            new Point3(1, -1, 0), new Point3(1, 0, 0), new Point3(1, 1, 0), 
                                            new Point3(0,0,0)};
            var tLayout = new BlockLayout(tLayoutList);

            var wallCross = new RAMMesh();
            wallCross.GetCoreData().Parts.Add(halfWallMesh.GetCoreData().Parts[0]);
            wallCross.GetCoreData().Parts.Add(partZh);
            wallCross.GetCoreData().Parts.Add(partX);
            wallCross.GetCoreData().Parts.Add(partXh);
            wallCross.GetCoreData().Parts.Add(partMinZ);
            wallCross.GetCoreData().Parts.Add(partMinZh);
            wallCross.GetCoreData().Parts.Add(partMinX);
            wallCross.GetCoreData().Parts.Add(partMinXh);
            Point3[] crossLayoutList = { new Point3(-1, -1, 0), new Point3(-1, 0, 0), new Point3(-1, 1, 0), 
                                            new Point3(0, -1, 0), new Point3(0, 1, 0),
                                            new Point3(0, -1, 1), new Point3(0, 0, 1), new Point3(0, 1, 1),
                                            new Point3(0, -1, -1), new Point3(0, 0, -1), new Point3(0, 1, -1),
                                            new Point3(1, -1, 0), new Point3(1, 0, 0), new Point3(1, 1, 0),
                                            new Point3(0,0,0)};
            var crossLayout = new BlockLayout(crossLayoutList);


            blockTypeFactory.CreateNewBlockType(blockMesh, basicBlockLayout);
            blockTypeFactory.CreateNewBlockType(wallStraight, straightLayout);
            blockTypeFactory.CreateNewBlockType(wallBend, bendLayout);
            blockTypeFactory.CreateNewBlockType(wallT, tLayout);
            blockTypeFactory.CreateNewBlockType(wallCross, crossLayout);

            BlockType activeType = blockTypeFactory.TypeList[0];


            DirectionalLight light = renderer.CreateDirectionalLight();
            light.ShadowsEnabled = true;
            light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));

            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100));





            game.GameLoopEvent += delegate
            {
                playerController.Update();

                HUDRenderer.SetBlockType(activeType);
                placeTool.SetBlockType(activeType);

                HUDRenderer.Update();
                placeTool.Update();


                renderer.Draw();
            };

            game.Run();
        }

        [Test]
        public void TestBlockLogicSimple()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var renderer = new DeferredRenderer(game);
            var blockFactory = new BlockFactory(renderer);
            var blockTypeFactory = new BlockTypeFactory();
            var blockPlaceLogic = new BlockPlaceLogic(blockTypeFactory, blockFactory);

            var meshConverter = new OBJToRAMMeshConverter(new RAMTextureFactory());
            var planeMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.mtl");

            loadBasicWalls(meshConverter, blockTypeFactory);

           // var placeTool = new PlaceTool(game, renderer, blockFactory, blockPlaceLogic);


            var playerController = new PlayerController(game, blockFactory);

            BlockType activeType = blockTypeFactory.TypeList[0];


            DirectionalLight light = renderer.CreateDirectionalLight();
            light.ShadowsEnabled = true;
            light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));

            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100));

            Point3 pos1 = new Point3(0, 0, 0);
            BlockType blockType1 = blockTypeFactory.TypeList[1];
            blockFactory.CreateBlock(blockType1, pos1);
            
            Point3 pos2 = new Point3(1, 0, 0);
            BlockType blockType2 = blockTypeFactory.TypeList[2];
            //blockFactory.CreateBlock(blockType2, pos2);

            blockPlaceLogic.CalulateBlocks();

            /*
            Point3 pos3 = new Point3(1, 0, 2);
            BlockType blockType3 = blockTypeFactory.TypeList[3];
            blockFactory.CreateBlock(blockType3, pos3);
            */
            game.GameLoopEvent += delegate
            {
                playerController.Update();

                //placeTool.SetBlockType(activeType);

                //placeTool.Update();

                renderer.Draw();
            };

            game.Run();
        }


        private void loadBasicWalls(OBJToRAMMeshConverter meshConverter, BlockTypeFactory blockTypeFactory)
        {

            var halfWallMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\HalfWall").FullName +
                                         "\\HalfWall.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\HalfWall").FullName +
                                         "\\HalfWall.mtl");

            var blockMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                        "\\BasicTestBlock.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                        "\\BasicTestBlock.mtl");


            var partZh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partMinZ = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI).xna()
            };
            var partMinZh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partX = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * 0.5f).xna()
            };
            var partXh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * 0.5f).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partMinX = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * -0.5f).xna()
            };
            var partMinXh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * -0.5f).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };


            Point3[] basicBlockLayoutList = { new Point3(0, 0, 0) };
            var basicBlockLayout = new BlockLayout(basicBlockLayoutList);

            var wallStraight = new RAMMesh();
            wallStraight.GetCoreData().Parts.Add(halfWallMesh.GetCoreData().Parts[0]);
            wallStraight.GetCoreData().Parts.Add(partZh);
            wallStraight.GetCoreData().Parts.Add(partMinZ);
            wallStraight.GetCoreData().Parts.Add(partMinZh);
            Point3[] straightLayoutList = { new Point3(-1, -1, 0), new Point3(-1, 0, 0), new Point3(-1, 1, 0), 
                                            new Point3(0, -1, 0), new Point3(0, 1, 0),
                                            new Point3(1, -1, 0), new Point3(1, 0, 0), new Point3(1, 1, 0),
                                            new Point3(0,0,0) };
            var straightLayout = new BlockLayout(straightLayoutList);

            var wallBend = new RAMMesh();
            wallBend.GetCoreData().Parts.Add(halfWallMesh.GetCoreData().Parts[0]);
            wallBend.GetCoreData().Parts.Add(partZh);
            wallBend.GetCoreData().Parts.Add(partX);
            wallBend.GetCoreData().Parts.Add(partXh);
            Point3[] bendLayoutList = { new Point3(-1, -1, 0), new Point3(-1, 0, 0), new Point3(-1, 1, 0), 
                                            new Point3(0, -1, 0), new Point3(0, 1, 0),
                                            new Point3(0, -1, 1), new Point3(0, 0, 1), new Point3(0, 1, 1)};
            var bendLayout = new BlockLayout(bendLayoutList);

            var wallT = new RAMMesh();
            wallT.GetCoreData().Parts.Add(halfWallMesh.GetCoreData().Parts[0]);
            wallT.GetCoreData().Parts.Add(partZh);
            wallT.GetCoreData().Parts.Add(partX);
            wallT.GetCoreData().Parts.Add(partXh);
            wallT.GetCoreData().Parts.Add(partMinZ);
            wallT.GetCoreData().Parts.Add(partMinZh);
            Point3[] tLayoutList = { new Point3(-1, -1, 0), new Point3(-1, 0, 0), new Point3(-1, 1, 0), 
                                            new Point3(0, -1, 0), new Point3(0, 1, 0),
                                            new Point3(0, -1, 1), new Point3(0, 0, 1), new Point3(0, 1, 1),
                                            new Point3(1, -1, 0), new Point3(1, 0, 0), new Point3(1, 1, 0), 
                                            new Point3(0,0,0)};
            var tLayout = new BlockLayout(tLayoutList);

            var wallCross = new RAMMesh();
            wallCross.GetCoreData().Parts.Add(halfWallMesh.GetCoreData().Parts[0]);
            wallCross.GetCoreData().Parts.Add(partZh);
            wallCross.GetCoreData().Parts.Add(partX);
            wallCross.GetCoreData().Parts.Add(partXh);
            wallCross.GetCoreData().Parts.Add(partMinZ);
            wallCross.GetCoreData().Parts.Add(partMinZh);
            wallCross.GetCoreData().Parts.Add(partMinX);
            wallCross.GetCoreData().Parts.Add(partMinXh);
            Point3[] crossLayoutList = { new Point3(-1, -1, 0), new Point3(-1, 0, 0), new Point3(-1, 1, 0), 
                                            new Point3(0, -1, 0), new Point3(0, 1, 0),
                                            new Point3(0, -1, 1), new Point3(0, 0, 1), new Point3(0, 1, 1),
                                            new Point3(0, -1, -1), new Point3(0, 0, -1), new Point3(0, 1, -1),
                                            new Point3(1, -1, 0), new Point3(1, 0, 0), new Point3(1, 1, 0),
                                            new Point3(0,0,0)};
            var crossLayout = new BlockLayout(crossLayoutList);


            blockTypeFactory.CreateNewBlockType(blockMesh, basicBlockLayout);
            blockTypeFactory.CreateNewBlockType(wallStraight, straightLayout);
            blockTypeFactory.CreateNewBlockType(wallBend, bendLayout);
            blockTypeFactory.CreateNewBlockType(wallT, tLayout);
            blockTypeFactory.CreateNewBlockType(wallCross, crossLayout);
        }
    }
}
