using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Client;

using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Features.Various.Client;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Tests.Features.Simulation.Physics
{
    [TestFixture]
    public class PhysicsMeshTest
    {

        /// <summary>
        /// Creates a vector array of positions representing faces for a simple 4-faced pyramid
        /// </summary>
        /// <returns></returns>
        private static Vector3[] CreatePyramidPositions()
        {
            Vector3[] positions = new Vector3[6 * 3];
            int i = 0;
            positions[i] = Vector3.UnitY; i++;
            positions[i] = Vector3.UnitZ; i++;
            positions[i] = Vector3.UnitX; i++;

            positions[i] = Vector3.UnitY; i++;
            positions[i] = Vector3.UnitX; i++;
            positions[i] = -Vector3.UnitZ; i++;

            positions[i] = Vector3.UnitY; i++;
            positions[i] = -Vector3.UnitZ; i++;
            positions[i] = -Vector3.UnitX; i++;

            positions[i] = Vector3.UnitY; i++;
            positions[i] = -Vector3.UnitX; i++;
            positions[i] = Vector3.UnitZ; i++;

            positions[i] = -Vector3.UnitZ; i++;
            positions[i] = Vector3.UnitX; i++;
            positions[i] = Vector3.UnitZ; i++;

            positions[i] = Vector3.UnitZ; i++;
            positions[i] = -Vector3.UnitX; i++;
            positions[i] = -Vector3.UnitZ; i++;
            for (int j = 0; j < positions.Length; j++)
            {
                positions[j] = positions[j] * 10;
            }
            return positions;
        }


        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestMeshPhysicsActorBuilderBoxes()
        {

            var mesh = new RAMMesh();
            var data = mesh.GetCollisionData();

            var box = new MeshCollisionData.Box();
            box.Dimensions = Vector3.One;
            box.Orientation = Matrix.Identity;

            data.Boxes.Add(box);


            box = new MeshCollisionData.Box();
            box.Dimensions = Vector3.One * 3;
            box.Orientation = Matrix.CreateTranslation(new Vector3(2, 2, 2));

            data.Boxes.Add(box);


            testMeshPhysicsActorBuilder(mesh);
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestMeshPhysicsActorBuilderTriangleMesh()
        {

            var mesh = new RAMMesh();
            var data = mesh.GetCollisionData();

            var triangleMesh = new MeshCollisionData.TriangleMeshData();
            Vector3[] pos = CreatePyramidPositions();
            Vector3[] transpos = new Vector3[pos.Length];
            var mat = Matrix.CreateTranslation(20, 5, 20);

            Vector3.Transform(pos, ref mat, transpos);
            triangleMesh.Positions = transpos;

            triangleMesh.Indices = new int[triangleMesh.Positions.Length];
            for (int i = 0; i < triangleMesh.Positions.Length; i++)
            {
                triangleMesh.Indices[i] = i;
            }

            data.TriangleMesh = triangleMesh;




            testMeshPhysicsActorBuilder(mesh);
        }


        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestMeshPhysicsActorBuilderConvex()
        {

            var mesh = new RAMMesh();
            var data = mesh.GetCollisionData();

            var convexMesh = new MeshCollisionData.Convex();
            convexMesh.Positions = new List<Vector3>();
            Vector3[] pos = CreatePyramidPositions();
            Vector3[] transpos = new Vector3[pos.Length];
            var mat = Matrix.CreateTranslation(20, 5, 20);

            Vector3.Transform(pos, ref mat, transpos);
            convexMesh.Positions.AddRange(transpos);

            data.ConvexMeshes.Add(convexMesh);


            testMeshPhysicsActorBuilder(mesh);
        }

        private void testMeshPhysicsActorBuilder(RAMMesh mesh)
        {
            XNAGame game = new XNAGame();
            BoundingBox boundingBox = new BoundingBox();

            PhysicsEngine engine = new PhysicsEngine();
            PhysicsDebugRendererXNA debugRenderer = null;

            Matrix mirrorMatrix = Matrix.CreateScale(-1, 1, 1);


            game.InitializeEvent += delegate
                                    {
                                        engine.Initialize();
                                        debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);
                                        debugRenderer.Initialize(game);


                                        var builder = new MeshPhysicsActorBuilder(new MeshPhysicsPool());
                                        builder.CreateActorStatic(engine.Scene, mesh.GetCollisionData(), Matrix.Identity);


                                        boundingBox = builder.CalculateBoundingBox(mesh.GetCollisionData());

                                        builder.CreateActorStatic(engine.Scene, mesh.GetCollisionData(), mirrorMatrix);



                                    };

            game.DrawEvent += delegate
                              {
                                  debugRenderer.Render(game);

                                  game.LineManager3D.WorldMatrix = Matrix.Identity;
                                  game.LineManager3D.AddBox(boundingBox, Color.Orange);
                                  game.LineManager3D.WorldMatrix = mirrorMatrix;
                                  game.LineManager3D.AddBox(boundingBox, Color.Yellow);
                                  game.LineManager3D.WorldMatrix = Matrix.Identity;

                              };
            game.UpdateEvent += delegate
                                {
                                    engine.Update(game.Elapsed);
                                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                                    {
                                        Actor actor = PhysicsHelper.CreateDynamicSphereActor(engine.Scene, 1, 1);
                                        actor.GlobalPosition = game.SpectaterCamera.CameraPosition +
                                                               game.SpectaterCamera.CameraDirection * 5;
                                        actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 5;
                                    }
                                };


            game.Run();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestPhysicsMeshPoolTriangleMesh()
        {
            XNAGame game = new XNAGame();

            var mesh = new RAMMesh();
            var data = mesh.GetCollisionData();

            var triangleMesh = new MeshCollisionData.TriangleMeshData();
            Vector3[] pos = CreatePyramidPositions();
            Vector3[] transpos = new Vector3[pos.Length];
            var mat = Matrix.CreateTranslation(20, 5, 20);

            Vector3.Transform(pos, ref mat, transpos);
            triangleMesh.Positions = transpos;
            triangleMesh.Indices = new int[triangleMesh.Positions.Length];
            for (int i = 0; i < triangleMesh.Positions.Length; i++)
            {
                triangleMesh.Indices[i] = i;
            }


            data.TriangleMesh = triangleMesh;

            PhysicsEngine engine = new PhysicsEngine();
            PhysicsDebugRendererXNA debugRenderer = null;




            game.InitializeEvent += delegate
            {
                engine.Initialize();
                debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);
                debugRenderer.Initialize(game);

                var pool = new MeshPhysicsPool();

                var tMesh1 = pool.CreateTriangleMesh(engine.Scene, mesh.GetCollisionData().TriangleMesh);
                var tMesh2 = pool.CreateTriangleMesh(engine.Scene, mesh.GetCollisionData().TriangleMesh);
                Assert.AreEqual(tMesh1, tMesh2);


                var actorDesc = new ActorDescription(new TriangleMeshShapeDescription() { TriangleMesh = tMesh1 });
                engine.Scene.CreateActor(actorDesc);

            };

            game.DrawEvent += delegate
            {
                debugRenderer.Render(game);


            };
            int frameNum = 0;
            game.UpdateEvent += delegate
            {
                engine.Update(game.Elapsed);
                //if (frameNum > 2) game.Exit();
                frameNum++;
            };


            game.Run();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestPhysicsMeshPoolTriangleMeshPreload()
        {
            XNAGame game = new XNAGame();

            RAMMesh mesh = createTriangleMesh();

            PhysicsEngine engine = new PhysicsEngine();

            MeshPhysicsPool pool = null;


            game.InitializeEvent += delegate
            {
                engine.Initialize();

                pool = new MeshPhysicsPool();
                pool.PreloadTriangleMesh(engine.Scene, mesh.GetCollisionData().TriangleMesh);





            };

            game.DrawEvent += delegate
            {

            };
            int frameNum = 0;
            game.UpdateEvent += delegate
            {

                System.Threading.Thread.Sleep(1000);
                pool.Update(engine.Scene);

                // This should finish instantly
                var tMesh1 = pool.CreateTriangleMesh(engine.Scene, mesh.GetCollisionData().TriangleMesh);

                engine.Update(game.Elapsed);
                if (frameNum > 2) game.Exit();
                frameNum++;
            };


            game.Run();
        }

        private RAMMesh createTriangleMesh()
        {
            var mesh = new RAMMesh();
            var data = mesh.GetCollisionData();

            var triangleMesh = new MeshCollisionData.TriangleMeshData();
            Vector3[] pos = CreatePyramidPositions();
            Vector3[] transpos = new Vector3[pos.Length];
            var mat = Matrix.CreateTranslation(20, 5, 20);

            Vector3.Transform(pos, ref mat, transpos);
            triangleMesh.Positions = transpos;
            triangleMesh.Indices = new int[triangleMesh.Positions.Length];
            for (int i = 0; i < triangleMesh.Positions.Length; i++)
            {
                triangleMesh.Indices[i] = i;
            }


            data.TriangleMesh = triangleMesh;
            return mesh;
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestPhysicsMeshPoolConvexMesh()
        {
            XNAGame game = new XNAGame();

            var mesh = new RAMMesh();
            var data = mesh.GetCollisionData();

            var convex = new MeshCollisionData.Convex();
            convex.Positions = new List<Vector3>();
            Vector3[] pos = CreatePyramidPositions();
            Vector3[] transpos = new Vector3[pos.Length];
            var mat = Matrix.CreateTranslation(20, 5, 20);

            Vector3.Transform(pos, ref mat, transpos);
            convex.Positions.AddRange(transpos);

            data.ConvexMeshes.Add(convex);

            PhysicsEngine engine = new PhysicsEngine();
            PhysicsDebugRendererXNA debugRenderer = null;




            game.InitializeEvent += delegate
            {
                engine.Initialize();
                debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);
                debugRenderer.Initialize(game);

                var pool = new MeshPhysicsPool();

                var tMesh1 = pool.CreateConvexMesh(engine.Scene, mesh.GetCollisionData().ConvexMeshes[0]);
                var tMesh2 = pool.CreateConvexMesh(engine.Scene, mesh.GetCollisionData().ConvexMeshes[0]);
                Assert.AreEqual(tMesh1, tMesh2);


                var actorDesc = new ActorDescription(new ConvexShapeDescription() { ConvexMesh = tMesh1 });
                engine.Scene.CreateActor(actorDesc);

            };

            game.DrawEvent += delegate
            {
                debugRenderer.Render(game);


            };
            int frameNum = 0;
            game.UpdateEvent += delegate
            {
                engine.Update(game.Elapsed);
                //if (frameNum > 2) game.Exit();
                frameNum++;
            };


            game.Run();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestMeshPhysicsElementFactoryStatic()
        {
            XNAGame game = new XNAGame();

            var mesh = new RAMMesh();
            var data = mesh.GetCollisionData();

            var box = new MeshCollisionData.Box();
            box.Dimensions = Vector3.One * 2;
            box.Orientation = Matrix.Identity;

            data.Boxes.Add(box);


            box = new MeshCollisionData.Box();
            box.Dimensions = Vector3.One * 4;
            box.Orientation = Matrix.CreateTranslation(new Vector3(2, 2, 2));

            data.Boxes.Add(box);


            BoundingBox bb = new BoundingBox();



            PhysicsEngine engine = new PhysicsEngine();
            PhysicsDebugRendererXNA debugRenderer = null;

            TheWizards.Client.ClientPhysicsQuadTreeNode root;
            root = new ClientPhysicsQuadTreeNode(
                new BoundingBox(
                    new Vector3(-16 * 16 / 2f, -100, -16 * 16 / 2f),
                    new Vector3(16 * 16 / 2f, 100, 16 * 16 / 2f)));

            QuadTree.Split(root, 4);


            ClientPhysicsTestSphere sphere = new ClientPhysicsTestSphere(Vector3.Zero, 2);

            Curve3D curve1 = ClientTest.CreateTestObject1MovementCurve();


            var visualizer = new QuadTreeVisualizerXNA();
            float time = 0;

            var spheres = new List<ClientPhysicsTestSphere>();
            var meshes = new List<MeshStaticPhysicsElement>();

            var physicsElementFactoryXNA = new MeshPhysicsFactoryXNA(engine, root);
            var factory = physicsElementFactoryXNA.Factory;
            game.AddXNAObject(physicsElementFactoryXNA);

            var el = factory.CreateStaticElement(mesh, Matrix.CreateTranslation(new Vector3(20, 0, 20)));
            meshes.Add(el);


            el = factory.CreateStaticElement(mesh, Matrix.CreateTranslation(new Vector3(-40, 0, 8)));
            meshes.Add(el);


            el = factory.CreateStaticElement(mesh, Matrix.CreateTranslation(new Vector3(80, 0, 70)));
            meshes.Add(el);

            game.InitializeEvent += delegate
                {
                    engine.Initialize();
                    debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);
                    debugRenderer.Initialize(game);

                };

            game.DrawEvent += delegate
                {
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
                    game.LineManager3D.AddCenteredBox(sphere.Center, sphere.Radius, Color.Red);

                    for (int i = 0; i < meshes.Count; i++)
                    {
                        game.LineManager3D.AddCenteredBox(meshes[i].BoundingSphere.Center,
                                                          meshes[i].BoundingSphere.Radius * 2, Color.Black);
                    }

                };
            game.UpdateEvent += delegate
                {
                    time += game.Elapsed;


                    sphere.Move(root, curve1.Evaluate(time * (1 / 4f)));


                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                    {
                        ClientPhysicsTestSphere iSphere = new ClientPhysicsTestSphere(engine.Scene,
                            game.SpectaterCamera.CameraPosition + game.SpectaterCamera.CameraDirection
                            , 1);

                        iSphere.InitDynamic();
                        iSphere.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 10;

                        spheres.Add(iSphere);
                    }



                    for (int i = 0; i < spheres.Count; i++)
                    {
                        spheres[i].Update(root, game);
                    }



                    engine.Update(game.Elapsed);
                };


            game.Run();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestMeshPhysicsElementFactoryDynamic()
        {
            XNAGame game = new XNAGame();

            RAMMesh mesh = createTwoBoxMesh();


            BoundingBox bb = new BoundingBox();



            PhysicsEngine engine = new PhysicsEngine();
            PhysicsDebugRendererXNA debugRenderer = null;

            ClientPhysicsQuadTreeNode root = CreatePhysicsQuadtree(16, 4);



            var visualizer = new QuadTreeVisualizerXNA();

            var meshes = new List<MeshDynamicPhysicsElement>();

            var physicsElementFactoryXNA = new MeshPhysicsFactoryXNA(engine, root);
            var factory = physicsElementFactoryXNA.Factory;
            game.AddXNAObject(physicsElementFactoryXNA);

            game.InitializeEvent += delegate
            {
                engine.Initialize();
                debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);
                debugRenderer.Initialize(game);

            };

            game.DrawEvent += delegate
            {
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

                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    var el = factory.CreateDynamicElement(mesh,
                                                 Matrix.CreateTranslation(game.SpectaterCamera.CameraPosition +
                                                                          game.SpectaterCamera.CameraDirection));
                    el.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 10;


                    meshes.Add(el);
                }

                engine.Update(game.Elapsed);
            };


            game.Run();
        }

        public static ClientPhysicsQuadTreeNode CreatePhysicsQuadtree(int numNodes, int numSplits)
        {
            TheWizards.Client.ClientPhysicsQuadTreeNode root;
            root = new ClientPhysicsQuadTreeNode(
                new BoundingBox(
                    new Vector3(-numNodes * numNodes / 2f, -100, -numNodes * numNodes / 2f),
                    new Vector3(numNodes * numNodes / 2f, 100, numNodes * numNodes / 2f)));

            QuadTree.Split(root, numSplits);
            return root;
        }

        private RAMMesh createTwoBoxMesh()
        {
            var mesh = new RAMMesh();
            var data = mesh.GetCollisionData();

            var box = new MeshCollisionData.Box();
            box.Dimensions = Vector3.One * 2;
            box.Orientation = Matrix.Identity;

            data.Boxes.Add(box);


            box = new MeshCollisionData.Box();
            box.Dimensions = Vector3.One * 4;
            box.Orientation = Matrix.CreateTranslation(new Vector3(2, 2, 2));

            data.Boxes.Add(box);
            return mesh;
        }


        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestMeshDynamicPhysicsElement()
        {
            XNAGame game = new XNAGame();

            var mesh = new RAMMesh();
            var data = mesh.GetCollisionData();

            var box = new MeshCollisionData.Box();
            box.Dimensions = Vector3.One * 2;
            box.Orientation = Matrix.Identity;

            data.Boxes.Add(box);


            box = new MeshCollisionData.Box();
            box.Dimensions = Vector3.One * 4;
            box.Orientation = Matrix.CreateTranslation(new Vector3(2, 2, 2));

            data.Boxes.Add(box);


            BoundingBox bb = new BoundingBox();



            PhysicsEngine engine = new PhysicsEngine();
            PhysicsDebugRendererXNA debugRenderer = null;

            TheWizards.Client.ClientPhysicsQuadTreeNode root;
            root = new ClientPhysicsQuadTreeNode(
                new BoundingBox(
                    new Vector3(-16 * 16 / 2f, -100, -16 * 16 / 2f),
                    new Vector3(16 * 16 / 2f, 100, 16 * 16 / 2f)));

            QuadTree.Split(root, 4);


            var builder = new MeshPhysicsActorBuilder(new MeshPhysicsPool());
            var visualizer = new QuadTreeVisualizerXNA();
            float time = 0;

            var spheres = new List<MeshDynamicPhysicsElement>();
            var meshes = new List<MeshStaticPhysicsElement>();

            var physicsElementFactoryXNA = new MeshPhysicsFactoryXNA(engine, root);
            var factory = physicsElementFactoryXNA.Factory;
            game.AddXNAObject(physicsElementFactoryXNA);


            game.InitializeEvent += delegate
            {
                engine.Initialize();
                debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);
                debugRenderer.Initialize(game);

            };

            game.DrawEvent += delegate
            {
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

                for (int i = 0; i < meshes.Count; i++)
                {
                    game.LineManager3D.AddCenteredBox(meshes[i].BoundingSphere.Center,
                                                      meshes[i].BoundingSphere.Radius * 2, Color.Black);
                }

            };
            game.UpdateEvent += delegate
            {
                time += game.Elapsed;




                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    var dEl = new MeshDynamicPhysicsElement(mesh,
                                                            Matrix.CreateTranslation(
                                                                game.SpectaterCamera.CameraPosition +
                                                                game.SpectaterCamera.CameraDirection), builder);

                    dEl.InitDynamic(engine.Scene);
                    dEl.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 10;

                    spheres.Add(dEl);
                }



                for (int i = 0; i < spheres.Count; i++)
                {
                    spheres[i].Update(root);
                }



                engine.Update(game.Elapsed);
            };


            game.Run();
        }

        [Test]
        public void TestMoveStaticMeshPhysicsElement()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TestRemoveMeshStaticPhysicsElement()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TestRemoveMeshDynamicPhysicsElement()
        {
            throw new NotImplementedException();
        }


    }
}
