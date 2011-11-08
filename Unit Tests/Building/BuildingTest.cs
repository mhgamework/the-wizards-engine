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

            var partMinZ = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.Translation(new Vector3(0, 0, 0)).xna()
            };
            var partMinZh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partZ = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI).xna()
            };
            var partZh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partMinX = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * 0.5f).xna()
            };
            var partMinXh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * 0.5f).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partX = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * -0.5f).xna()
            };
            var partXh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * -0.5f).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            
            var wallBend = new RAMMesh();
            wallBend.GetCoreData().Parts.Add(partZ);
            wallBend.GetCoreData().Parts.Add(partZh);
            wallBend.GetCoreData().Parts.Add(partX);
            wallBend.GetCoreData().Parts.Add(partXh);
            Point3[] bendLayoutList = {new Point3(0, 0, 0),
                                          new Point3(1, 0, 0), new Point3(0, 0, 0), new Point3(0, 0, 1)};
            var bendLayout = new BlockLayout(bendLayoutList);

            blockTypeFactory.AddRotatedBlocktypes(wallBend, bendLayout);

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
            blockFactory.CreateBlock(blockType1, pos2);

            blockPlaceLogic.CalulateBlocks();

            
            game.GameLoopEvent += delegate
            {
                playerController.Update();
                renderer.Draw();
            };

            game.Run();
        }

        /// <summary>
        /// Tests the blocklogic in the case of placing blocks on one plane only.
        /// User input possible. Blocklogic doesnt account blocks in different levels.
        /// Standard blocks placed on xyz = 101, 001, 100, 102, 201.
        /// </summary>
        [Test]
        public void TestBlockLogic2D()
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

            loadBasicWalls2D(meshConverter, blockTypeFactory);

            var playerController = new PlayerController(game, blockFactory);
            BlockType activeType = blockTypeFactory.TypeList[0];
            PlaceTool placeTool = new PlaceTool(game, renderer, blockFactory, blockPlaceLogic);
            placeTool.SetBlockType(activeType);

            DirectionalLight light = renderer.CreateDirectionalLight();
            light.ShadowsEnabled = true;
            light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));
            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100));


            Point3 pos1 = new Point3(1, 0, 1);
            Point3 pos2 = new Point3(1, 0, 0);
            Point3 pos3 = new Point3(0, 0, 1);
            Point3 pos4 = new Point3(1, 0, 2);
            Point3 pos5 = new Point3(2, 0, 1);
            BlockType blockType1 = blockTypeFactory.TypeList[1];
           
            //testinput
            blockFactory.CreateBlock(blockType1, pos1);
            blockFactory.CreateBlock(blockType1, pos2);
            blockFactory.CreateBlock(blockType1, pos3);
            blockFactory.CreateBlock(blockType1, pos4);
            blockFactory.CreateBlock(blockType1, pos5);

            blockPlaceLogic.CalulateBlocks();


            game.GameLoopEvent += delegate
            {
                playerController.Update();
                placeTool.SetBlockType(activeType);
                placeTool.Update();

                renderer.Draw();
            };

            game.Run();
        }

        /// <summary>
        /// Loads the basic walls (block, straight, bend, T, cross) with 2D-layouts
        /// </summary>
        /// <param name="meshConverter"></param>
        /// <param name="blockTypeFactory"></param>
        private void loadBasicWalls2D(OBJToRAMMeshConverter meshConverter, BlockTypeFactory blockTypeFactory)
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

            var partMinZ = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.Translation(new Vector3(0, 0, 0)).xna()
            };
            var partMinZh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partZ = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI).xna()
            };
            var partZh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partMinX = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * 0.5f).xna()
            };
            var partMinXh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * 0.5f).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partX = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * -0.5f).xna()
            };
            var partXh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * -0.5f).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };


            Point3[] basicBlockLayoutList = { new Point3(0, 0, 0)};
            var basicBlockLayout = new BlockLayout(basicBlockLayoutList);

            var wallStraight = new RAMMesh();
            wallStraight.GetCoreData().Parts.Add(partZ);
            wallStraight.GetCoreData().Parts.Add(partZh);
            wallStraight.GetCoreData().Parts.Add(partMinZ);
            wallStraight.GetCoreData().Parts.Add(partMinZh);
            Point3[] straightLayoutList = { new Point3(-1,0,1), new Point3(-1,0,-1),
                                            new Point3(0, 0, -1), new Point3(0, 0, 0), new Point3(0, 0, 1),
                                            new Point3(1,0,1), new Point3(1,0,-1)};

            var straightLayout = new BlockLayout(straightLayoutList);

            var wallBend = new RAMMesh();
            wallBend.GetCoreData().Parts.Add(partZ);
            wallBend.GetCoreData().Parts.Add(partZh);
            wallBend.GetCoreData().Parts.Add(partX);
            wallBend.GetCoreData().Parts.Add(partXh);
            Point3[] bendLayoutList = { new Point3(-1,0,1), new Point3(-1,0,-1), 
                                        new Point3(0, 0, 1), new Point3(0, 0, 0), 
                                        new Point3(1, 0, 1), new Point3(1,0,0), new Point3(1,0,-1)};
            var bendLayout = new BlockLayout(bendLayoutList);

            var wallT = new RAMMesh();
            wallT.GetCoreData().Parts.Add(partZ);
            wallT.GetCoreData().Parts.Add(partZh);
            wallT.GetCoreData().Parts.Add(partX);
            wallT.GetCoreData().Parts.Add(partXh);
            wallT.GetCoreData().Parts.Add(partMinZ);
            wallT.GetCoreData().Parts.Add(partMinZh);
            Point3[] tLayoutList = { new Point3(-1, 0, 1), new Point3(-1,0,-1), 
                                     new Point3(0, 0, 1), new Point3(0, 0, 0), new Point3(0, 0, -1), 
                                     new Point3(1, 0,1), new Point3(1,0,0), new Point3(1,0,-1)};
            var tLayout = new BlockLayout(tLayoutList);

            var wallCross = new RAMMesh();
            wallCross.GetCoreData().Parts.Add(partZ);
            wallCross.GetCoreData().Parts.Add(partZh);
            wallCross.GetCoreData().Parts.Add(partX);
            wallCross.GetCoreData().Parts.Add(partXh);
            wallCross.GetCoreData().Parts.Add(partMinZ);
            wallCross.GetCoreData().Parts.Add(partMinZh);
            wallCross.GetCoreData().Parts.Add(partMinX);
            wallCross.GetCoreData().Parts.Add(partMinXh);
            Point3[] crossLayoutList = { new Point3(-1, 0, 1), new Point3(-1, 0, 0), new Point3(-1, 0, -1), 
                                         new Point3(0, 0, 1), new Point3(0, 0, 0), new Point3(0, 0, -1), 
                                         new Point3(1, 0, 1), new Point3(1, 0, 0), new Point3(1, 0, -1)};
            var crossLayout = new BlockLayout(crossLayoutList);


            blockTypeFactory.AddRotatedBlocktypes(blockMesh, basicBlockLayout);
            blockTypeFactory.AddRotatedBlocktypes(wallStraight, straightLayout);
            blockTypeFactory.AddRotatedBlocktypes(wallBend, bendLayout);
            blockTypeFactory.AddRotatedBlocktypes(wallT, tLayout);
            blockTypeFactory.AddRotatedBlocktypes(wallCross, crossLayout);
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

            var partMinZ = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.Translation(new Vector3(0, 0, 0)).xna()
            };
            var partMinZh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partZ = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI).xna()
            };
            var partZh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partMinX = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * 0.5f).xna()
            };
            var partMinXh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * 0.5f).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };
            var partX = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * -0.5f).xna()
            };
            var partXh = new MeshCoreData.Part
            {
                MeshPart = halfWallMesh.GetCoreData().Parts[0].MeshPart,
                MeshMaterial = halfWallMesh.GetCoreData().Parts[0].MeshMaterial,
                ObjectMatrix = Matrix.RotationY((float)Math.PI * -0.5f).xna() * Matrix.Translation(new Vector3(0, 0.5f, 0)).xna()
            };


            Point3[] basicBlockLayoutList = { new Point3(0, 0, 0), 
                                              new Point3(-1, -1, -1), new Point3(-1, -1, 1), new Point3(1, -1, -1), new Point3(1, -1, 1),
                                              new Point3(-1, 1, -1), new Point3(-1, 1, 1), new Point3(1, 1, -1), new Point3(1, 1, 1)};
            var basicBlockLayout = new BlockLayout(basicBlockLayoutList);

            var wallStraight = new RAMMesh();
            wallStraight.GetCoreData().Parts.Add(partZ);
            wallStraight.GetCoreData().Parts.Add(partZh);
            wallStraight.GetCoreData().Parts.Add(partMinZ);
            wallStraight.GetCoreData().Parts.Add(partMinZh);
            Point3[] straightLayoutList = { new Point3(0, 0, 0), 
                                            new Point3(-1, -1, -1), new Point3(-1, -1, 1), new Point3(1, -1, -1), new Point3(1, -1, 1),
                                            new Point3(-1, 1, -1), new Point3(-1, 1, 1), new Point3(1, 1, -1), new Point3(1, 1, 1),
                                            new Point3(0, -1, -1), new Point3(0, -1, 0), new Point3(0, -1, 1),
                                            new Point3(0, 0, -1), new Point3(0, 0, 0), new Point3(0, 0, 1),
                                            new Point3(0, 1, -1), new Point3(0, 1, 0), new Point3(0, 1, 1)};

            var straightLayout = new BlockLayout(straightLayoutList);

            var wallBend = new RAMMesh();
            wallBend.GetCoreData().Parts.Add(partZ);
            wallBend.GetCoreData().Parts.Add(partZh);
            wallBend.GetCoreData().Parts.Add(partX);
            wallBend.GetCoreData().Parts.Add(partXh);
            Point3[] bendLayoutList = { new Point3(0, 0, 0), 
                                              new Point3(-1, -1, -1), new Point3(-1, -1, 1), new Point3(1, -1, -1), new Point3(1, -1, 1),
                                              new Point3(-1, 1, -1), new Point3(-1, 1, 1), new Point3(1, 1, -1), new Point3(1, 1, 1),
                                              new Point3(1, -1, 0), new Point3(0, -1, 0), new Point3(0, -1, 1),
                                              new Point3(1, 0, 0), new Point3(0, 0, 0), new Point3(0, 0, 1),
                                              new Point3(1, 1, 0), new Point3(0, 1, 0), new Point3(0, 1, 1)};
            var bendLayout = new BlockLayout(bendLayoutList);

            var wallT = new RAMMesh();
            wallT.GetCoreData().Parts.Add(partZ);
            wallT.GetCoreData().Parts.Add(partZh);
            wallT.GetCoreData().Parts.Add(partX);
            wallT.GetCoreData().Parts.Add(partXh);
            wallT.GetCoreData().Parts.Add(partMinZ);
            wallT.GetCoreData().Parts.Add(partMinZh);
            Point3[] tLayoutList = { new Point3(0, 0, 0), 
                                              new Point3(-1, -1, -1), new Point3(-1, -1, 1), new Point3(1, -1, -1), new Point3(1, -1, 1),
                                              new Point3(-1, 1, -1), new Point3(-1, 1, 1), new Point3(1, 1, -1), new Point3(1, 1, 1),
                                              new Point3(1, -1, 0), new Point3(0, -1, 0), new Point3(0, -1, 1), new Point3(0, -1,-1),
                                              new Point3(1, 0, 0), new Point3(0, 0, 0), new Point3(0, 0, 1), new Point3(0, 0,-1),
                                              new Point3(1, 1, 0), new Point3(0, 1, 0), new Point3(0, 1, 1), new Point3(0, 1,-1)};
            var tLayout = new BlockLayout(tLayoutList);

            var wallCross = new RAMMesh();
            wallCross.GetCoreData().Parts.Add(partZ);
            wallCross.GetCoreData().Parts.Add(partZh);
            wallCross.GetCoreData().Parts.Add(partX);
            wallCross.GetCoreData().Parts.Add(partXh);
            wallCross.GetCoreData().Parts.Add(partMinZ);
            wallCross.GetCoreData().Parts.Add(partMinZh);
            wallCross.GetCoreData().Parts.Add(partMinX);
            wallCross.GetCoreData().Parts.Add(partMinXh);
            Point3[] crossLayoutList = { new Point3(0, 0, 0), 
                                              new Point3(-1, -1, -1), new Point3(-1, -1, 1), new Point3(1, -1, -1), new Point3(1, -1, 1),
                                              new Point3(-1, 1, -1), new Point3(-1, 1, 1), new Point3(1, 1, -1), new Point3(1, 1, 1),
                                              new Point3(1, -1, 0), new Point3(0, -1, 0), new Point3(0, -1, 1), new Point3(0, -1,-1), new Point3(-1, -1, 0),
                                              new Point3(1, 0, 0), new Point3(0, 0, 0), new Point3(0, 0, 1), new Point3(0, 0,-1), new Point3(-1, 0, 0),
                                              new Point3(1, 1, 0), new Point3(0, 1, 0), new Point3(0, 1, 1), new Point3(0, 1,-1), new Point3(-1, 1, 0)};
            var crossLayout = new BlockLayout(crossLayoutList);


            blockTypeFactory.AddRotatedBlocktypes(blockMesh, basicBlockLayout);
            blockTypeFactory.AddRotatedBlocktypes(wallStraight, straightLayout);
            blockTypeFactory.AddRotatedBlocktypes(wallBend, bendLayout);
            blockTypeFactory.AddRotatedBlocktypes(wallT, tLayout);
            blockTypeFactory.AddRotatedBlocktypes(wallCross, crossLayout);
        }
   
        /// <summary>
        /// Tests if a DynamicBlock can be created and if its slots can be filled in properly.
        /// Test succeeds if:
        /// - a cross is displayed at xyz = 100 with floors
        /// - a T at xyz = -10-1 (with a missing -z wall) with floors
        /// - a block with floors and skew walls at xyz = 102
        /// </summary>
        [Test]
        public void TestSetDynamicBlockSlots()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var renderer = new DeferredRenderer(game);
            var meshConverter = new OBJToRAMMeshConverter(new RAMTextureFactory());
            var planeMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.mtl");
            DirectionalLight light = renderer.CreateDirectionalLight();
            light.ShadowsEnabled = true;
            light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));
            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100)) * Matrix.Translation(new Vector3(0, -0.5f, 0));

            var basicStraight = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicStraight").FullName +
                                         "\\BasicStraight.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicStraight").FullName +
                                         "\\BasicStraight.mtl");
            var basicPillar = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicPillar").FullName +
                                        "\\BasicPillar.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicPillar").FullName +
                                        "\\BasicPillar.mtl");
            var basicFloor = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicFloor").FullName +
                                        "\\BasicFloor.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicFloor").FullName +
                                        "\\BasicFloor.mtl");
            var basicSkew = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicSkew").FullName +
                                        "\\BasicSkew.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicSkew").FullName +
                                        "\\BasicSkew.mtl");

            
            var dynBlock1 = new DynamicBlock(new Point3(1, 0, 0), renderer);
            var dynBlock2 = new DynamicBlock(new Point3(-1, 0, -1), renderer);
            var dynBlock3 = new DynamicBlock(new Point3(1, 0, 2), renderer);

            var straightUnit = new BuildUnit(basicStraight);
            var pillarUnit = new BuildUnit(basicPillar);
            var floorUnit = new BuildUnit(basicFloor);
            var skewUnit = new BuildUnit(basicSkew);
            
            dynBlock1.SetStraight(DynamicBlockDirection.X, 0,straightUnit);
            dynBlock1.SetStraight(DynamicBlockDirection.X, 1, straightUnit);
            dynBlock1.SetStraight(DynamicBlockDirection.MinX, 0, straightUnit);
            dynBlock1.SetStraight(DynamicBlockDirection.MinX, 1, straightUnit);
            dynBlock1.SetStraight(DynamicBlockDirection.Z, 0, straightUnit);
            dynBlock1.SetStraight(DynamicBlockDirection.Z, 1, straightUnit);
            dynBlock1.SetStraight(DynamicBlockDirection.MinZ, 0, straightUnit);
            dynBlock1.SetStraight(DynamicBlockDirection.MinZ, 1, straightUnit);
            dynBlock1.SetPillar(pillarUnit);
            dynBlock1.SetFloor(DynamicBlockDirection.MinXMinZ, 0, floorUnit);
            dynBlock1.SetFloor(DynamicBlockDirection.MinXMinZ, 1, floorUnit);
            dynBlock1.SetFloor(DynamicBlockDirection.XMinZ, 0, floorUnit);
            dynBlock1.SetFloor(DynamicBlockDirection.XMinZ, 1, floorUnit);
            dynBlock1.SetFloor(DynamicBlockDirection.MinXZ, 0, floorUnit);
            dynBlock1.SetFloor(DynamicBlockDirection.MinXZ, 1, floorUnit);
            dynBlock1.SetFloor(DynamicBlockDirection.XZ, 0, floorUnit);
            dynBlock1.SetFloor(DynamicBlockDirection.XZ, 1, floorUnit);

            dynBlock2.SetStraight(DynamicBlockDirection.X, 0, straightUnit);
            dynBlock2.SetStraight(DynamicBlockDirection.X, 1, straightUnit);
            dynBlock2.SetStraight(DynamicBlockDirection.MinX, 0, straightUnit);
            dynBlock2.SetStraight(DynamicBlockDirection.MinX, 1, straightUnit);
            dynBlock2.SetStraight(DynamicBlockDirection.Z, 0, straightUnit);
            dynBlock2.SetStraight(DynamicBlockDirection.Z, 1, straightUnit);
            dynBlock2.SetPillar(pillarUnit);
            dynBlock2.SetFloor(DynamicBlockDirection.MinXMinZ, 0, floorUnit);
            dynBlock2.SetFloor(DynamicBlockDirection.MinXMinZ, 1, floorUnit);
            dynBlock2.SetFloor(DynamicBlockDirection.XMinZ, 0, floorUnit);
            dynBlock2.SetFloor(DynamicBlockDirection.XMinZ, 1, floorUnit);
            dynBlock2.SetFloor(DynamicBlockDirection.MinXZ, 0, floorUnit);
            dynBlock2.SetFloor(DynamicBlockDirection.MinXZ, 1, floorUnit);
            dynBlock2.SetFloor(DynamicBlockDirection.XZ, 0, floorUnit);
            dynBlock2.SetFloor(DynamicBlockDirection.XZ, 1, floorUnit);

            dynBlock3.SetSkew(DynamicBlockDirection.MinXMinZ, 0, skewUnit);
            dynBlock3.SetSkew(DynamicBlockDirection.MinXMinZ, 1, skewUnit);
            dynBlock3.SetSkew(DynamicBlockDirection.XZ, 0, skewUnit);
            dynBlock3.SetSkew(DynamicBlockDirection.XZ, 1, skewUnit);
            dynBlock3.SetSkew(DynamicBlockDirection.MinXZ, 0, skewUnit);
            dynBlock3.SetSkew(DynamicBlockDirection.MinXZ, 1, skewUnit);
            dynBlock3.SetSkew(DynamicBlockDirection.XMinZ, 0, skewUnit);
            dynBlock3.SetSkew(DynamicBlockDirection.XMinZ, 1, skewUnit);
            dynBlock3.SetFloor(DynamicBlockDirection.MinXMinZ, 0, floorUnit);
            dynBlock3.SetFloor(DynamicBlockDirection.MinXMinZ, 1, floorUnit);
            dynBlock3.SetFloor(DynamicBlockDirection.XMinZ, 0, floorUnit);
            dynBlock3.SetFloor(DynamicBlockDirection.XMinZ, 1, floorUnit);
            dynBlock3.SetFloor(DynamicBlockDirection.MinXZ, 0, floorUnit);
            dynBlock3.SetFloor(DynamicBlockDirection.MinXZ, 1, floorUnit);
            dynBlock3.SetFloor(DynamicBlockDirection.XZ, 0, floorUnit);
            dynBlock3.SetFloor(DynamicBlockDirection.XZ, 1, floorUnit);
            
            game.GameLoopEvent += delegate
            {
                game.SpectaterCamera.CameraPosition =
                                             new SlimDX.Vector3(game.SpectaterCamera.CameraPosition.X, 2,
                                                                game.SpectaterCamera.CameraPosition.Z);
               renderer.Draw();
            };

            game.Run();
        }
        
        /// <summary>
        /// Tests if straight walls can be placed.
        /// Test succeeded if:
        /// - only a pillar is visible at xyz = 001
        /// - a series of connencted walls is visible near the x-axis
        /// - some walls are correctly removed when 'R' is pressed
        /// - the same walls are added again removed when 'E' is pressed
        /// </summary>
        [Test]
        public void TestPlaceStraightWallsDynamic()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var renderer = new DeferredRenderer(game);
            var meshConverter = new OBJToRAMMeshConverter(new RAMTextureFactory());
            var planeMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.mtl");
            DirectionalLight light = renderer.CreateDirectionalLight();
            light.ShadowsEnabled = true;
            light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));
            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100)) * Matrix.Translation(new Vector3(0, -0.5f, 0));

            var basicStraightMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicStraight").FullName +
                                         "\\BasicStraight.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicStraight").FullName +
                                         "\\BasicStraight.mtl");
            var basicPillarMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicPillar").FullName +
                                        "\\BasicPillar.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicPillar").FullName +
                                        "\\BasicPillar.mtl");

            var basicStraight = new BuildUnit(basicStraightMesh);
            var basicPillar = new BuildUnit(basicPillarMesh);
            var straightWallType1 = new WallType();

            straightWallType1.StraightUnit = basicStraight;
            straightWallType1.PillarUnit = basicPillar;

            var dynBlockFactory = new DynamicBlockFactory(renderer);
            var resolver = new DynamicBlockResolver(dynBlockFactory);
            var wallPlacer = new StraightWallPlacer(dynBlockFactory, resolver);

            wallPlacer.PlaceStraightWall(new Point3(1, 0, 0), straightWallType1);
            wallPlacer.PlaceStraightWall(new Point3(2, 0, 0), straightWallType1);
            wallPlacer.PlaceStraightWall(new Point3(2, 0, -1), straightWallType1);
            wallPlacer.PlaceStraightWall(new Point3(3, 0, -1), straightWallType1);
            wallPlacer.PlaceStraightWall(new Point3(3, 0, 0), straightWallType1);
            wallPlacer.PlaceStraightWall(new Point3(0, 0, 1), straightWallType1);

            

            game.GameLoopEvent += delegate
            {
                game.SpectaterCamera.CameraPosition =
                                             new SlimDX.Vector3(game.SpectaterCamera.CameraPosition.X, 2,
                                                                game.SpectaterCamera.CameraPosition.Z);
                if(game.Keyboard.IsKeyPressed(Key.R))
                    wallPlacer.RemoveStraightWall(new Point3(3, 0, 0));
                if (game.Keyboard.IsKeyPressed(Key.E))
                    wallPlacer.PlaceStraightWall(new Point3(3, 0, 0), straightWallType1);

                renderer.Draw();
            };

            game.Run();
        }

        /// <summary>
        /// Tests if skew walls and straight walls can be placed.
        /// Test succeeded if:
        /// - you see some wall construction with straight and skew walls + 1 pillar at xyz = 001
        /// - pressing R results in removing some of the walls
        /// - predding E results in replacing those walls
        /// </summary>
        [Test]
        public void TestPlaceSkewWallsDynamic()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var renderer = new DeferredRenderer(game);
            var meshConverter = new OBJToRAMMeshConverter(new RAMTextureFactory());
            var planeMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.mtl");
            DirectionalLight light = renderer.CreateDirectionalLight();
            light.ShadowsEnabled = true;
            light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));
            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100)) * Matrix.Translation(new Vector3(0, -0.5f, 0));

            var basicStraightMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicStraight").FullName +
                                         "\\BasicStraight.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicStraight").FullName +
                                         "\\BasicStraight.mtl");
            var basicPillarMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicPillar").FullName +
                                        "\\BasicPillar.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicPillar").FullName +
                                        "\\BasicPillar.mtl");
            var basicSkewMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicSkew").FullName +
                                        "\\BasicSkew.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicSkew").FullName +
                                        "\\BasicSkew.mtl");

            var basicStraight = new BuildUnit(basicStraightMesh);
            var basicPillar = new BuildUnit(basicPillarMesh);
            var basicSkew = new BuildUnit(basicSkewMesh);

            var wallType = new WallType();
            wallType.StraightUnit = basicStraight;
            wallType.PillarUnit = basicPillar;
            wallType.SkewUnit = basicSkew;

            var dynBlockFactory = new DynamicBlockFactory(renderer);
            var resolver = new DynamicBlockResolver(dynBlockFactory);

            var straightWallPlacer = new StraightWallPlacer(dynBlockFactory, resolver);
            var skewWallPlacer = new SkewWallPlacer(dynBlockFactory, resolver);

            straightWallPlacer.PlaceStraightWall(new Point3(1, 0, 0), wallType);

            
            straightWallPlacer.PlaceStraightWall(new Point3(5, 0, 1), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(6, 0, 2), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(7, 0, 3), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(6, 0, 4), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(5, 0, 3), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(5, 0, 5), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(6, 0, 5), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(4, 0, 4), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(3, 0, 4), wallType);
            
            
            skewWallPlacer.PlaceSkewWall(new Point3(6, 0, 1), wallType);
            skewWallPlacer.PlaceSkewWall(new Point3(7, 0, 2), wallType);
            skewWallPlacer.PlaceSkewWall(new Point3(7, 0, 4), wallType);
            skewWallPlacer.PlaceSkewWall(new Point3(5, 0, 4), wallType);
            skewWallPlacer.PlaceSkewWall(new Point3(2, 0, 4), wallType);
            skewWallPlacer.PlaceSkewWall(new Point3(2, 0, 5), wallType);
            

            game.GameLoopEvent += delegate
            {
                game.SpectaterCamera.CameraPosition =
                                             new SlimDX.Vector3(game.SpectaterCamera.CameraPosition.X, 2,
                                                                game.SpectaterCamera.CameraPosition.Z);

                if (game.Keyboard.IsKeyPressed(Key.R))
                {
                    straightWallPlacer.RemoveStraightWall(new Point3(6, 0, 2));
                    straightWallPlacer.RemoveStraightWall(new Point3(5, 0, 3));
                    straightWallPlacer.RemoveStraightWall(new Point3(6, 0, 5));
                    skewWallPlacer.RemoveSkewWall(new Point3(7, 0, 2));
                    skewWallPlacer.RemoveSkewWall(new Point3(5, 0, 4));
                }
                if (game.Keyboard.IsKeyPressed(Key.E))
                {
                    straightWallPlacer.PlaceStraightWall(new Point3(6, 0, 2), wallType);
                    straightWallPlacer.PlaceStraightWall(new Point3(5, 0, 3), wallType);
                    straightWallPlacer.PlaceStraightWall(new Point3(6, 0, 5), wallType);
                    skewWallPlacer.PlaceSkewWall(new Point3(7, 0, 2), wallType);
                    skewWallPlacer.PlaceSkewWall(new Point3(5, 0, 4), wallType);
                }

                renderer.Draw();
            };

            game.Run();
        }

        /// <summary>
        /// TEST DOESNT WORK PROPERLY ANYMORE
        /// Tests if floors can be placed and adjacent blocks are altered correctly.
        /// Test succeeded if:
        /// - the walls from the skewWallsTest are visible
        /// - some floor-pieces are visible, they all touch a wall
        /// - no floor piece is added to xyz = 001
        /// - pressing A results in displaying a box around every dynamicBlock
        /// </summary>
        [Test]
        public void TestPlaceFloorsDynamic()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var renderer = new DeferredRenderer(game);
            var meshConverter = new OBJToRAMMeshConverter(new RAMTextureFactory());
            var planeMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.mtl");
            DirectionalLight light = renderer.CreateDirectionalLight();
            light.ShadowsEnabled = true;
            light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));
            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100)) * Matrix.Translation(new Vector3(0, -0.5f, 0));

            var basicStraightMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicStraight").FullName +
                                         "\\BasicStraight.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicStraight").FullName +
                                         "\\BasicStraight.mtl");
            var basicPillarMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicPillar").FullName +
                                        "\\BasicPillar.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicPillar").FullName +
                                        "\\BasicPillar.mtl");
            var basicSkewMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicSkew").FullName +
                                        "\\BasicSkew.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicSkew").FullName +
                                        "\\BasicSkew.mtl");
            var basicFloorMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicFloor").FullName +
                                        "\\BasicFloor.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicFloor").FullName +
                                        "\\BasicFloor.mtl");

            var basicStraight = new BuildUnit(basicStraightMesh);
            var basicPillar = new BuildUnit(basicPillarMesh);
            var basicSkew = new BuildUnit(basicSkewMesh);
            var basicFloor = new BuildUnit(basicFloorMesh);

            var wallType = new WallType();
            wallType.StraightUnit = basicStraight;
            wallType.PillarUnit = basicPillar;
            wallType.SkewUnit = basicSkew;

            var floorType = new FloorType();
            floorType.DefaultUnit = basicFloor;

            var dynBlockFactory = new DynamicBlockFactory(renderer);
            var resolver = new DynamicBlockResolver(dynBlockFactory);

            var straightWallPlacer = new StraightWallPlacer(dynBlockFactory, resolver);
            var skewWallPlacer = new SkewWallPlacer(dynBlockFactory, resolver);
            var floorPlacer = new FloorPlacer(dynBlockFactory, resolver);

            straightWallPlacer.PlaceStraightWall(new Point3(0, 0, 1), wallType);
            floorPlacer.PlaceFloor(new Point3(0, 0, 1), floorType); //there shouldnt be added a floor at this pos


            straightWallPlacer.PlaceStraightWall(new Point3(5, 0, 1), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(6, 0, 2), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(7, 0, 3), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(6, 0, 4), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(5, 0, 3), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(5, 0, 5), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(6, 0, 5), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(4, 0, 4), wallType);
            straightWallPlacer.PlaceStraightWall(new Point3(3, 0, 4), wallType);
            skewWallPlacer.PlaceSkewWall(new Point3(6, 0, 1), wallType);
            skewWallPlacer.PlaceSkewWall(new Point3(7, 0, 2), wallType);
            skewWallPlacer.PlaceSkewWall(new Point3(7, 0, 4), wallType);
            skewWallPlacer.PlaceSkewWall(new Point3(5, 0, 4), wallType);
            skewWallPlacer.PlaceSkewWall(new Point3(2, 0, 4), wallType);
            skewWallPlacer.PlaceSkewWall(new Point3(2, 0, 5), wallType);

            floorPlacer.PlaceFloor(new Point3(7, 0, 1), floorType);
            floorPlacer.PlaceFloor(new Point3(3, 0, 1), floorType);
            floorPlacer.PlaceFloor(new Point3(2, 0, 1), floorType);
            floorPlacer.PlaceFloor(new Point3(3, 0, 2), floorType);
            floorPlacer.PlaceFloor(new Point3(3, 0, 3), floorType);
            floorPlacer.PlaceFloor(new Point3(6, 0, 3), floorType);

            bool displayBoxes = true;

            game.GameLoopEvent += delegate
            {
                game.SpectaterCamera.CameraPosition =
                                             new SlimDX.Vector3(game.SpectaterCamera.CameraPosition.X, 2,
                                                                game.SpectaterCamera.CameraPosition.Z);

                if (game.Keyboard.IsKeyPressed(Key.R))
                {
                    floorPlacer.RemoveFloor(new Point3(7, 0, 1));
                    floorPlacer.RemoveFloor(new Point3(3, 0, 1));
                    floorPlacer.RemoveFloor(new Point3(2, 0, 1));
                    floorPlacer.RemoveFloor(new Point3(3, 0, 2));
                    floorPlacer.RemoveFloor(new Point3(3, 0, 3));
                    floorPlacer.RemoveFloor(new Point3(6, 0, 3));
                }
                if (game.Keyboard.IsKeyPressed(Key.E))
                {
                    floorPlacer.PlaceFloor(new Point3(7, 0, 1), floorType);
                    floorPlacer.PlaceFloor(new Point3(3, 0, 1), floorType);
                    floorPlacer.PlaceFloor(new Point3(2, 0, 1), floorType);
                    floorPlacer.PlaceFloor(new Point3(3, 0, 2), floorType);
                    floorPlacer.PlaceFloor(new Point3(3, 0, 3), floorType);
                    floorPlacer.PlaceFloor(new Point3(6, 0, 3), floorType);
                }
                if (game.Keyboard.IsKeyPressed(Key.Q))
                {
                    displayBoxes = !displayBoxes;
                }

                if (displayBoxes)
                {
                    for (int i = 0; i < dynBlockFactory.BlockList.Count; i++)
                    {
                        var cPos = dynBlockFactory.BlockList[i].Position;
                        game.LineManager3D.AddCenteredBox(cPos, 1, System.Drawing.Color.White);
                    }
                }

                renderer.Draw();
            };

            game.Run();
        }

       
        [Test]
        public void TestWallFloorDynamicPLAY()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var renderer = new DeferredRenderer(game);
            var meshConverter = new OBJToRAMMeshConverter(new RAMTextureFactory());
            var planeMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                         "\\Plane.mtl");
            DirectionalLight light = renderer.CreateDirectionalLight();
            light.ShadowsEnabled = true;
            light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));
            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100)) * Matrix.Translation(new Vector3(0, -0.5f, 0));

            var basicStraightMesh = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicStraight").FullName +
                                         "\\BasicStraight.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicStraight").FullName +
                                         "\\BasicStraight.mtl");
            var basicPillarMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicPillar").FullName +
                                        "\\BasicPillar.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicPillar").FullName +
                                        "\\BasicPillar.mtl");
            var basicSkewMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicSkew").FullName +
                                        "\\BasicSkew.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicSkew").FullName +
                                        "\\BasicSkew.mtl");
            var basicFloorMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicFloor").FullName +
                                        "\\BasicFloor.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicFloor").FullName +
                                        "\\BasicFloor.mtl");

            var basicStraight = new BuildUnit(basicStraightMesh);
            var basicPillar = new BuildUnit(basicPillarMesh);
            var basicSkew = new BuildUnit(basicSkewMesh);
            var basicFloor = new BuildUnit(basicFloorMesh);

            var wallType = new WallType();
            wallType.StraightUnit = basicStraight;
            wallType.PillarUnit = basicPillar;
            wallType.SkewUnit = basicSkew;

            var floorType = new FloorType();
            floorType.DefaultUnit = basicFloor;

            var dynBlockFactory = new DynamicBlockFactory(renderer);
            var dynTypeFactory = new DynamicTypeFactory();
            dynTypeFactory.WallTypes.Add(wallType);
            dynTypeFactory.FloorTypes.Add(floorType);
            var resolver = new DynamicBlockResolver(dynBlockFactory);

            var straightWallPlacer = new StraightWallPlacer(dynBlockFactory, resolver);
            var skewWallPlacer = new SkewWallPlacer(dynBlockFactory, resolver);
            var floorPlacer = new FloorPlacer(dynBlockFactory, resolver);

            var placeTool = new DynamicPlaceTool(game, dynBlockFactory, dynTypeFactory, straightWallPlacer, skewWallPlacer, floorPlacer);
            placeTool.PlaceMode = DynamicPlaceMode.StraightWallMode;


            bool displayBoxes = false;
            game.GameLoopEvent += delegate
            {
                if(game.Keyboard.IsKeyPressed(Key.NumberPad1))
                    placeTool.PlaceMode = DynamicPlaceMode.StraightWallMode;
                if(game.Keyboard.IsKeyPressed(Key.NumberPad2))
                    placeTool.PlaceMode = DynamicPlaceMode.SkewWallMode;
                if (game.Keyboard.IsKeyPressed(Key.NumberPad3))
                    placeTool.PlaceMode = DynamicPlaceMode.FloorMode;
                
                if (game.Keyboard.IsKeyPressed(Key.Q))
                    displayBoxes = !displayBoxes;

                if (displayBoxes)
                {
                    for (int i = 0; i < dynBlockFactory.BlockList.Count; i++)
                    {
                        var cPos = dynBlockFactory.BlockList[i].Position;
                        game.LineManager3D.AddCenteredBox(cPos, 1, System.Drawing.Color.Gray);
                    }
                }

                placeTool.Update();

                renderer.Draw();
            };

            game.Run();
        }

        [Test]
        public void TestPlaceWindows()
        {
            
        }
    }
}
