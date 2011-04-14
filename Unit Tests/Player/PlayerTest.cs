using System;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Player.Client;
using MHGameWork.TheWizards.Tests.Client;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.TheWizards.Tests.Player
{
    [TestFixture]
    public class PlayerTest
    {
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestPlayerControllerPhysX()
        {
            throw new Exception("This is Deprecated");
            Database.Database db = new MHGameWork.TheWizards.Database.Database();

            ClientXNAGameService cg = new MHGameWork.TheWizards.Client.ClientXNAGameService(db);

            using (PhysicsEngine engine = new PhysicsEngine())
            {




                PlayerService playerService = new PlayerService(db);

                PlayerData player = new PlayerData();
                player.Name = "MHGameWork";
                playerService.AddPlayer(player);

                PlayerController controller = null;

                cg.XNAGame.SpectaterCamera.CameraDirection = Vector3.Normalize(new Vector3(1.3f, -2, 1));
                cg.XNAGame.SpectaterCamera.CameraPosition = new Vector3(10, 10, -10);

                cg.XNAGame.SpectaterCamera.FitInView(new BoundingSphere(new Vector3(0, 0, -2), 4));



                cg.XNAGame.InitializeEvent +=
                    delegate
                    {
                        engine.Initialize(cg.XNAGame);
                        PhysicsDebugRenderer debugRenderer =
                            new PhysicsDebugRenderer(cg.XNAGame, engine.Scene);

                        cg.XNAGame.AddXNAObject(debugRenderer);

                        controller = new PlayerController(cg.XNAGame, player, engine.Scene);

                    };

                cg.XNAGame.UpdateEvent +=
                    delegate
                    {
                        engine.Update(cg.XNAGame);
                    };

                XNATaskQueue queue = new XNATaskQueue(cg.XNAGame);
                cg.XNAGame.AddXNAObject(queue);

                queue.AddUpdateTask(1,
                    delegate(float elapsed)
                    {

                    });
                queue.AddUpdateTask(3,
                    delegate(float elapsed)
                    {
                        controller.DoMoveForward(elapsed);
                    });
                queue.AddUpdateTask(2,
                    delegate(float elapsed)
                    {
                        controller.DoStrafeRight(elapsed);
                    });
                queue.AddUpdateTask(2,
                    delegate(float elapsed)
                    {
                        controller.DoMoveBackwards(elapsed);
                    });
                queue.AddUpdateTask(3,
                    delegate(float elapsed)
                    {
                        controller.DoStrafeLeft(elapsed);
                    });
                queue.AddUpdateTask(3,
                    delegate(float elapsed)
                    {
                        controller.DoRotateHorizontal(1 * elapsed);
                    });
                queue.AddUpdateTask(0,
                    delegate(float elapsed)
                    {
                        cg.XNAGame.Exit();
                    });

                cg.XNAGame.DrawEvent +=
                    delegate
                    {
                        //cg.XNAGame.SpectaterCamera.Enabled = false;
                        cg.XNAGame.LineManager3D.AddCenteredBox(player.Position, 1, Color.Red);
                    };

                cg.Run();



                if (Vector3.Distance(player.Position, new Vector3(-1, 0, -1) * controller.MovementSpeed) > 0.001f)
                {
                    throw new Exception("Invalid movement of the player!");
                }

            }

        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]

        public void TestPlayerInputOffline()
        {
            throw new Exception("This is Deprecated");
            XNAGame game = new XNAGame();

            using (PhysicsEngine engine = new PhysicsEngine())
            {

                PlayerData player = new PlayerData();
                player.Name = "MHGameWork";

                PlayerController controller = null;
                PlayerInput input = null;
                bool cameraMode = true;

                game.SpectaterCamera.CameraDirection = Vector3.Normalize(new Vector3(1.3f, -2, 1));
                game.SpectaterCamera.CameraPosition = new Vector3(10, 10, -10);


                game.InitializeEvent +=
                    delegate
                    {

                        engine.Initialize(game);
                        PhysicsDebugRenderer debugRenderer = new PhysicsDebugRenderer(game,
                                                                                      engine.Scene);

                        game.AddXNAObject(debugRenderer);

                        controller = new PlayerController(game, player, engine.Scene);
                        input = new PlayerInput(controller);



                        ActorDescription actorDesc;
                        Actor actor;

                        BoxShapeDescription boxShapeDesc = new BoxShapeDescription(1, 1, 1);

                        actorDesc = new ActorDescription(boxShapeDesc);
                        actorDesc.BodyDescription = new BodyDescription(1f);

                        actor = engine.Scene.CreateActor(actorDesc);
                        actor.GlobalPosition = new Vector3(1, 4, 0);

                        actor = engine.Scene.CreateActor(actorDesc);
                        actor.GlobalPosition = new Vector3(0, 4, 3);


                        actorDesc.BodyDescription = new BodyDescription(0.01f);

                        actor = engine.Scene.CreateActor(actorDesc);
                        actor.GlobalPosition = new Vector3(5, 4, 0);

                        actor = engine.Scene.CreateActor(actorDesc);
                        actor.GlobalPosition = new Vector3(3, 4, 3);



                    };

                game.UpdateEvent +=
                    delegate
                    {
                        engine.Update(game);
                        if (!cameraMode) input.Update(game);
                        if (game.Keyboard.IsKeyPressed(Keys.Tab))
                        {

                            cameraMode = !cameraMode;
                            game.SpectaterCamera.Enabled = cameraMode;
                        }
                    };


                game.DrawEvent +=
                    delegate
                    {
                        //cg.XNAGame.SpectaterCamera.Enabled = false;
                        game.LineManager3D.AddCenteredBox(player.Position, 1, Color.Red);
                        game.LineManager3D.AddLine(player.Position, player.Position + controller.GetForwardVector(), Color.Green);
                    };

                game.Run();

                controller.Dispose();
                GC.Collect();

            }




        }

        /// <summary>
        /// This might be obsolete See GameplayTest
        /// </summary>
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestThirdPersonCamera()
        {
            throw new Exception("This is Deprecated");

            //TODO

            XNAGame game = new XNAGame();

            PhysicsEngine engine = new PhysicsEngine();

            PlayerData player = new PlayerData();
            player.Name = "MHGameWork";

            PlayerController controller = null;
            PlayerInput input = null;
            PlayerThirdPersonCamera playerCamera = new PlayerThirdPersonCamera(game, player);
            game.AddXNAObject(playerCamera);
            bool cameraMode = true;

            game.SpectaterCamera.CameraDirection = Vector3.Normalize(new Vector3(1.3f, -2, 1));
            game.SpectaterCamera.CameraPosition = new Vector3(10, 10, -10);


            game.InitializeEvent += delegate
             {

                 engine.Initialize(game);
                 PhysicsDebugRenderer debugRenderer = new PhysicsDebugRenderer(game, engine.Scene);

                 game.AddXNAObject(debugRenderer);

                 controller = new PlayerController(game, player, engine.Scene);
                 input = new PlayerInput(controller);


                 ActorDescription actorDesc;
                 Actor actor;

                 BoxShapeDescription boxShapeDesc = new BoxShapeDescription(1f, 1f, 1f);

                 actorDesc = new ActorDescription(boxShapeDesc);
                 actorDesc.BodyDescription = new BodyDescription(1f);

                 actor = engine.Scene.CreateActor(actorDesc);
                 actor.GlobalPosition = new Vector3(1, 4, 0);

                 actor = engine.Scene.CreateActor(actorDesc);
                 actor.GlobalPosition = new Vector3(0, 4, 3);


                 actorDesc.BodyDescription = new BodyDescription(0.01f);

                 actor = engine.Scene.CreateActor(actorDesc);
                 actor.GlobalPosition = new Vector3(5, 4, 0);

                 actor = engine.Scene.CreateActor(actorDesc);
                 actor.GlobalPosition = new Vector3(3, 4, 3);


             };

            game.UpdateEvent += delegate
                {
                    engine.Update(game);
                    if (!cameraMode) input.Update(game);
                    if (game.Keyboard.IsKeyPressed(Keys.Tab))
                    {

                        cameraMode = !cameraMode;
                        game.SpectaterCamera.Enabled = cameraMode;
                        playerCamera.Enabled = !game.SpectaterCamera.Enabled;
                        if (cameraMode)
                        {
                            game.SetCamera(game.SpectaterCamera);
                        }
                        else
                        {
                            game.SetCamera(playerCamera);
                        }
                    }
                };


            game.DrawEvent +=
                delegate
                {
                    //cg.XNAGame.SpectaterCamera.Enabled = false;
                    game.LineManager3D.AddCenteredBox(player.Position, 1, Color.Red);
                    game.LineManager3D.AddLine(player.Position, player.Position + controller.GetForwardVector(), Color.Green);
                };

            game.Run();


        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestPlayerClientPhysics()
        {
            XNAGame game = new XNAGame();

            PhysicsEngine engine = new PhysicsEngine();

            PlayerData player = new PlayerData();
            player.Name = "MHGameWork";

            PlayerController controller = null;
            PlayerInput input = null;
            PlayerClientPhysics playerClientPhysics = null;
            PlayerThirdPersonCamera playerCamera = new PlayerThirdPersonCamera(game, player);
            game.AddXNAObject(playerCamera);
            game.SetCamera(playerCamera);

            TheWizards.Client.ClientPhysicsQuadTreeNode tree = ClientTest.CreateTestClientPhysicsQuadtree();

            QuadTreeVisualizer visualizer = new QuadTreeVisualizer();

            game.InitializeEvent += delegate
             {

                 engine.Initialize(game);
                 PhysicsDebugRenderer debugRenderer = new PhysicsDebugRenderer(game, engine.Scene);

                 game.AddXNAObject(debugRenderer);

                 controller = new PlayerController(game, player, engine.Scene);
                 input = new PlayerInput(controller);
                 playerClientPhysics = new MHGameWork.TheWizards.Player.Client.PlayerClientPhysics(controller);



             };

            game.UpdateEvent += delegate
                {
                    engine.Update(game);
                    playerClientPhysics.Update(tree);

                    input.Update(game);

                };


            game.DrawEvent +=
                delegate
                {

                    visualizer.RenderNodeGroundBoundig(game, tree,
                        delegate(ClientPhysicsQuadTreeNode node, out Color col)
                        {
                            col = Color.Green;

                            return node.PhysicsObjects.Count == 0;
                        });

                    visualizer.RenderNodeGroundBoundig(game, tree,
                       delegate(ClientPhysicsQuadTreeNode node, out Color col)
                       {
                           col = Color.Orange;

                           return node.PhysicsObjects.Count > 0;
                       });

                    visualizer.RenderNodeGroundBoundig(game, tree,
                       delegate(ClientPhysicsQuadTreeNode node, out Color col)
                       {
                           col = Color.Red;

                           return node.DynamicObjectsCount > 0;
                       });
                    game.LineManager3D.AddCenteredBox(player.Position, 1, Color.Red);
                    game.LineManager3D.AddLine(player.Position, player.Position + controller.GetForwardVector(), Color.Green);
                };

            game.Run();


        }

        [Test]
        public void TestCreatePlayer()
        {
            throw new Exception("This is Deprecated");
            Database.Database db = new MHGameWork.TheWizards.Database.Database();
            PlayerService playerService = new PlayerService(db);


            PlayerData player = new PlayerData();
            player.Name = "Test Player 1";
            player.Position = new Microsoft.Xna.Framework.Vector3(10, 0, 1);

            playerService.AddPlayer(player);

            Assert.AreSame(player, playerService.FindPlayerByName("Test Player 1"));


        }
    }
}
