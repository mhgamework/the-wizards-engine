using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.OBJParser;
using MHGameWork.TheWizards.Tests.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests
{
    [TestFixture]
    public class Showcase
    {
        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestOBJToRAMMeshConverterPerObjectVisualCool()
        {
            var textureFactory = new RAMTextureFactory();
            var c = new OBJToRAMMeshConverter(textureFactory);


            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Town001.mtl", new FileStream("../GameData/Town/OBJ03/Town001.mtl", FileMode.Open));
            importer.ImportObjFile("../GameData/Town/OBJ03/Town001.obj");

            var meshes = c.CreateMeshesFromObjects(importer);

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new MeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);

            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);





            var spheres = new List<ClientPhysicsTestSphere>();
            var engine = new PhysicsEngine();
            PhysicsDebugRenderer debugRenderer = null;

            var root = PhysicsMeshTest.CreatePhysicsQuadtree(20, 5);

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
            var gameMeshes = new List<OBJParserTest.TestGameMesh>();

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

            var barrelMesh = OBJParserTest.GetBarrelMesh(c);
            var crateMesh = OBJParserTest.GetCrateMesh(c);

            var sphereMesh = new SphereMesh(0.3f, 20, Color.Green);
            var visualizer = new QuadTreeVisualizer();

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

                for (int i = 0; i < meshes.Count; i++)
                {
                    var mesh = meshes[i];
                    var data = mesh.GetCollisionData();
                    /*if (data.TriangleMesh != null)
                        physicsElementFactory.MeshPhysicsPool.PreloadTriangleMesh(engine.Scene, data.TriangleMesh);*/
                }



            };

            bool showPhysics = true;
            game.DrawEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Keys.P))
                    showPhysics = !showPhysics;
                if (showPhysics)
                    debugRenderer.Render(game);
                /*visualizer.RenderNodeGroundBoundig(game, root,
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
                   });*/

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
                    var pEl = physicsElementFactory.CreateDynamicElement(crateMesh,
                                                                         Matrix.CreateTranslation(
                                                                             game.SpectaterCamera.CameraPosition +
                                                                             game.SpectaterCamera.CameraDirection));
                    pEl.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 30;

                    var rEl = renderer.AddMesh(crateMesh);


                    gameMeshes.Add(new OBJParserTest.TestGameMesh(rEl, pEl));
                }
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.E))
                {
                    var pEl = physicsElementFactory.CreateDynamicElement(barrelMesh,
                                                                         Matrix.CreateTranslation(
                                                                             game.SpectaterCamera.CameraPosition +
                                                                             game.SpectaterCamera.CameraDirection));
                    pEl.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 30;

                    var rEl = renderer.AddMesh(barrelMesh);


                    gameMeshes.Add(new OBJParserTest.TestGameMesh(rEl, pEl));
                }


                for (int i = 0; i < gameMeshes.Count; i++)
                {
                    var m = gameMeshes[i];
                    m.RenderElement.WorldMatrix = m.PhysicsElement.World;
                }

            };

            game.Run();

        }


        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestThrowCrates()
        {
            var texFactory = new RAMTextureFactory();
            var c = new OBJToRAMMeshConverter(texFactory);


            RAMMesh mesh = OBJParserTest.GetCrateMesh(c);

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new MeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);
            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);





            var gameMeshes = new List<OBJParserTest.TestGameMesh>();
            var engine = new PhysicsEngine();
            PhysicsDebugRenderer debugRenderer = null;

            var root = PhysicsMeshTest.CreatePhysicsQuadtree(16, 4);

            var physicsElementFactory = new MeshPhysicsElementFactory(engine, root);

            var game = new XNAGame();
            game.IsFixedTimeStep = false;
            game.DrawFps = true;

            var visualizer = new QuadTreeVisualizer();

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


            };
            game.UpdateEvent += delegate
            {
                engine.Update(game.Elapsed);
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    var pEl = physicsElementFactory.CreateDynamicElement(mesh,
                                                                         Matrix.CreateTranslation(
                                                                             game.SpectaterCamera.CameraPosition +
                                                                             game.SpectaterCamera.CameraDirection));
                    pEl.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 30;

                    var rEl = renderer.AddMesh(mesh);


                    gameMeshes.Add(new OBJParserTest.TestGameMesh(rEl, pEl));
                }

                for (int i = 0; i < gameMeshes.Count; i++)
                {
                    var m = gameMeshes[i];
                    m.RenderElement.WorldMatrix = m.PhysicsElement.World;
                }

            };

            game.Run();

        }
        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestThrowBarrels()
        {
            var texFactory = new RAMTextureFactory();
            var c = new OBJToRAMMeshConverter(texFactory);


            RAMMesh mesh = OBJParserTest.GetBarrelMesh(c);

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new MeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);
            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);





            var gameMeshes = new List<OBJParserTest.TestGameMesh>();
            var engine = new PhysicsEngine();
            PhysicsDebugRenderer debugRenderer = null;

            var root = PhysicsMeshTest.CreatePhysicsQuadtree(16, 4);

            var physicsElementFactory = new MeshPhysicsElementFactory(engine, root);

            var game = new XNAGame();
            game.IsFixedTimeStep = false;
            game.DrawFps = true;

            var visualizer = new QuadTreeVisualizer();

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


            };
            game.UpdateEvent += delegate
            {
                engine.Update(game.Elapsed);
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    var pEl = physicsElementFactory.CreateDynamicElement(mesh,
                                                                         Matrix.CreateTranslation(
                                                                             game.SpectaterCamera.CameraPosition +
                                                                             game.SpectaterCamera.CameraDirection));
                    pEl.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 30;

                    var rEl = renderer.AddMesh(mesh);


                    gameMeshes.Add(new OBJParserTest.TestGameMesh(rEl, pEl));
                }

                for (int i = 0; i < gameMeshes.Count; i++)
                {
                    var m = gameMeshes[i];
                    m.RenderElement.WorldMatrix = m.PhysicsElement.World;
                }

            };

            game.Run();

        }

    }
}
