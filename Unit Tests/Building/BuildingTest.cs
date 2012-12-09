using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Building;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Rendering;
using NUnit.Framework;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;
using DirectionalLight = MHGameWork.TheWizards.CG.SDL.DirectionalLight;
using PointLight = MHGameWork.TheWizards.CG.SDL.PointLight;

namespace MHGameWork.TheWizards.Tests.Building
{
    [TestFixture]
    public class BuildingTest
    {
        private Data.ModelContainer container;
        private DynamicBlockFactory dynBlockFactory;
        private DX11Game game;
        private DeferredRenderer renderer;
        private DynamicTypeFactory dynTypeFactory;
        private WallPlacer wallPlacer;
        private FloorPlacer floorPlacer;
        private DynamicPlaceTool placeTool;
        private BuildSlotRenderer buildSlotRenderer;

        [SetUp]
        public void Init()
        {
            container = new Data.ModelContainer();
            game = new DX11Game();
            game.InitDirectX();
            renderer = new DeferredRenderer(game);
            buildSlotRenderer = new BuildSlotRenderer(container, renderer);
            var meshConverter = new OBJToRAMMeshConverter(new RAMTextureFactory());

            DirectionalLight light = renderer.CreateDirectionalLight();
            light.ShadowsEnabled = false;
            light.LightDirection = Vector3.Normalize(new Vector3(2, -1, 3));

            PointLight light2 = renderer.CreatePointLight();
            light2.LightPosition = new Vector3(0, 80, 40);
            light2.ShadowsEnabled = true;
            light2.LightRadius = 1000; //does this do anything??
            light2.LightIntensity = 1;

            /* old white planemesh
            var planeMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                        "\\Plane.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building").FullName +
                                        "\\Plane.mtl");
            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(100, 100, 100)) * Matrix.Translation(new Vector3(0, -0.5f, 0));
            */

            var planeMesh = CreateMeshFromObj(meshConverter,
                                       TWDir.GameData.CreateSubdirectory("Core\\GroundPlane").FullName +
                                       "\\GroundPlane001.obj",
                                       TWDir.GameData.CreateSubdirectory("Core\\GroundPlane").FullName +
                                       "\\GroundPlane001.mtl");
            var planeEl = renderer.CreateMeshElement(planeMesh);
            planeEl.WorldMatrix = Matrix.Scaling(new Vector3(20, 20, 20)) * Matrix.Translation(new Vector3(0, -0.5f, 0));

            var skydome = CreateMeshFromObj(meshConverter,
                                         TWDir.GameData.CreateSubdirectory("Core\\Skydome").FullName +
                                         "\\Skydome001.obj",
                                         TWDir.GameData.CreateSubdirectory("Core\\Skydome").FullName +
                                         "\\Skydome001.mtl");
            var skydomeEl = renderer.CreateMeshElement(skydome);
            skydomeEl.WorldMatrix = Matrix.Scaling(new Vector3(1, 1, 1));



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

            dynBlockFactory = new DynamicBlockFactory(renderer, container);
            dynTypeFactory = new DynamicTypeFactory();
            dynTypeFactory.WallTypes.Add(wallType);
            dynTypeFactory.FloorTypes.Add(floorType);
            var resolver = new DynamicBlockResolver(dynBlockFactory);

            wallPlacer = new WallPlacer(dynBlockFactory, resolver);
            floorPlacer = new FloorPlacer(dynBlockFactory, resolver);

            placeTool = new DynamicPlaceTool(game, dynBlockFactory, dynTypeFactory, wallPlacer, floorPlacer);
        }

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


            Point3[] basicBlockLayoutList = { new Point3(0, 0, 0) };
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


            var dynBlock1 = new DynamicBlock(new Point3(1, 0, 0), renderer, container);
            var dynBlock2 = new DynamicBlock(new Point3(-1, 0, -1), renderer, container);
            var dynBlock3 = new DynamicBlock(new Point3(1, 0, 2), renderer, container);

            var straightUnit = new BuildUnit(basicStraight);
            var pillarUnit = new BuildUnit(basicPillar);
            var floorUnit = new BuildUnit(basicFloor);
            var skewUnit = new BuildUnit(basicSkew);

            dynBlock1.SetStraight(DynamicBlockDirection.X, 0, straightUnit);
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

            var dynBlockFactory = new DynamicBlockFactory(renderer, container);
            var resolver = new DynamicBlockResolver(dynBlockFactory);
            var wallPlacer = new WallPlacer(dynBlockFactory, resolver);

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
                if (game.Keyboard.IsKeyPressed(Key.R))
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

            var dynBlockFactory = new DynamicBlockFactory(renderer, container);
            var resolver = new DynamicBlockResolver(dynBlockFactory);

            var wallPlacer = new WallPlacer(dynBlockFactory, resolver);

            wallPlacer.PlaceStraightWall(new Point3(1, 0, 0), wallType);


            wallPlacer.PlaceStraightWall(new Point3(5, 0, 1), wallType);
            wallPlacer.PlaceStraightWall(new Point3(6, 0, 2), wallType);
            wallPlacer.PlaceStraightWall(new Point3(7, 0, 3), wallType);
            wallPlacer.PlaceStraightWall(new Point3(6, 0, 4), wallType);
            wallPlacer.PlaceStraightWall(new Point3(5, 0, 3), wallType);
            wallPlacer.PlaceStraightWall(new Point3(5, 0, 5), wallType);
            wallPlacer.PlaceStraightWall(new Point3(6, 0, 5), wallType);
            wallPlacer.PlaceStraightWall(new Point3(4, 0, 4), wallType);
            wallPlacer.PlaceStraightWall(new Point3(3, 0, 4), wallType);


            wallPlacer.PlaceResolvedSkewWall(new Point3(6, 0, 1), wallType);
            wallPlacer.PlaceResolvedSkewWall(new Point3(7, 0, 2), wallType);
            wallPlacer.PlaceResolvedSkewWall(new Point3(7, 0, 4), wallType);
            wallPlacer.PlaceResolvedSkewWall(new Point3(5, 0, 4), wallType);
            wallPlacer.PlaceResolvedSkewWall(new Point3(2, 0, 4), wallType);
            wallPlacer.PlaceResolvedSkewWall(new Point3(2, 0, 5), wallType);


            game.GameLoopEvent += delegate
            {
                game.SpectaterCamera.CameraPosition =
                                             new SlimDX.Vector3(game.SpectaterCamera.CameraPosition.X, 2,
                                                                game.SpectaterCamera.CameraPosition.Z);

                if (game.Keyboard.IsKeyPressed(Key.R))
                {
                    wallPlacer.RemoveStraightWall(new Point3(6, 0, 2));
                    wallPlacer.RemoveStraightWall(new Point3(5, 0, 3));
                    wallPlacer.RemoveStraightWall(new Point3(6, 0, 5));
                    wallPlacer.RemoveSkewWall(new Point3(7, 0, 2));
                    wallPlacer.RemoveSkewWall(new Point3(5, 0, 4));
                }
                if (game.Keyboard.IsKeyPressed(Key.E))
                {
                    wallPlacer.PlaceStraightWall(new Point3(6, 0, 2), wallType);
                    wallPlacer.PlaceStraightWall(new Point3(5, 0, 3), wallType);
                    wallPlacer.PlaceStraightWall(new Point3(6, 0, 5), wallType);
                    wallPlacer.PlaceResolvedSkewWall(new Point3(7, 0, 2), wallType);
                    wallPlacer.PlaceResolvedSkewWall(new Point3(5, 0, 4), wallType);
                }

                renderer.Draw();
            };

            game.Run();
        }

        /// <summary> 
        /// Tests if floors can be placed by specifying a (non-integer) vector.
        /// Test succeeded if:
        /// - floors are placed at the origin in the XZclose, XZfar, MinXZclose and MinXMinZfar slots
        /// - floors are placed at 100MinXMinZfar and 001MinXMinZfar
        /// - pressing R results in deleting all the floors
        /// - pressing E results in replacing the floors when they are deleted
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

            var basicFloorMesh = CreateMeshFromObj(meshConverter,
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicFloor").FullName +
                                        "\\BasicFloor.obj",
                                        TWDir.GameData.CreateSubdirectory("Core\\Building\\DynamicBlock\\BasicFloor").FullName +
                                        "\\BasicFloor.mtl");

            var basicFloor = new BuildUnit(basicFloorMesh);

            var floorType = new FloorType();
            floorType.DefaultUnit = basicFloor;

            var dynBlockFactory = new DynamicBlockFactory(renderer, container);
            var resolver = new DynamicBlockResolver(dynBlockFactory);

            var floorPlacer = new FloorPlacer(dynBlockFactory, resolver);

            floorPlacer.PlaceFloor(new Vector3(0.1f, 0f, 0.1f), floorType);
            floorPlacer.PlaceFloor(new Vector3(0.4f, 0f, 0.4f), floorType);
            floorPlacer.PlaceFloor(new Vector3(-0.1f, 0f, 0.1f), floorType);
            floorPlacer.PlaceFloor(new Vector3(-0.3f, 0f, -0.3f), floorType);
            floorPlacer.PlaceFloor(new Vector3(0, 0f, 0.6f), floorType);
            floorPlacer.PlaceFloor(new Vector3(0.51f, 0f, 0), floorType);

            bool displayBoxes = true;

            game.GameLoopEvent += delegate
            {
                game.SpectaterCamera.CameraPosition =
                                             new SlimDX.Vector3(game.SpectaterCamera.CameraPosition.X, 2,
                                                                game.SpectaterCamera.CameraPosition.Z);

                if (game.Keyboard.IsKeyPressed(Key.R))
                {
                    floorPlacer.RemoveFloor(new Vector3(0.1f, 0f, 0.1f));
                    floorPlacer.RemoveFloor(new Vector3(0.4f, 0f, 0.4f));
                    floorPlacer.RemoveFloor(new Vector3(-0.1f, 0f, 0.1f));
                    floorPlacer.RemoveFloor(new Vector3(-0.3f, 0f, -0.3f));
                    floorPlacer.PlaceFloor(new Vector3(0, 0f, 0.6f), floorType);
                    floorPlacer.PlaceFloor(new Vector3(0.51f, 0f, 0), floorType);
                }
                if (game.Keyboard.IsKeyPressed(Key.E))
                {
                    floorPlacer.PlaceFloor(new Vector3(0.1f, 0f, 0.1f), floorType);
                    floorPlacer.PlaceFloor(new Vector3(0.4f, 0f, 0.4f), floorType);
                    floorPlacer.PlaceFloor(new Vector3(-0.1f, 0f, 0.1f), floorType);
                    floorPlacer.PlaceFloor(new Vector3(-0.3f, 0f, -0.3f), floorType);
                    floorPlacer.PlaceFloor(new Vector3(0, 0f, 0.6f), floorType);
                    floorPlacer.PlaceFloor(new Vector3(0.51f, 0f, 0), floorType);
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

        /// <summary>
        /// Tests the placement of floors by painting.
        /// Controls:
        /// - E: place floor at cursor
        /// - R: remove floor at cursor
        /// - Arrow Up/Down: lift/lower build plane
        /// </summary>
        [Test]
        public void TestPaintFloorsDynamic()
        {
            

            placeTool.PlaceMode = DynamicPlaceMode.FloorMode;

            bool displayBoxes = true;
            game.GameLoopEvent += delegate
            {
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

                placeTool.Update();
                renderer.Draw();
            };

            game.Run();
        }

        [Test]
        public void TestPlaceStraightWallsPointToPoint()
        {
           
            //Note that exactly one coordinate from these vectors should be 0.5 +- n!! 
            wallPlacer.PlaceStraightWallsPointToPoint(new Vector3(0, 0, -0.5f), new Vector3(0, 0, 3.5f), dynTypeFactory.WallTypes[0]);
            wallPlacer.PlaceStraightWallsPointToPoint(new Vector3(2.5f, 0, 0), new Vector3(8, 0, 3.5f), dynTypeFactory.WallTypes[0]);
            wallPlacer.PlaceStraightWallsPointToPoint(new Vector3(-0.5f, 0, 2), new Vector3(-8.5f, 0, -2), dynTypeFactory.WallTypes[0]);
            wallPlacer.PlaceStraightWallsPointToPoint(new Vector3(5, 0, 5.5f), new Vector3(5, 0, 6.5f), dynTypeFactory.WallTypes[0]);

            bool displayBoxes = true;
            game.GameLoopEvent += delegate
            {
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
        public void TestPlaceSkewWallsPointToPoint()
        {
            

            //Note that exactly one coordinate from these vectors should be 0.5 +- n!! 
            wallPlacer.PlaceSkewWallsPointToPoint(new Vector3(2.5f, 0, 0), new Vector3(0, 0, 2.5f), dynTypeFactory.WallTypes[0]);
            wallPlacer.PlaceSkewWallsPointToPoint(new Vector3(2.5f, 1, 0), new Vector3(0, 1, 2.5f), dynTypeFactory.WallTypes[0]);
            wallPlacer.PlaceSkewWallsPointToPoint(new Vector3(2.5f, 0, 0), new Vector3(5, 0, 2.5f), dynTypeFactory.WallTypes[0]);
            wallPlacer.PlaceSkewWallsPointToPoint(new Vector3(5, 0, 5.5f), new Vector3(20, 0, 2.5f), dynTypeFactory.WallTypes[0]);

            bool displayBoxes = true;
            game.GameLoopEvent += delegate
            {
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
        public void TestPlaceWindows()
        {

        }

        /// <summary>
        /// PLAYTEST
        /// Tests all features that are working.
        /// User input needed (duh, that's why it's called a playtest).
        /// </summary>
        [Test]
        public void TestWallFloorDynamicPLAY()
        {
            

            placeTool.PlaceMode = DynamicPlaceMode.StraightWallMode;


            bool displayBoxes = false;
            game.GameLoopEvent += delegate
                                      {
                                          game.SpectaterCamera.CameraPosition = new Vector3(game.SpectaterCamera.CameraPosition.X, placeTool.planeHeight + 2.5f, game.SpectaterCamera.CameraPosition.Z);

                                          if (game.Keyboard.IsKeyPressed(Key.NumberPad1))
                                              placeTool.PlaceMode = DynamicPlaceMode.StraightWallMode;
                                          if (game.Keyboard.IsKeyPressed(Key.NumberPad2))
                                              placeTool.PlaceMode = DynamicPlaceMode.SkewWallMode;
                                          if (game.Keyboard.IsKeyPressed(Key.NumberPad3))
                                              placeTool.PlaceMode = DynamicPlaceMode.FloorMode;
                                          if (game.Keyboard.IsKeyPressed(Key.NumberPad4))
                                              placeTool.PlaceMode = DynamicPlaceMode.PTPStraightMode;
                                          if (game.Keyboard.IsKeyPressed(Key.NumberPad5))
                                              placeTool.PlaceMode = DynamicPlaceMode.PTPSkewMode;

                                          if (game.Keyboard.IsKeyPressed(Key.UpArrow))
                                              placeTool.planeHeight++;
                                          if (game.Keyboard.IsKeyPressed(Key.DownArrow))
                                          {
                                              placeTool.planeHeight--;
                                              if (placeTool.planeHeight < -0.5)
                                                  placeTool.planeHeight++;
                                          }

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

                                          buildSlotRenderer.ProcessWorldChanges();
                                          container.ClearDirty();
                                          renderer.Draw();
                                      };

            game.Run();
        }

        [Test]
        public void TestPlayerPTP()
        {
           

            Vector3 pos1 = new Vector3(0, 0, 0.6f);
            Assert.GreaterOrEqual(0.5f, -1 * placeTool.snapToFaceMids(pos1).Z);

            Vector3 pos2 = new Vector3(0, 0, 0.9f);
            Assert.GreaterOrEqual(0.5f, -1 * placeTool.snapToFaceMids(pos1).Z);

            placeTool.PlaceMode = DynamicPlaceMode.PTPStraightMode;

            bool displayBoxes = false;
            game.GameLoopEvent += delegate
            {
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

                if (game.Keyboard.IsKeyPressed(Key.NumberPad1))
                    placeTool.PlaceMode = DynamicPlaceMode.PTPStraightMode;
                if (game.Keyboard.IsKeyPressed(Key.NumberPad2))
                    placeTool.PlaceMode = DynamicPlaceMode.PTPSkewMode;

                placeTool.Update();

                renderer.Draw();
            };

            game.Run();
        }
    }
}
