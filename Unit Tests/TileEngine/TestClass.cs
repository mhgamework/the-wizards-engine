using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Tests.Rendering;
using MHGameWork.TheWizards.Tests.TileEngine;
using MHGameWork.TheWizards.TileEngine.SnapEngine;
using NUnit.Framework;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using System.IO;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Editor.Transform;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.ServerClient;


namespace MHGameWork.TheWizards.TileEngine
{
    [TestFixture]
    public class TestClass
    {
        private RAMMesh meshWallInnerCorner;
        private RAMMesh meshWallStraight;
        private WorldObjectType wallInnerCornerType;
        private WorldObjectType wallStraightType;
        private List<WorldObjectType> typeList = new List<WorldObjectType>();

        [Test]
        public void TestStart()
        {
            XNAGame game = new XNAGame();

            game.InitializeEvent += delegate
            {
            };
            game.DrawEvent += delegate
            {
            };
            game.UpdateEvent += delegate
            {
            };
            game.Run();
        }

        [Test]
        public void TestLoadObject()
        {
            XNAGame game = new XNAGame();

            OBJParser.ObjImporter importer = new OBJParser.ObjImporter();
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());

            importer.AddMaterialFileStream("Goblin02.mtl", new FileStream("../../../Assets/Goblin/Goblin02.mtl", FileMode.Open));
            importer.ImportObjFile("../../../Assets/Goblin/Goblin02.obj");

            var mesh = c.CreateMesh(importer);

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);

            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);

            var el = renderer.AddMesh(mesh);
            el.WorldMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 0));


            game.IsFixedTimeStep = false;
            game.DrawFps = true;

            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);

            game.InitializeEvent += delegate
            {
            };
            game.DrawEvent += delegate
            {
            };
            game.UpdateEvent += delegate
            {
            };
            game.Run();

        }

        [Test]
        public void TestSnapPointToPoint()
        {
            XNAGame game = new XNAGame();

            SnapInformation inf1 = new SnapInformation();
            SnapInformation inf2 = new SnapInformation();

            SnapPoint p1 = new SnapPoint();
            p1.Position = new Vector3(1.7f, 1, 1);
            p1.Normal = new Vector3(0, 0, 1);
            p1.Normal.Normalize();
            p1.Up = new Vector3(0, -1, 0);
            p1.Up.Normalize();

            SnapPoint p2 = new SnapPoint();
            p2.Position = new Vector3(1, 1, 1);
            p2.Normal = new Vector3(1, 0, 0);
            p2.Normal.Normalize();
            p2.Up = new Vector3(0, -1, 0);
            p2.Up.Normalize();

            SnapPoint p3 = new SnapPoint();
            p3.Position = new Vector3(2, 0, 2);
            p3.Normal = new Vector3(1, 1, 0);
            p3.Normal.Normalize();
            p3.Up = new Vector3(1, -1, 0);
            p3.Up.Normalize();

            SnapType type = new SnapType("t1");
            p1.SnapType = type;
            p2.SnapType = type;

            inf1.addSnapObject(p1);
            inf1.addSnapObject(p3);
            inf2.addSnapObject(p2);

            SnapEngineClass snapEngine = new SnapEngineClass();
            snapEngine.addSnapInformation(inf1);
            snapEngine.addSnapInformation(inf2);

            SphereMesh sphere1 = new SphereMesh(0.2f, 24, Color.Green);
            SphereMesh sphere2 = new SphereMesh(0.2f, 24, Color.Yellow);
            SphereMesh sphere3 = new SphereMesh(0.2f, 24, Color.Green);

            sphere1.WorldMatrix = Matrix.CreateTranslation(p1.Position);
            sphere2.WorldMatrix = Matrix.CreateTranslation(p2.Position);
            sphere3.WorldMatrix = Matrix.CreateTranslation(p3.Position);

            game.AddXNAObject(sphere1);
            game.AddXNAObject(sphere2);
            game.AddXNAObject(sphere3);

            game.DrawEvent += delegate
            {
                game.LineManager3D.AddLine(p1.Position, p1.Position + 0.5f * p1.Normal, Color.Red);
                game.LineManager3D.AddLine(p1.Position, p1.Position + 0.5f * p1.Up, Color.Yellow);

                game.LineManager3D.AddLine(p3.Position, p3.Position + 0.5f * p3.Normal, Color.Red);
                game.LineManager3D.AddLine(p3.Position, p3.Position + 0.5f * p3.Up, Color.Yellow);

                game.LineManager3D.AddLine(p2.Position, p2.Position + 0.5f * p2.Normal, Color.Red);
                game.LineManager3D.AddLine(p2.Position, p2.Position + 0.5f * p2.Up, Color.Yellow);
            };
            game.UpdateEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    snapEngine.SnapTo(inf1, inf2);
                    sphere1.WorldMatrix = Matrix.CreateTranslation(p1.Position);
                    sphere2.WorldMatrix = Matrix.CreateTranslation(p2.Position);
                    sphere3.WorldMatrix = Matrix.CreateTranslation(p3.Position);
                }
            };

            game.Run();
        }

        [Test]
        public void TestSnapPointToPoint2()
        {
            XNAGame game = new XNAGame();


            SnapPoint p1 = new SnapPoint();
            p1.Position = new Vector3(0, 0, 0);
            var w2 = (float)(1 / Math.Sqrt(2));
            p1.Normal = Vector3.Normalize(new Vector3(w2 * w2, w2, w2 * w2));
            p1.Up = Vector3.Normalize(new Vector3(-w2 * w2, w2, -w2 * w2));
            p1.ClockwiseWinding = true;

            SnapPoint p2 = new SnapPoint();
            p2.Position = new Vector3(0, 0, 0);
            p2.Normal = Vector3.Normalize(new Vector3(1, 0, 0));
            p2.Up = Vector3.Normalize(new Vector3(0, 1, 0));
            p2.ClockwiseWinding = false;

            var snapper = new SnapperPointPoint();

            List<Transformation> transformations = new List<Transformation>();


            game.DrawEvent += delegate
                                  {
                                      transformations.Clear();
                                      snapper.SnapAToB(p2, p1, Transformation.Identity, transformations);
                                      game.LineManager3D.DrawGroundShadows = true;

                                      game.LineManager3D.AddLine(p1.Position, p1.Position + 3f * p1.Normal, Color.Red);
                                      game.LineManager3D.AddLine(p1.Position, p1.Position + 3f * p1.Up, Color.Blue);

                                      game.LineManager3D.WorldMatrix = transformations[0].CreateMatrix();

                                      game.LineManager3D.AddLine(p2.Position, p2.Position + 3f * p2.Normal + Vector3.One * 0.01f, Color.Yellow);
                                      game.LineManager3D.AddLine(p2.Position, p2.Position + 3f * p2.Up + Vector3.One * 0.01f, Color.Purple);
                                  };

            game.Run();
        }


        public void TestGizmo()
        {
            XNAGame game = new XNAGame();

            EditorGizmoTranslation translationGizmo = new EditorGizmoTranslation();
            EditorGrid grid = new EditorGrid();

            translationGizmo.Position = new Vector3(0, 0, 0);
            translationGizmo.Enabled = true;

            grid.Size = new Vector2(100, 100);
            grid.Interval = 1;
            grid.MajorInterval = 10;


            bool toggle = false;

            game.InitializeEvent += delegate
           {
               translationGizmo.Load(game);
           };
            game.DrawEvent += delegate
            {

                game.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;

                translationGizmo.Render(game);
                grid.Render(game);
            };
            game.UpdateEvent += delegate
            {
                translationGizmo.Update(game);

                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                {
                    toggle = !toggle;
                }
                if (toggle)
                {
                    game.Mouse.CursorEnabled = true;
                    game.IsMouseVisible = true;
                    game.SpectaterCamera.Enabled = false;
                }
                else
                {
                    game.Mouse.CursorEnabled = false;
                    game.IsMouseVisible = false;
                    game.SpectaterCamera.Enabled = true;
                }

            };

            game.Run();
        }

        [Test]
        public void TestMoveMesh()
        {
            XNAGame game = new XNAGame();
            game.IsFixedTimeStep = false;
            game.DrawFps = true;

            //Import obj-files
            OBJParser.ObjImporter importer = new OBJParser.ObjImporter();
            var meshCreator = new OBJToRAMMeshConverter(new RAMTextureFactory());
            importer.AddMaterialFileStream("WallCorner.mtl", new FileStream("../../../Assets/WallCorner/WallCorner.mtl", FileMode.Open));
            importer.ImportObjFile("../../../Assets/WallCorner/WallCorner.obj");
            var meshWallCorner = meshCreator.CreateMesh(importer);
            importer.AddMaterialFileStream("WallStraight.mtl", new FileStream("../../../Assets/WallStraight/WallStraight.mtl", FileMode.Open));
            importer.ImportObjFile("../../../Assets/WallStraight/WallStraight.obj");
            var meshWallStraight = meshCreator.CreateMesh(importer);

            TexturePool texturePool = new TexturePool();
            MeshPartPool meshpartPool = new MeshPartPool();
            VertexDeclarationPool vertexDeclarationPool = new VertexDeclarationPool();
            SimpleMeshRenderer renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);
            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);
            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);


            //Create WorldObjectTypes
            WorldObjectType wallStraightType = new WorldObjectType(meshWallStraight, Guid.NewGuid());
            WorldObjectType wallCornerType = new WorldObjectType(meshWallCorner, Guid.NewGuid());



            //Add some WorldObjects to the WorldSpace
            WorldObject wallStraight = new WorldObject(game, wallStraightType, renderer);
            WorldObject wallCorner = new WorldObject(game, wallCornerType, renderer);

            List<WorldObject> WorldObjectList = new List<WorldObject>();
            WorldObjectList.Add(wallCorner);
            WorldObjectList.Add(wallStraight);

            World world = new World();
            world.WorldObjectList.AddRange(WorldObjectList);

            WorldObjectList[1].Position = new Vector3(30, 0, 0);

            WorldObjectFactory factory = new WorldObjectFactory(world, new SimpleMeshFactory(), new TileFaceTypeFactory());

            TileSnapInformationBuilder builder = new TileSnapInformationBuilder();
            WorldObjectMoveTool moveTool = new WorldObjectMoveTool(game, world, factory, builder, renderer);
            game.AddXNAObject(moveTool);

            game.Run();
        }

        [Test]
        public void TestAddDeleteMeshToFromWorld()
        {
            XNAGame game = new XNAGame();

            OBJParser.ObjImporter importer = new OBJParser.ObjImporter();
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());

            importer.AddMaterialFileStream("WallInnerCorner.mtl", new FileStream(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallInnerCorner.mtl", FileMode.Open));
            importer.ImportObjFile(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallInnerCorner.obj");
            var meshWallInnerCorner = c.CreateMesh(importer);
            importer.AddMaterialFileStream("WallStraight.mtl", new FileStream(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallStraight.mtl", FileMode.Open));
            importer.ImportObjFile(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallStraight.obj");
            var meshWallStraight = c.CreateMesh(importer);
            importer.AddMaterialFileStream("WallOuterCorner.mtl", new FileStream(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallOuterCorner.mtl", FileMode.Open));
            importer.ImportObjFile(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallOuterCorner.obj");
            var meshWallOuterCorner = c.CreateMesh(importer);

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);

            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);

            World world = new World();
            TileSnapInformationBuilder builder = new TileSnapInformationBuilder();

            WorldObjectPlaceTool placeTool = new WorldObjectPlaceTool(game, world, renderer, builder, new SimpleMeshFactory(), new TileFaceTypeFactory());
            WorldObjectType type1 = new WorldObjectType(meshWallInnerCorner, Guid.NewGuid());
            WorldObjectType type2 = new WorldObjectType(meshWallStraight, Guid.NewGuid());
            WorldObjectType type3 = new WorldObjectType(meshWallOuterCorner, Guid.NewGuid());

            var tileDataInnerCorner = new TileData(Guid.NewGuid());
            var tileDataStraight = new TileData(Guid.NewGuid());
            var tileDataOuterCorner = new TileData(Guid.NewGuid());
            tileDataInnerCorner.Dimensions = type1.BoundingBox.Max - type1.BoundingBox.Min;
            tileDataStraight.Dimensions = type2.BoundingBox.Max - type2.BoundingBox.Min;
            tileDataOuterCorner.Dimensions = type1.BoundingBox.Max - type1.BoundingBox.Min;

            var faceSnapType1 = new TileFaceType(Guid.NewGuid()) { Name = "type1" };
            //var faceSnapType2 = new TileFaceType() { Name = "type2" };

            tileDataInnerCorner.SetFaceType(TileFace.Front, faceSnapType1);
            tileDataInnerCorner.SetLocalWinding(TileFace.Front, true);

            tileDataInnerCorner.SetFaceType(TileFace.Left, faceSnapType1);
            tileDataInnerCorner.SetLocalWinding(TileFace.Left, true);

            tileDataStraight.SetFaceType(TileFace.Back, faceSnapType1);
            tileDataStraight.SetLocalWinding(TileFace.Back, false);

            tileDataStraight.SetFaceType(TileFace.Front, faceSnapType1);
            tileDataStraight.SetLocalWinding(TileFace.Front, true);

            tileDataOuterCorner.SetFaceType(TileFace.Front, faceSnapType1);
            tileDataOuterCorner.SetLocalWinding(TileFace.Front, true);

            tileDataOuterCorner.SetFaceType(TileFace.Right, faceSnapType1);
            tileDataOuterCorner.SetLocalWinding(TileFace.Right, true);


            type1.TileData = tileDataInnerCorner;
            type2.TileData = tileDataStraight;
            type3.TileData = tileDataOuterCorner;


            type1.SnapInformation = builder.CreateFromTile(tileDataInnerCorner);
            type2.SnapInformation = builder.CreateFromTile(tileDataStraight);
            type3.SnapInformation = builder.CreateFromTile(tileDataOuterCorner);

            List<WorldObjectType> typeList = new List<WorldObjectType>();
            typeList.Add(type1);
            typeList.Add(type2);
            typeList.Add(type3);

            WorldObjectFactory factory = new WorldObjectFactory(world, new SimpleMeshFactory(), new TileFaceTypeFactory());
            WorldObjectMoveTool moveTool = new WorldObjectMoveTool(game, world, factory, builder, renderer);

            game.AddXNAObject(moveTool);
            game.AddXNAObject(placeTool);

            game.IsFixedTimeStep = false;
            game.DrawFps = true;
            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);

            EditorGrid grid;
            grid = new EditorGrid();
            grid.Size = new Vector2(100, 100);
            grid.Interval = 1;
            grid.MajorInterval = 10;

            bool mouseEnabled = false;

            game.UpdateEvent += delegate
                                    {
                                        if (placeTool.Enabled)
                                        {
                                            moveTool.Enabled = false;

                                            if (placeTool.ObjectsPlacedSinceEnabled() == 1)
                                                placeTool.Enabled = false;
                                        }
                                        else
                                        {
                                            moveTool.Enabled = true;
                                        }

                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad1))
                                        {
                                            placeTool.PlaceType = typeList[0];
                                            placeTool.Enabled = true;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad2))
                                        {
                                            placeTool.PlaceType = typeList[1];
                                            placeTool.Enabled = true;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad3))
                                        {
                                            placeTool.PlaceType = typeList[2];
                                            placeTool.Enabled = true;
                                        }

                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                                        {
                                            mouseEnabled = !mouseEnabled;
                                        }
                                        if (mouseEnabled)
                                        {
                                            game.Mouse.CursorEnabled = true;
                                            game.IsMouseVisible = true;
                                            game.SpectaterCamera.Enabled = false;
                                        }
                                        else
                                        {
                                            game.Mouse.CursorEnabled = false;
                                            game.IsMouseVisible = false;
                                            game.SpectaterCamera.Enabled = true;
                                            //activeWorldObject = null;
                                        }

                                    };
            game.DrawEvent += delegate
                                  {
                                      grid.Render(game);
                                  };

            game.Run();
        }

        [Test]
        public void TestLearnSnap()
        {
            XNAGame game = new XNAGame();

            OBJParser.ObjImporter importer = new OBJParser.ObjImporter();
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());

            importer.AddMaterialFileStream("WallInnerCorner.mtl", new FileStream(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallInnerCorner.mtl", FileMode.Open));
            importer.ImportObjFile(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallInnerCorner.obj");
            var meshWallInnerCorner = c.CreateMesh(importer);
            importer.AddMaterialFileStream("WallStraight.mtl", new FileStream(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallStraight.mtl", FileMode.Open));
            importer.ImportObjFile(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallStraight.obj");
            var meshWallStraight = c.CreateMesh(importer);

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);

            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);

            World world = new World();

            WorldObjectType type1 = new WorldObjectType(meshWallInnerCorner, Guid.NewGuid());
            WorldObjectType type2 = new WorldObjectType(meshWallStraight, Guid.NewGuid());

            var tileDataInnerCorner = new TileData(Guid.NewGuid());
            var tileDataStraight = new TileData(Guid.NewGuid());

            tileDataInnerCorner.Dimensions = type1.BoundingBox.Max - type1.BoundingBox.Min;
            tileDataInnerCorner.MeshOffset = Matrix.CreateTranslation(new Vector3(0, -2, 0));
            tileDataStraight.Dimensions = type2.BoundingBox.Max - type2.BoundingBox.Min;
            tileDataStraight.MeshOffset = Matrix.CreateTranslation(new Vector3(0, -2, 0));


            type1.TileData = tileDataInnerCorner;
            type2.TileData = tileDataStraight;


            WorldObjectFactory factory = new WorldObjectFactory(world, new SimpleMeshFactory(), new TileFaceTypeFactory());
            var TileInnerCorner = factory.CreateNewWorldObject(game, type1, renderer);
            TileInnerCorner.Position = new Vector3(-7, 0, 0);
            var TileStraight = factory.CreateNewWorldObject(game, type2, renderer);
            TileStraight.Position = new Vector3(7, 0, 0);

            game.IsFixedTimeStep = false;
            game.DrawFps = true;
            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);

            EditorGrid grid;
            grid = new EditorGrid();
            grid.Size = new Vector2(100, 100);
            grid.Interval = 1;
            grid.MajorInterval = 10;

            var snapLearnTool = new SnapLearnTool(world, renderer, new TileSnapInformationBuilder());
            game.AddXNAObject(snapLearnTool);

            bool mouseEnabled = false;

            var WallCornerBB = type1.TileData.GetBoundingBox();

            game.UpdateEvent += delegate
                                    {
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                                        {
                                            mouseEnabled = !mouseEnabled;
                                        }
                                        if (mouseEnabled)
                                        {
                                            game.Mouse.CursorEnabled = true;
                                            game.IsMouseVisible = true;
                                            game.SpectaterCamera.Enabled = false;
                                        }
                                        else
                                        {
                                            game.Mouse.CursorEnabled = false;
                                            game.IsMouseVisible = false;
                                            game.SpectaterCamera.Enabled = true;
                                            //activeWorldObject = null;
                                        }

                                    };
            game.DrawEvent += delegate
                                  {
                                      grid.Render(game);
                                      game.LineManager3D.WorldMatrix = TileInnerCorner.WorldMatrix;
                                      //game.LineManager3D.AddBox(WallCornerBB, Color.White);
                                  };

            game.Run();
        }

        /// <summary>
        /// Controls:
        /// 
        /// LeftAlt: Toggle mouse on/off
        /// M: Activate move-tool
        /// L: Activate snaplearn-tool
        /// Numpad 1: Activate place-tool, place wallInnerCorner
        /// Numpad 2: Activate place-tool, place wallStraight
        /// R: Toggle gizmo type (rotation/translation) in move-tool
        /// Standard camera movement controls
        /// </summary>
        [Test]
        public void RunEditor()
        {
            XNAGame game = new XNAGame();

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();
            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);
            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);

            game.IsFixedTimeStep = false;
            game.DrawFps = true;
            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);

            EditorGrid grid;
            grid = new EditorGrid();
            grid.Size = new Vector2(100, 100);
            grid.Interval = 1;
            grid.MajorInterval = 10;

            World world = new World();
            var factory = new WorldObjectFactory(world, new SimpleMeshFactory(), new TileFaceTypeFactory());
            var builder = new TileSnapInformationBuilder();

            //var TileInnerCorner = factory.CreateNewWorldObject(game, wallInnerCornerType, renderer);
            //TileInnerCorner.Position = new Vector3(-7, 0, 0);
            //var TileStraight = factory.CreateNewWorldObject(game, wallStraightType, renderer);
            //TileStraight.Position = new Vector3(7, 0, 0);

            var placeTool = new WorldObjectPlaceTool(game, world, renderer, builder, new SimpleMeshFactory(), new TileFaceTypeFactory());
            var moveTool = new WorldObjectMoveTool(game, world, factory, builder, renderer);
            var snapLearnTool = new SnapLearnTool(world, renderer, builder);

            game.AddXNAObject(placeTool);
            game.AddXNAObject(moveTool);
            game.AddXNAObject(snapLearnTool);


            setupWorldObjectTypes(game, renderer);






            bool mouseEnabled = false;

            game.UpdateEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.M))
                {
                    placeTool.Enabled = false;
                    moveTool.Enabled = true;
                    snapLearnTool.Enabled = false;
                }
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.L))
                {
                    placeTool.Enabled = false;
                    moveTool.Enabled = false;
                    snapLearnTool.Enabled = true;
                }


                if (placeTool.Enabled)
                {

                    moveTool.Enabled = false;
                    snapLearnTool.Enabled = false;

                    if (placeTool.ObjectsPlacedSinceEnabled() == 1)
                        placeTool.Enabled = false;
                }


                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad1))
                {
                    placeTool.PlaceType = typeList[0];
                    placeTool.Enabled = true;
                }
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad2))
                {
                    placeTool.PlaceType = typeList[1];
                    placeTool.Enabled = true;
                }

                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                {
                    mouseEnabled = !mouseEnabled;
                }
                if (mouseEnabled)
                {
                    game.Mouse.CursorEnabled = true;
                    game.IsMouseVisible = true;
                    game.SpectaterCamera.Enabled = false;
                }
                else
                {
                    game.Mouse.CursorEnabled = false;
                    game.IsMouseVisible = false;
                    game.SpectaterCamera.Enabled = true;
                    //activeWorldObject = null;
                }

            };

            game.DrawEvent += delegate
            {
                grid.Render(game);

            };

            game.Run();
        }

        private void setupWorldObjectTypes(XNAGame game, SimpleMeshRenderer renderer)
        {
            OBJParser.ObjImporter importer = new OBJParser.ObjImporter();
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());

            importer.AddMaterialFileStream("WallInnerCorner.mtl", new FileStream(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallInnerCorner.mtl", FileMode.Open));
            importer.ImportObjFile(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallInnerCorner.obj");
            meshWallInnerCorner = c.CreateMesh(importer);
            importer.AddMaterialFileStream("WallStraight.mtl", new FileStream(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallStraight.mtl", FileMode.Open));
            importer.ImportObjFile(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallStraight.obj");
            meshWallStraight = c.CreateMesh(importer);

            wallInnerCornerType = new WorldObjectType(meshWallInnerCorner, Guid.NewGuid());
            wallStraightType = new WorldObjectType(meshWallStraight, Guid.NewGuid());

            var tileDataInnerCorner = new TileData(Guid.NewGuid());
            var tileDataStraight = new TileData(Guid.NewGuid());

            tileDataInnerCorner.Dimensions = wallInnerCornerType.BoundingBox.Max - wallInnerCornerType.BoundingBox.Min;
            tileDataInnerCorner.MeshOffset = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            tileDataStraight.Dimensions = wallStraightType.BoundingBox.Max - wallStraightType.BoundingBox.Min;
            tileDataStraight.MeshOffset = Matrix.CreateTranslation(new Vector3(0, 0, 0));

            wallInnerCornerType.TileData = tileDataInnerCorner;
            wallStraightType.TileData = tileDataStraight;

            typeList.Add(wallInnerCornerType);
            typeList.Add(wallStraightType);

        }

        [Test]
        public void TestSerializeTileData()
        {
            TileData data = new TileData(Guid.NewGuid());
            TileFaceType type = new TileFaceType(Guid.NewGuid());
            type.FlipWinding = true;
            TileFaceType root = new TileFaceType(Guid.NewGuid());
            type.SetParent(root);

            data.Dimensions = new Vector3(1, 2, 3);
            data.SetFaceType(TileFace.Front, type);

            OBJParser.ObjImporter importer = new OBJParser.ObjImporter();
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());
            importer.AddMaterialFileStream("WallInnerCorner.mtl", new FileStream(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallInnerCorner.mtl", FileMode.Open));
            importer.ImportObjFile(TWDir.GameData.CreateSubdirectory("Core\\TileEngine") + "/TileSet001/WallInnerCorner.obj");
            meshWallInnerCorner = c.CreateMesh(importer);

            data.Mesh = meshWallInnerCorner;

            var simpleMeshFactory = new SimpleMeshFactory();
            simpleMeshFactory.AddMesh(meshWallInnerCorner);

            TileDataFactory tileDataFactory = new TileDataFactory(simpleMeshFactory,
                                                                  new SimpleTileFaceTypeFactory());

            FileStream stream = File.OpenWrite(TWDir.Test.CreateSubdirectory("TileEngine").FullName + "\\TestTileData.xml");
            tileDataFactory.SerializeTileData(data, stream);

            stream.Close();

            FileStream readStream =
                File.OpenRead(TWDir.Test.CreateSubdirectory("TileEngine").FullName + "\\TestTileData.xml");

            TileData readData = tileDataFactory.DeserializeTileData(readStream);

            readStream.Close();
        }

        [Test]
        public void TestSerializeWorldObject()
        {

        }
    }


}
