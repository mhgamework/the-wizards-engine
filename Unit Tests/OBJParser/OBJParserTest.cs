using System.IO;
using System.Threading;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Common.Core;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Entity.Client;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;
using StillDesign.PhysX;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.Tests.OBJParser
{
    [TestFixture]
    public class OBJParserTest
    {
        [Test]
        public void TestImportMaterials()
        {

            ObjImporter importer = new ObjImporter();


            var fsMat = new FileStream(@"..\GameData\Crate01.mtl", FileMode.Open);

            importer.AddMaterialFileStream("Crate01.mtl", fsMat);
            importer.ImportObjFile(@"..\GameData\Crate01.obj");

            var materials = importer.Materials;

            fsMat.Close();

            //TODO: check the values
        }

        /// <summary>
        /// Imports OBJ files and renders the geometry as wireframe
        /// 
        /// </summary>
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestImportGeometry()
        {
            XNAGame game = new XNAGame();

            ObjImporter importer = new ObjImporter();
            bool dirty = true;
            Seeder seeder = new Seeder(489);
            LineManager3DLines lines = null;


            var fsCrate = new FileStream(@"..\GameData\Crate01.obj", FileMode.Open);
            var fsMHouse = new FileStream(@"..\GameData\MerchantsHouse.obj", FileMode.Open);

            game.UpdateEvent += delegate
                                {
                                    if (game.Keyboard.IsKeyPressed(Keys.C) || dirty)
                                    {

                                        lines = new LineManager3DLines();
                                        importer.ImportObjFile(fsCrate);

                                        renderWireframe(importer, lines, Color.Black, Vector3.Zero);
                                        renderObjects(importer, lines, seeder, Vector3.Right * 5);
                                        renderSubObjects(importer, lines, seeder, Vector3.Right * 10);

                                        importer.ImportObjFile(fsMHouse);

                                        renderWireframe(importer, lines, Color.Black, Vector3.Forward * 30);
                                        renderObjects(importer, lines, seeder, Vector3.Right * 30 + Vector3.Forward * 30);
                                        renderSubObjects(importer, lines, seeder, Vector3.Right * 60 + Vector3.Forward * 30);
                                        dirty = false;
                                    }
                                };

            game.DrawEvent += delegate
                              {
                                  game.LineManager3D.Render(lines);
                              };

            game.Run();

            fsCrate.Close();
            fsMHouse.Close();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestOBJToRAMMeshConverter()
        {
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());


            var fsMat = new FileStream(@"..\GameData\Crate01.mtl", FileMode.Open);


            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Crate01.mtl", fsMat);
            importer.ImportObjFile(@"..\GameData\Crate01.obj");


            fsMat.Close();



            var mesh = c.CreateMesh(importer);

            importer = new ObjImporter();
            importer.AddMaterialFileStream("CollisionModelBoxes001.mtl",
                                           EmbeddedFile.GetStream("MHGameWork.TheWizards.Tests.Physics.Files.CollisionModelBoxes001.mtl",
                                                                  "CollisionModelBoxes001.mtl"));
            importer.ImportObjFile(EmbeddedFile.GetStream("MHGameWork.TheWizards.Tests.Physics.Files.CollisionModelBoxes001.obj", "CollisionModelBoxes001.obj"));

            mesh = c.CreateMesh(importer);



        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestOBJToRAMMeshConverterVisual()
        {

            PhysicsEngine engine = new PhysicsEngine();
            PhysicsDebugRenderer debugRenderer = null;




            var texFactory = new RAMTextureFactory();
            var c = new OBJToRAMMeshConverter(texFactory);


            var importer = new ObjImporter();
            importer.AddMaterialFileStream("CollisionModelBoxes001.mtl",
                                           EmbeddedFile.GetStream("MHGameWork.TheWizards.Tests.Physics.Files.CollisionModelBoxes001.mtl",
                                                                  "CollisionModelBoxes001.mtl"));
            importer.ImportObjFile(EmbeddedFile.GetStream("MHGameWork.TheWizards.Tests.Physics.Files.CollisionModelBoxes001.obj", "CollisionModelBoxes001.obj"));


            var mesh = c.CreateMesh(importer);

            importer = new ObjImporter();
            importer.AddMaterialFileStream("HouseTest.mtl", new FileStream("../GameData/001-House01_BoxTest-OBJ/HouseTest.mtl", FileMode.Open));
            importer.ImportObjFile("../GameData/001-House01_BoxTest-OBJ/HouseTest.obj");

            var mesh2 = c.CreateMesh(importer);

            importer = new ObjImporter();
            var fsMHouseMat = new FileStream(@"..\GameData\MerchantsHouse.mtl", FileMode.Open);

            importer.AddMaterialFileStream("MerchantsHouse.mtl", fsMHouseMat);
            importer.ImportObjFile(@"..\GameData\MerchantsHouse.obj");

            fsMHouseMat.Close();

            var mesh3 = c.CreateMesh(importer);

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);

            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);


            var el = renderer.AddMesh(mesh);
            el.WorldMatrix = Matrix.CreateTranslation(Vector3.Right * 0 * 2 + Vector3.UnitZ * 0 * 2);

            el = renderer.AddMesh(mesh2);
            el.WorldMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 80));

            el = renderer.AddMesh(mesh3);
            el.WorldMatrix = Matrix.CreateTranslation(new Vector3(0, 0, -80));



            var game = new XNAGame();
            game.IsFixedTimeStep = false;
            game.DrawFps = true;

            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);



            game.InitializeEvent += delegate
            {
                engine.Initialize();
                debugRenderer = new PhysicsDebugRenderer(game, engine.Scene);
                debugRenderer.Initialize(game);


                var builder = new MeshPhysicsActorBuilder(new MeshPhysicsPool());
                builder.CreateActorStatic(engine.Scene, mesh.GetCollisionData(), Matrix.Identity);
                builder.CreateActorStatic(engine.Scene, mesh2.GetCollisionData(), Matrix.CreateTranslation(new Vector3(0, 0, 80)));
                builder.CreateActorStatic(engine.Scene, mesh3.GetCollisionData(), Matrix.CreateTranslation(new Vector3(0, 0, -80)));



            };

            game.DrawEvent += delegate
            {
                debugRenderer.Render(game);

            };
            game.UpdateEvent += delegate
            {
                engine.Update(game.Elapsed);
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    Actor actor = PhysicsHelper.CreateDynamicSphereActor(engine.Scene, 0.3f, 1);
                    actor.GlobalPosition = game.SpectaterCamera.CameraPosition +
                                           game.SpectaterCamera.CameraDirection * 5;
                    actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 5;
                }
            };



            game.Run();

        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestOBJToRAMMeshConverterPerObjectVisual()
        {
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());


            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Town001.mtl", new FileStream(TestFiles.TownMtl , FileMode.Open));
            importer.ImportObjFile(TestFiles.TownObj);

            var meshes = c.CreateMeshesFromObjects(importer);

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);

            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);





            var spheres = new List<ClientPhysicsTestSphere>();
            var engine = new PhysicsEngine();
            PhysicsDebugRenderer debugRenderer = null;

            var builder = new MeshPhysicsActorBuilder(new MeshPhysicsPool());

            TheWizards.Client.ClientPhysicsQuadTreeNode root;

            int numNodes = 20;

            root = new ClientPhysicsQuadTreeNode(
                new BoundingBox(
                    new Vector3(-numNodes * numNodes / 2f, -100, -numNodes * numNodes / 2f),
                    new Vector3(numNodes * numNodes / 2f, 100, numNodes * numNodes / 2f)));

            QuadTree.Split(root, 5);

            var physicsElementFactory = new MeshPhysicsElementFactory(engine, root);

            var physicsElements = new List<MeshStaticPhysicsElement>();
            for (int i = 0; i < 0 * 100 + 1 * meshes.Count; i++)
            {
                var mesh = meshes[i];
                var el = renderer.AddMesh(mesh);
                el.WorldMatrix = Matrix.CreateTranslation(Vector3.Right * 0 * 2 + Vector3.UnitZ * 0 * 2);

                var pEl = physicsElementFactory.CreateStaticElement(mesh, Matrix.Identity);
                physicsElements.Add(pEl);

            }

            var game = new XNAGame();
            game.IsFixedTimeStep = false;
            game.DrawFps = true;
            game.SpectaterCamera.FarClip = 5000;
            game.Graphics1.PreparingDeviceSettings += delegate(object sender, PreparingDeviceSettingsEventArgs e)
            {
                DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
                e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = displayMode.Format;
                e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = displayMode.Width;
                e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = displayMode.Height;
                game.SpectaterCamera.AspectRatio = displayMode.Width / (float)displayMode.Height;

            };
            game.Graphics1.ToggleFullScreen();

            var sphereMesh = new SphereMesh(0.3f, 20, Color.Green);
            var visualizer = new QuadTreeVisualizerXNA();

            game.AddXNAObject(physicsElementFactory);

            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);


            game.InitializeEvent += delegate
                                    {
                                        engine.Initialize();
                                        debugRenderer = new PhysicsDebugRenderer(game, engine.Scene);
                                        debugRenderer.Initialize(game);
                                        sphereMesh.Initialize(game);



                                    };

            bool showPhysics = true;
            game.DrawEvent += delegate
                              {
                                  if (game.Keyboard.IsKeyPressed(Keys.P))
                                      showPhysics = !showPhysics;
                                  if (showPhysics)
                                      debugRenderer.Render(game);
                                  visualizer.RenderNodeGroundBoundig(game, root,
                                      delegate(ClientPhysicsQuadTreeNode node, out Color col)
                                      {
                                          col = Color.Green;

                                          return node.PhysicsObjects.Count == 0;
                                      });

                                  visualizer.RenderNodeGroundBoundig(game, root,
                                     delegate(ClientPhysicsQuadTreeNode node, out Color col)
                                     {
                                         col = Color.Orange;

                                         return node.PhysicsObjects.Count > 0;
                                     });

                                  for (int i = 0; i < physicsElements.Count; i++)
                                  {
                                      var el = physicsElements[i];
                                      //game.LineManager3D.AddBox(BoundingBox.CreateFromSphere( el.BoundingSphere), Color.Orange);
                                  }
                                  for (int i = 0; i < spheres.Count; i++)
                                  {
                                      sphereMesh.WorldMatrix = Matrix.CreateTranslation(spheres[i].Center);
                                      sphereMesh.Render(game);
                                  }

                              };
            game.UpdateEvent += delegate
            {
                engine.Update(game.Elapsed);
                sphereMesh.Update(game);
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    var iSphere = new ClientPhysicsTestSphere(engine.Scene,
                        game.SpectaterCamera.CameraPosition + game.SpectaterCamera.CameraDirection
                        , 0.3f);

                    iSphere.InitDynamic();
                    iSphere.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 10;

                    spheres.Add(iSphere);
                }



                for (int i = 0; i < spheres.Count; i++)
                {
                    spheres[i].Update(root, game);
                }

            };

            game.Run();

        }

       
        
        public static RAMMesh GetBarrelMesh(OBJToRAMMeshConverter c)
        {
            var fsMat = new FileStream(TestFiles.BarrelMtl, FileMode.Open);

            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Barrel01.mtl", fsMat);

            importer.ImportObjFile(TestFiles.BarrelObj);

            var meshes = c.CreateMeshesFromObjects(importer);

            fsMat.Close();

            return meshes[0];
        }
        public static RAMMesh GetCrateMesh(OBJToRAMMeshConverter c)
        {
            var fsMat = new FileStream(TestFiles.CrateMtl, FileMode.Open);

            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Crate01.mtl",fsMat);


            importer.ImportObjFile(TestFiles.CrateObj);

            var meshes = c.CreateMeshesFromObjects(importer);
            fsMat.Close();
            return meshes[0];
        }

        public class TestGameMesh
        {
            public SimpleMeshRenderElement RenderElement;
            public MeshDynamicPhysicsElement PhysicsElement;

            public TestGameMesh(SimpleMeshRenderElement renderElement, MeshDynamicPhysicsElement physicsElement)
            {
                RenderElement = renderElement;
                PhysicsElement = physicsElement;
            }
        }

        private void renderSubObjects(ObjImporter importer, LineManager3DLines lines, Seeder seeder, Vector3 offset)
        {
            Color col;
            for (int i = 0; i < importer.Groups.Count; i++)
            {
                for (int j = 0; j < importer.Groups[i].SubObjects.Count; j++)
                {
                    col = seeder.NextColor();
                    for (int k = 0; k < importer.Groups[i].SubObjects[j].Faces.Count; k++)
                    {
                        lines.AddTriangle(
                            importer.Vertices[
                                importer.Groups[i].SubObjects[j].Faces[k].V1.Position] + offset,
                            importer.Vertices[
                                importer.Groups[i].SubObjects[j].Faces[k].V2.Position] + offset,
                            importer.Vertices[
                                importer.Groups[i].SubObjects[j].Faces[k].V3.Position] + offset,
                            col);

                    }
                }
            }
        }

        private void renderObjects(ObjImporter importer, LineManager3DLines lines, Seeder seeder, Vector3 offset)
        {
            Color col;
            for (int i = 0; i < importer.Groups.Count; i++)
            {
                col = seeder.NextColor();
                for (int j = 0; j < importer.Groups[i].SubObjects.Count; j++)
                {
                    for (int k = 0; k < importer.Groups[i].SubObjects[j].Faces.Count; k++)
                    {
                        lines.AddTriangle(
                            importer.Vertices[
                                importer.Groups[i].SubObjects[j].Faces[k].V1.Position] + offset,
                            importer.Vertices[
                                importer.Groups[i].SubObjects[j].Faces[k].V2.Position] + offset,
                            importer.Vertices[
                                importer.Groups[i].SubObjects[j].Faces[k].V3.Position] + offset,
                            col);

                    }
                }
            }
        }

        private void renderWireframe(ObjImporter importer, LineManager3DLines lines, Color col, Vector3 offset)
        {
            for (int i = 0; i < importer.Groups.Count; i++)
            {
                for (int j = 0; j < importer.Groups[i].SubObjects.Count; j++)
                {
                    for (int k = 0; k < importer.Groups[i].SubObjects[j].Faces.Count; k++)
                    {
                        lines.AddTriangle(
                            importer.Vertices[
                                importer.Groups[i].SubObjects[j].Faces[k].V1.Position] + offset,
                            importer.Vertices[
                                importer.Groups[i].SubObjects[j].Faces[k].V2.Position] + offset,
                            importer.Vertices[
                                importer.Groups[i].SubObjects[j].Faces[k].V3.Position] + offset,
                            col);

                    }
                }
            }
        }
    }
}
