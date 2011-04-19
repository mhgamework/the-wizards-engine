using System.Collections.Generic;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Common.Core;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Tests.Physics
{
    [TestFixture()]
    public class PhysicsTest
    {
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestPhysicsDebugRenderer()
        {
            XNAGame game = new XNAGame();
            game.SpectaterCamera.CameraPosition = new Vector3(0, 0, -40);
            PhysicsEngine engine = new PhysicsEngine();

            //game.AddXNAObject(engine);

            engine.Initialize();

            PhysicsDebugRenderer debugRenderer = new PhysicsDebugRenderer(game, engine.Scene);
            game.AddXNAObject(debugRenderer);
            game.AddXNAObject(engine);
            InitTestScene(engine.Scene);


            /*game.InitializeEvent += delegate
                {



                };

            game.UpdateEvent += delegate
                {
                    engine.Update(game.Elapsed);
                };*/

            game.Run();

            engine.Dispose();

        }

        private void InitTestScene(StillDesign.PhysX.Scene scene)
        {

            ActorDescription actorDesc;
            Actor actor;

            CapsuleShapeDescription capsuleShapeDesc = new CapsuleShapeDescription(1, 3);

            actorDesc = new ActorDescription(capsuleShapeDesc);
            actorDesc.BodyDescription = new BodyDescription(1f);

            actor = scene.CreateActor(actorDesc);
            actor.GlobalOrientation = Matrix.CreateRotationX(MathHelper.PiOver2);



            BoxShapeDescription boxShapeDesc = new BoxShapeDescription(40, 1, 1);

            actorDesc = new ActorDescription(boxShapeDesc);
            actorDesc.BodyDescription = new BodyDescription(1f);

            actor = scene.CreateActor(actorDesc);
            actor.GlobalPosition = new Vector3(0, 4, 0);


            boxShapeDesc = new BoxShapeDescription(1, 1, 1);

            actorDesc = new ActorDescription(boxShapeDesc);
            actorDesc.BodyDescription = new BodyDescription(40f);

            actor = scene.CreateActor(actorDesc);
            actor.GlobalPosition = new Vector3(15, 40, 0);

            boxShapeDesc = new BoxShapeDescription(1, 1, 1);

            actorDesc = new ActorDescription(boxShapeDesc);
            actorDesc.BodyDescription = new BodyDescription(1f);

            actor = scene.CreateActor(actorDesc);
            actor.GlobalPosition = new Vector3(-13, 5, 0);

            boxShapeDesc = new BoxShapeDescription(1, 1, 1);

            actorDesc = new ActorDescription(boxShapeDesc);
            actorDesc.BodyDescription = new BodyDescription(1f);

            actor = scene.CreateActor(actorDesc);
            actor.GlobalPosition = new Vector3(-7, 5, 0);

            boxShapeDesc = new BoxShapeDescription(1, 1, 1);

            actorDesc = new ActorDescription(boxShapeDesc);
            actorDesc.BodyDescription = new BodyDescription(1f);

            actor = scene.CreateActor(actorDesc);
            actor.GlobalPosition = new Vector3(-10, 5, 0);

            boxShapeDesc = new BoxShapeDescription(1, 1, 1);

            actorDesc = new ActorDescription(boxShapeDesc);
            actorDesc.BodyDescription = new BodyDescription(1f);

            actor = scene.CreateActor(actorDesc);
            actor.GlobalPosition = new Vector3(-4, 5, 0);

        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestTriangleMesh()
        {
            XNAGame game = new XNAGame();
            game.SpectaterCamera.CameraPosition = new Vector3(0, 0, -40);
            PhysicsEngine engine = new PhysicsEngine();

            //game.AddXNAObject(engine);




            game.InitializeEvent += delegate
            {
                engine.Initialize();

                PhysicsDebugRenderer debugRenderer = new PhysicsDebugRenderer(game, engine.Scene);

                game.AddXNAObject(debugRenderer);

                TangentVertex[] vertices;
                short[] indices;
                BoxMesh.CreateUnitBoxVerticesAndIndices(out vertices, out indices);

                var positions = new Vector3[vertices.Length];
                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] = vertices[i].pos;
                }

                int[] intIndices = new int[indices.Length];
                for (int i = 0; i < intIndices.Length; i++)
                {
                    intIndices[i] = indices[i];
                }

                var triangleMesh = CreateTriangleMesh(positions, intIndices, engine.Scene);

                var triangleMeshShapeDesc = new TriangleMeshShapeDescription();
                triangleMeshShapeDesc.TriangleMesh = triangleMesh;

                var actorDesc = new ActorDescription(triangleMeshShapeDesc);

                var actor = engine.Scene.CreateActor(actorDesc);








            };

            game.UpdateEvent += delegate
            {
                engine.Update(game.Elapsed);
            };

            game.Run();

            engine.Dispose();

        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestJoint()
        {
            XNAGame game = new XNAGame();
            game.SpectaterCamera.CameraPosition = new Vector3(0, 0, -40);
            PhysicsEngine engine = new PhysicsEngine();

            //game.AddXNAObject(engine);




            game.InitializeEvent += delegate
            {
                engine.Initialize();

                PhysicsDebugRenderer debugRenderer = new PhysicsDebugRenderer(game, engine.Scene);

                game.AddXNAObject(debugRenderer);


                Actor actorA, actorB;
                {
                    BoxShapeDescription boxShapeDesc = new BoxShapeDescription(3, 3, 3);

                    BodyDescription bodyDesc = new BodyDescription(10.0f);
                    bodyDesc.BodyFlags |= BodyFlag.Kinematic;

                    ActorDescription actorDesc = new ActorDescription()
                    {
                        BodyDescription = bodyDesc,
                        GlobalPose = Matrix.CreateTranslation(70, 25, 65),
                        Shapes = { boxShapeDesc }
                    };
                    actorA = engine.Scene.CreateActor(actorDesc);
                }
                {
                    BoxShapeDescription boxShapeDesc = new BoxShapeDescription(3, 3, 3);

                    ActorDescription actorDesc = new ActorDescription()
                    {
                        BodyDescription = new BodyDescription(10.0f),
                        GlobalPose = Matrix.CreateTranslation(70, 15, 65),
                        Shapes = { boxShapeDesc }
                    };
                    actorB = engine.Scene.CreateActor(actorDesc);
                }

                PrismaticJointDescription prismaticJointDesc = new PrismaticJointDescription()
                {
                    Actor1 = actorA,
                    Actor2 = actorB,
                };
                prismaticJointDesc.SetGlobalAnchor(new Vector3(70, 20, 65));
                prismaticJointDesc.SetGlobalAxis(new Vector3(0, 1, 0));

                PrismaticJoint prismaticJoint = engine.Scene.CreateJoint(prismaticJointDesc) as PrismaticJoint;

                LimitPlane limitPlane = new LimitPlane(new Vector3(0, 1, 0), new Vector3(-30, 8, -30), 0);
                prismaticJoint.AddLimitPlane(limitPlane);




            };

            game.UpdateEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    Actor actor = PhysicsHelper.CreateDynamicSphereActor(engine.Scene, 1, 1);
                    actor.GlobalPosition = game.SpectaterCamera.CameraPosition +
                                           game.SpectaterCamera.CameraDirection * 5;
                    actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 5;
                }
                engine.Update(game.Elapsed);
            };

            game.Run();

            engine.Dispose();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestObjImportPhysics()
        {
            XNAGame game = new XNAGame();
            game.SpectaterCamera.CameraPosition = new Vector3(0, 0, -40);
            game.SpectaterCamera.FarClip = 10000;
            PhysicsEngine engine = new PhysicsEngine();

            //game.AddXNAObject(engine);




            game.InitializeEvent += delegate
            {
                engine.Initialize();

                PhysicsDebugRenderer debugRenderer = new PhysicsDebugRenderer(game, engine.Scene);

                game.AddXNAObject(debugRenderer);


                ObjImporter importer = new ObjImporter();
                //importer.ImportObjFile(EmbeddedFile.GetStream(typeof(ObjImporter).Assembly, "MHGameWork.TheWizards.OBJParser.Files.Crate.obj", "Crate.obj"));
                importer.ImportObjFile(TestFiles.MerchantsHouseObj);
                List<Vector3> positions = importer.Vertices;
                Vector3[] transformedPositions = new Vector3[positions.Count];
                Matrix objectMatrix = Matrix.CreateScale(5);
                Matrix transform = objectMatrix;
                Vector3.Transform(positions.ToArray(), ref transform, transformedPositions);
                var indices = new List<int>();

                for (int i = 0; i < importer.Groups.Count; i++)
                {
                    for (int j = 0; j < importer.Groups[i].SubObjects.Count; j++)
                    {
                        for (int k = 0; k < importer.Groups[i].SubObjects[j].Faces.Count; k++)
                        {
                            indices.Add(importer.Groups[i].SubObjects[j].Faces[k].V1.Position);
                            indices.Add(importer.Groups[i].SubObjects[j].Faces[k].V2.Position);
                            indices.Add(importer.Groups[i].SubObjects[j].Faces[k].V3.Position);
                        }
                    }
                }





                var triangleMesh = CreateTriangleMesh(transformedPositions, indices.ToArray(), engine.Scene);

                var triangleMeshShapeDesc = new TriangleMeshShapeDescription();
                triangleMeshShapeDesc.TriangleMesh = triangleMesh;

                var actorDesc = new ActorDescription(triangleMeshShapeDesc);

                var actor = engine.Scene.CreateActor(actorDesc);





            };

            game.UpdateEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    Actor actor = PhysicsHelper.CreateDynamicSphereActor(engine.Scene, 1, 1);
                    actor.GlobalPosition = game.SpectaterCamera.CameraPosition +
                                           game.SpectaterCamera.CameraDirection * 5;
                    actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 5;
                }
                engine.Update(game.Elapsed);
            };

            game.Run();

            engine.Dispose();
        }

        [Test]
        public void TestContactNotify()
        {
            XNAGame game = new XNAGame();
            game.SpectaterCamera.CameraPosition = new Vector3(0, 0, -40);
            PhysicsEngine engine = new PhysicsEngine();


            engine.AddContactNotification(delegate(ContactPair contactInformation, ContactPairFlag events)
                                              {
                                                  var pos = contactInformation.ActorA.GlobalPosition;
                                                  game.LineManager3D.AddLine(pos, pos + Vector3.Up * 5, Color.Yellow);
                                                  pos = contactInformation.ActorB.GlobalPosition;
                                                  game.LineManager3D.AddLine(pos, pos + Vector3.Up * 5, Color.Orange);
                                              });




            game.InitializeEvent += delegate
                                        {
                                            engine.Initialize();

                                            PhysicsDebugRenderer debugRenderer = new PhysicsDebugRenderer(game,
                                                                                                          engine.Scene);

                                            game.AddXNAObject(debugRenderer);


                                        };

            game.UpdateEvent += delegate
         {
             if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
             {
                 Actor actor = PhysicsHelper.CreateDynamicSphereActor(engine.Scene, 1, 1);
                 actor.GlobalPosition = game.SpectaterCamera.CameraPosition +
                                        game.SpectaterCamera.CameraDirection * 5;
                 actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 5;
                 actor.ContactReportFlags = ContactPairFlag.All;
                 //actor.ContactReportThreshold = 10000;
             }
             engine.Update(game.Elapsed);
         };

            game.Run();

            engine.Dispose();


        }

        [Test]
        public void TestTestSphereShooter()
        {
            var game = new XNAGame();
            game.IsFixedTimeStep = false;
            //game.DrawFps = true;

            PhysicsEngine engine = new PhysicsEngine();
            engine.Initialize();
            var debugRenderer = new PhysicsDebugRenderer(game, engine.Scene);
            game.AddXNAObject(debugRenderer);
            game.AddXNAObject(engine);


            ClientPhysicsQuadTreeNode root;

            int numNodes = 20;

            root = new ClientPhysicsQuadTreeNode(
                new BoundingBox(
                    new Vector3(-numNodes * numNodes / 2f, -100, -numNodes * numNodes / 2f),
                    new Vector3(numNodes * numNodes / 2f, 100, numNodes * numNodes / 2f)));

            QuadTree.Split(root, 5);


            var shooter = new TestSphereShooter(game, engine, root, game.SpectaterCamera);
            game.AddXNAObject(shooter);

            var visualizer = new QuadTreeVisualizer();
            game.DrawEvent += delegate
                                  {
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

            game.Run();

        }

        public static TriangleMesh CreateTriangleMesh(Vector3[] positions, int[] indices, StillDesign.PhysX.Scene scene)
        {

            TriangleMeshDescription triangleMeshDesc = new TriangleMeshDescription();

            triangleMeshDesc.AllocateVertices<Vector3>(positions.Length);
            triangleMeshDesc.AllocateTriangles<int>(indices.Length); // int indices, should be short but whatever



            triangleMeshDesc.VerticesStream.SetData(positions);


            triangleMeshDesc.TriangleStream.SetData(indices);

            triangleMeshDesc.VertexCount = positions.Length;
            triangleMeshDesc.TriangleCount = indices.Length / 3;

            System.IO.MemoryStream stream = new System.IO.MemoryStream();

            Cooking.InitializeCooking();
            Cooking.CookTriangleMesh(triangleMeshDesc, stream);
            Cooking.CloseCooking();

            stream.Position = 0;

            return scene.Core.CreateTriangleMesh(stream);
        }
    }
}
