using System.Collections.Generic;
using System.Windows.Forms;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.ServerClient.Terrain;
using MHGameWork.TheWizards.ServerClient.TWClient;
using MHGameWork.TheWizards.Terrain;
using MHGameWork.TheWizards.Terrain.Client;
using MHGameWork.TheWizards.Terrain.Editor;
using MHGameWork.TheWizards.Tests.Client;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using MHGameWork.TheWizards.ServerClient.Terrain.Rendering;
using MHGameWork.TheWizards.ServerClient.Database;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Tests.Terrain
{
    [TestFixture]
    public class TerrainClientTest
    {
        /*[Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestRenderTerrainGeomipmap()
        {

        }*/


        private Database.Database loadDatabaseServices()
        {
            Database.Database database = new Database.Database();
            database.AddService(new DiskSerializerService(database, Application.StartupPath + "\\WizardsEditorSave"));
            database.AddService(new DiskLoaderService(database));
            database.AddService(new MHGameWork.TheWizards.ServerClient.Database.SettingsService(database, System.Windows.Forms.Application.StartupPath + "\\Settings.xml"));
            database.AddService(new UniqueIDService(database));

            return database;
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestCreateTerrainHeightfieldActor()
        {
            XNAGame game = new XNAGame();
            Database.Database database = loadDatabaseServices();

            TerrainManagerService tms = new TerrainManagerService(database);
            TaggedTerrain taggedTerrain = tms.CreateTerrain();


            TerrainGeomipmapRenderData renderData = new TerrainGeomipmapRenderData(game, taggedTerrain, tms);


            PhysicsEngine engine = new PhysicsEngine();
            PhysicsDebugRenderer debugRenderer = null;
            TerrainFullData data = null;

            int blockX = 1, blockZ = 1;

            game.InitializeEvent += delegate
                {
                    engine.Initialize();
                    debugRenderer = new PhysicsDebugRenderer(game, engine.Scene);
                    debugRenderer.Initialize(game);

                    data = taggedTerrain.GetFullData();
                    data.NumBlocksX = 16;
                    data.NumBlocksZ = 16;
                    data.BlockSize = 16;
                    data.SizeX = data.NumBlocksX * data.BlockSize;
                    data.SizeZ = data.NumBlocksZ * data.BlockSize;
                    data.HeightMap = new HeightMap(data.NumBlocksX * data.BlockSize, data.NumBlocksZ * data.BlockSize);

                    float height = 3;

                    TerrainRaiseTool.RaiseTerrain(data, 16 + 8, 16 + 8, 8, height);

                    data.Position = new Vector3(3, 10, -5);

                    renderData.CreateBlocksAndQuadtree(new LoadingTask());

                    TerrainBlockHeightfieldBuilder builder = new TerrainBlockHeightfieldBuilder();
                    builder.BuildHeightfieldActor(engine.Scene, data, blockX, blockZ, height);
                };

            game.DrawEvent += delegate
                {
                    debugRenderer.Render(game);

                    Vector3 min;
                    min = data.Position + new Vector3(blockX * data.BlockSize, 0, blockZ * data.BlockSize);

                    game.LineManager3D.AddBox(
                        new BoundingBox(min + Vector3.Up * -5, min + Vector3.Up * 5 + new Vector3(data.BlockSize, 0, data.BlockSize))
                        , Color.Orange);

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
        public void TestTerrainBlockClientPhysics()
        {
            XNAGame game = new XNAGame();

            Database.Database database = loadDatabaseServices();


            TerrainManagerService tms = new TerrainManagerService(database);
            TaggedTerrain taggedTerrain = tms.CreateTerrain();


            TerrainGeomipmapRenderData renderData = new TerrainGeomipmapRenderData(game, taggedTerrain, tms);


            PhysicsEngine engine = new PhysicsEngine();
            PhysicsDebugRenderer debugRenderer = null;

            TheWizards.Client.ClientPhysicsQuadTreeNode root = ClientTest.CreateTestClientPhysicsQuadtree();


            ClientPhysicsTestSphere sphere = new ClientPhysicsTestSphere(Vector3.Zero, 2);

            Curve3D curve1 = ClientTest.CreateTestObject1MovementCurve();


            QuadTreeVisualizerXNA visualizer = new QuadTreeVisualizerXNA();
            float time = 0;
            TerrainBlockClientPhysics block1 = null;
            TerrainBlockClientPhysics block2 = null;

            game.InitializeEvent += delegate
                {
                    engine.Initialize();
                    debugRenderer = new PhysicsDebugRenderer(game, engine.Scene);
                    debugRenderer.Initialize(game);

                    TerrainFullData data = taggedTerrain.GetFullData();
                    data.NumBlocksX = 16;
                    data.NumBlocksZ = 16;
                    data.BlockSize = 16;
                    data.Position = new Vector3(data.BlockSize * -4, 0, data.BlockSize * -4);
                    data.HeightMap = new HeightMap(data.NumBlocksX * data.BlockSize, data.NumBlocksZ * data.BlockSize);

                    renderData.CreateBlocksAndQuadtree(new LoadingTask());

                    TerrainBlockHeightfieldBuilder builder = new TerrainBlockHeightfieldBuilder();

                    block1 = new TerrainBlockClientPhysics(engine.Scene, data, 4, 4, 10, builder);
                    root.OrdenObject(block1);
                    block2 = new TerrainBlockClientPhysics(engine.Scene, data, 1, 1, 10, builder);
                    root.OrdenObject(block2);

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

                    game.LineManager3D.AddBox(block1.GetBoundingBox(), Color.White);
                    game.LineManager3D.AddBox(block2.GetBoundingBox(), Color.White);
                    game.LineManager3D.AddCenteredBox(sphere.Center, sphere.Radius, Color.Red);

                };
            game.UpdateEvent += delegate
                {
                    time += game.Elapsed;


                    sphere.Move(root, curve1.Evaluate(time * (1 / 4f)));

                    engine.Update(game.Elapsed);
                };


            game.Run();
        }



        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestTerrainClientPhysics()
        {
            XNAGame game = new XNAGame();

            Database.Database database = loadDatabaseServices();
            TerrainManagerService tms = new TerrainManagerService(database);
            TaggedTerrain taggedTerrain = tms.CreateTerrain();


            TerrainGeomipmapRenderData renderData = new TerrainGeomipmapRenderData(game, taggedTerrain, tms);


            PhysicsEngine engine = new PhysicsEngine();
            PhysicsDebugRenderer debugRenderer = null;

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
            TerrainClientPhysics terrain = null;

            List<ClientPhysicsTestSphere> spheres = new List<ClientPhysicsTestSphere>();

            TerrainFullData data = null;

            game.InitializeEvent += delegate
                {
                    engine.Initialize(game);
                    debugRenderer = new PhysicsDebugRenderer(game, engine.Scene);
                    debugRenderer.Initialize(game);

                    data = taggedTerrain.GetFullData();
                    data.NumBlocksX = 16;
                    data.NumBlocksZ = 16;
                    data.BlockSize = 16;
                    data.SizeX = data.NumBlocksX * data.BlockSize;
                    data.SizeZ = data.NumBlocksZ * data.BlockSize;
                    data.Position = new Vector3(-data.BlockSize * (data.NumBlocksX / 2), 4, -data.BlockSize * (data.NumBlocksZ / 2));
                    data.HeightMap = new HeightMap(data.NumBlocksX * data.BlockSize, data.NumBlocksZ * data.BlockSize);

                    TerrainRaiseTool.RaiseTerrain(data, 0, 0, 100, 20);
                    TerrainRaiseTool.RaiseTerrain(data, 12, -5, 30, -15);
                    TerrainRaiseTool.RaiseTerrain(data, -50, 80, 12, 7);
                    TerrainRaiseTool.RaiseTerrain(data, 78, 112, 18, -5);
                    TerrainRaiseTool.RaiseTerrain(data, -7, 50, 50, 30);
                    TerrainRaiseTool.RaiseTerrain(data, 0, 0, 300, 10);

                    renderData.CreateBlocksAndQuadtree(new LoadingTask());

                    terrain = new TerrainClientPhysics(data);

                    terrain.LoadInClientPhysics(engine.Scene, root);




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



                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.V))
                    {

                        for (int ix = 0; ix < data.NumBlocksX; ix++)
                        {
                            for (int iz = 0; iz < data.NumBlocksZ; iz++)
                            {
                                ClientPhysicsTestSphere iSphere = new ClientPhysicsTestSphere(engine.Scene,
                         new Vector3(ix * data.BlockSize, 200, iz * data.BlockSize) + data.Position
                          , 1);

                                iSphere.InitDynamic();
                                iSphere.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 10;

                                spheres.Add(iSphere);

                            }
                        }
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
