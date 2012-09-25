using System.Collections.Generic;
using System.Windows.Forms;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Entity.Client;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Tests.Client;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using MHGameWork.TheWizards.ServerClient.Database;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;
using MHGameWork.TheWizards.Entities;

namespace MHGameWork.TheWizards.Tests.Entity
{
    [TestFixture]
    public class EntityClientTest
    {
        private Database.Database loadDatabaseServices()
        {
            Database.Database database = new Database.Database();
            database.AddService(new DiskSerializerService(database, Application.StartupPath + "\\WizardsEditorSave"));
            database.AddService(new DiskLoaderService(database));
            database.AddService(new MHGameWork.TheWizards.ServerClient.Database.SettingsService(database, System.Windows.Forms.Application.StartupPath + "\\Settings.xml"));
            database.AddService(new UniqueIDService(database));

            return database;
        }

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
            return positions;
        }

        private static EntityFullData CreatePyramidEntity(EntityManagerService ems, float size)
        {
            TaggedEntity ent = ems.CreateEntity();
            TaggedObject obj = ems.CreateObject();

            EntityFullData entityData = ent.GetTag<EntityFullData>();
            entityData.TaggedObject = obj;

            ModelFullData modelData = new ModelFullData();
            modelData.Positions = CreatePyramidPositions();
            modelData.ObjectMatrix = Matrix.CreateScale(size);
            modelData.CalculateBoundingBox();
            modelData.CalculateBoundingSphere();

            entityData.ObjectFullData.Models.Add(modelData);

            return entityData;
        }
        private static EntityFullData CreateTwoPyramidEntity(EntityManagerService ems, float size1, float size2)
        {
            TaggedEntity ent = ems.CreateEntity();
            TaggedObject obj = ems.CreateObject();

            EntityFullData entityData = ent.GetTag<EntityFullData>();
            entityData.TaggedObject = obj;

            ModelFullData modelData = new ModelFullData();
            modelData.Positions = CreatePyramidPositions();
            modelData.ObjectMatrix = Matrix.CreateScale(size1);
            modelData.CalculateBoundingBox();
            modelData.CalculateBoundingSphere();

            entityData.ObjectFullData.Models.Add(modelData);

            modelData = new ModelFullData();
            modelData.Positions = CreatePyramidPositions();
            modelData.ObjectMatrix = Matrix.CreateScale(size2);
            modelData.CalculateBoundingBox();
            modelData.CalculateBoundingSphere();

            entityData.ObjectFullData.Models.Add(modelData);


            return entityData;
        }

        /// <summary>
        /// TODO: remove old entity system.
        /// </summary>
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestCreateEntityActor()
        {
            XNAGame game = new XNAGame();
            Database.Database database = loadDatabaseServices();

            EntityManagerService ems = new EntityManagerService(database);


            BoundingBox boundingBox = new BoundingBox();

            PhysicsEngine engine = new PhysicsEngine();
            PhysicsDebugRendererXNA debugRenderer = null;




            game.InitializeEvent += delegate
                {
                    engine.Initialize();
                    debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);
                    debugRenderer.Initialize(game);


                    EntityFullData entityData = CreateTwoPyramidEntity(ems, 5, 3);

                    entityData.ObjectFullData.Models[0].ObjectMatrix *= Matrix.CreateTranslation(new Vector3(-3, 0, 3));
                    entityData.ObjectFullData.Models[1].ObjectMatrix *= Matrix.CreateTranslation(new Vector3(3, 1, 2));

                    entityData.Transform = new Transformation(
                        Vector3.One * 2,
                        Quaternion.Identity,
                        new Vector3(10, 10, 10));

                    EntityPhysicsActorBuilder builder = new EntityPhysicsActorBuilder();
                    builder.CreateActorForEntity(engine.Scene, entityData);

                    BoundingBox bb;
                    bb = entityData.ObjectFullData.Models[0].BoundingBox;
                    bb.Min = Vector3.Transform(bb.Min, entityData.ObjectFullData.Models[0].ObjectMatrix * entityData.Transform.CreateMatrix());
                    bb.Max = Vector3.Transform(bb.Max, entityData.ObjectFullData.Models[0].ObjectMatrix * entityData.Transform.CreateMatrix());

                    BoundingBox bb2;
                    bb2 = entityData.ObjectFullData.Models[1].BoundingBox;
                    bb2.Min = Vector3.Transform(bb2.Min, entityData.ObjectFullData.Models[1].ObjectMatrix * entityData.Transform.CreateMatrix());
                    bb2.Max = Vector3.Transform(bb2.Max, entityData.ObjectFullData.Models[1].ObjectMatrix * entityData.Transform.CreateMatrix());


                    boundingBox = BoundingBox.CreateMerged(bb, bb2);


                };

            game.DrawEvent += delegate
                {
                    debugRenderer.Render(game);


                    game.LineManager3D.AddBox(boundingBox, Color.Orange);

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

        /// <summary>
        /// TODO: remove old entity sytem
        /// </summary>
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestEntityClientPhysis()
        {
            XNAGame game = new XNAGame();

            Database.Database database = loadDatabaseServices();

            EntityManagerService ems = new EntityManagerService(database);

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


            QuadTreeVisualizerXNA visualizer = new QuadTreeVisualizerXNA();
            float time = 0;

            List<ClientPhysicsTestSphere> spheres = new List<ClientPhysicsTestSphere>();
            List<EntityClientPhysics> entities = new List<EntityClientPhysics>();


            game.InitializeEvent += delegate
                {
                    engine.Initialize();
                    debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);
                    debugRenderer.Initialize(game);

                    EntityFullData entityData;
                    EntityClientPhysics entPhysics;



                    entityData = CreatePyramidEntity(ems, 5);
                    entityData.Transform = new Transformation(
                        Vector3.One, Quaternion.Identity,
                        new Vector3(10, 2, 20));

                    entPhysics = new EntityClientPhysics(entityData);
                    entPhysics.LoadInClientPhysics(engine.Scene, root);
                    entities.Add(entPhysics);

                    entityData = CreatePyramidEntity(ems, 20);
                    entityData.Transform = new Transformation(
                       Vector3.One, Quaternion.Identity,
                       new Vector3(-32, 0, -40));

                    entPhysics = new EntityClientPhysics(entityData);
                    entPhysics.LoadInClientPhysics(engine.Scene, root);
                    entities.Add(entPhysics);


                    entityData = CreateTwoPyramidEntity(ems, 5, 3);
                    entityData.ObjectFullData.Models[0].ObjectMatrix *= Matrix.CreateTranslation(new Vector3(-3, 0, 3));
                    entityData.ObjectFullData.Models[1].ObjectMatrix *= Matrix.CreateTranslation(new Vector3(3, 1, 2));
                    entityData.Transform = new Transformation(
                       Vector3.One * 2, Quaternion.Identity,
                       new Vector3(80, 0, -45));

                    entPhysics = new EntityClientPhysics(entityData);
                    entPhysics.LoadInClientPhysics(engine.Scene, root);
                    entities.Add(entPhysics);



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

                    for (int i = 0; i < entities.Count; i++)
                    {
                        game.LineManager3D.AddCenteredBox(entities[i].BoundingSphere.Center,
                                                          entities[i].BoundingSphere.Radius * 2, Color.Black);
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


    }
}
