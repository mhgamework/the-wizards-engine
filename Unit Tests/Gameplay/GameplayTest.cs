using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Gameplay.Fortress;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Scripting;
using MHGameWork.TheWizards.Tests.Networking;
using MHGameWork.TheWizards.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;
using StillDesign.PhysX;
using DataPacket = MHGameWork.TheWizards.Networking.Packets.DataPacket;
using PlayerController = MHGameWork.TheWizards.GamePlay.PlayerController;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    [TestFixture]
    public class GameplayTest
    {

        [TearDown]
        public void TearDown()
        {
            ScriptLayer.ClearScope();
            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
        }

        public static void ProcessPlayerInputDirect(PlayerController controller, IXNAGame game)
        {

            if (game.Keyboard.IsKeyDown(Keys.Z))
            {
                controller.DoMoveForward(game.Elapsed);
            }
            if (game.Keyboard.IsKeyDown(Keys.S))
            {
                controller.DoMoveBackwards(game.Elapsed);
            }
            if (game.Keyboard.IsKeyDown(Keys.Q))
            {
                controller.DoStrafeLeft(game.Elapsed);
            }
            if (game.Keyboard.IsKeyDown(Keys.D))
            {
                controller.DoStrafeRight(game.Elapsed);
            }
            if (game.Keyboard.IsKeyPressed(Keys.Space))
            {
                controller.DoJump();
            }
        }

        [Test]
        public void TestPlayerMovement()
        {
            var game = new XNAGame();
            EstablishDefaultScope(game);

            var player = createPlayerData();

            var controller = new HelperPlayerController(game, player);

            game.AddXNAObject(controller);
            game.UpdateEvent += delegate { ProcessPlayerInputDirect(controller.Controller, game); };
            game.DrawEvent += delegate { game.LineManager3D.DrawGroundShadows = true; };

            game.Run();


        }

        [Test]
        public void TestPlayerSyncOnline()
        {
            var server = new ServerPacketManagerNetworked(10045, 10046);
            var success = new AutoResetEvent(false);

            var serverSyncer = new ServerSyncer(server);
            ServerSyncedActor serverA = null;




            ClientPacketManagerNetworked client = null;
            ClientSyncer clientSyncer = null;



            StillDesign.PhysX.Scene serverScene = null;

            PhysicsDebugRenderer debugRendererServer;
            var game = new XNAGame();

            EstablishDefaultScope(game);

            PlayerController controller = null;
            PlayerController controllerClient = null;

            PlayerThirdPersonCamera cam = null;

            game.InitializeEvent += delegate
            {


            };

            game.UpdateEvent += delegate
            {
                ScriptLayer.Scene = ScriptLayer.Physics.Scene;
                if (game.FrameNumber == 1)
                {

                    serverScene = ScriptLayer.Physics.Core.CreateScene(ScriptLayer.Physics.Scene.Gravity, true);
                    debugRendererServer = new PhysicsDebugRenderer(game, serverScene);
                    game.AddXNAObject(debugRendererServer);
                    debugRendererServer.Initialize(game);

                    ScriptLayer.Scene = serverScene;


                    controller = new PlayerController(new PlayerData());
                    controller.Initialize(game);

                    server.Start();
                    serverA = serverSyncer.CreateActor(controller);
                }
                else if (game.FrameNumber == 2)
                {
                    var conn = NetworkingClientTest.ConnectTCP(10045, "127.0.0.1");
                    conn.Receiving = true;
                    client = new ClientPacketManagerNetworked(conn);

                    clientSyncer = new ClientSyncer(client);

                    client.WaitForUDPConnected();
                    client.SyncronizeRemotePacketIDs();

                    controllerClient = new PlayerController(new PlayerData());
                    controllerClient.Initialize(game);


                    var clientA = clientSyncer.CreateActor(controllerClient);
                    clientA.ID = serverA.ID;

                    cam = new PlayerThirdPersonCamera(game, controllerClient.Player);
                    cam.Enabled = true;
                    game.AddXNAObject(cam);
                    game.SetCamera(cam);

                }
                else
                {
                    controller.HorizontalAngle = cam.LookAngleHorizontal;

                    ProcessPlayerInputDirect(controller, game);
                    controller.Update(game);
                    serverSyncer.Update(game.Elapsed);
                    ScriptLayer.Physics.UpdateScene(game.Elapsed, serverScene);



                    controllerClient.Update(game);
                    clientSyncer.Update(game.Elapsed);
                }

            };



            game.Run();

        }

        [Test]
        public void TestPlayerOnlineServer()
        {
            var server = new ServerPacketManagerNetworked(10045, 10046);

            var serverSyncer = new ServerSyncer(server);
            var dataPacketTransporter = server.CreatePacketTransporter("PlayerControllerID",
                                                                         new DataPacket.Factory(), PacketFlags.TCP);

            StillDesign.PhysX.Scene serverScene = null;

            PhysicsDebugRenderer debugRendererServer;
            var game = new XNAGame();
            game.InputDisabled = true;
            EstablishDefaultScope(game);

            List<PlayerController> controllers = new List<PlayerController>();

            List<PlayerInputServer> inputServers = new List<PlayerInputServer>();

            IServerPacketTransporter<PlayerInputPacket> inputServerTransporter = null;
            game.InitializeEvent += delegate
            {


            };

            game.UpdateEvent += delegate
            {
                ScriptLayer.Scene = ScriptLayer.Physics.Scene;
                if (game.FrameNumber == 1)
                {

                    serverScene = ScriptLayer.Physics.Core.CreateScene(ScriptLayer.Physics.Scene.Gravity, true);
                    debugRendererServer = new PhysicsDebugRenderer(game, serverScene);
                    game.AddXNAObject(debugRendererServer);
                    debugRendererServer.Initialize(game);

                    ScriptLayer.Scene = serverScene;


                    server.Start();

                    inputServerTransporter = PlayerInputServer.CreateTransporter(server);

                }
                else
                {
                    if (server.Clients.Count > inputServers.Count)
                    {
                        if (server.Clients[inputServers.Count].IsReady)
                            for (int i = inputServers.Count; i < server.Clients.Count; i++)
                            {
                                var c = new PlayerController(new PlayerData());
                                var iS =
                                    new PlayerInputServer(
                                        inputServerTransporter.GetTransporterForClient(server.Clients[i]), c);

                                c.Initialize(game);

                                var a = serverSyncer.CreateActor(c);

                                //dataPacketTransporter.GetTransporterForClient(server.Clients[i]).Send(new DataPacket() { Data = BitConverter.GetBytes(a.ID) });
                                dataPacketTransporter.SendAll(new DataPacket() { Data = BitConverter.GetBytes(a.ID) });

                                for (int j = 0; j < i; j++)
                                {
                                    //Cheat
                                    dataPacketTransporter.GetTransporterForClient(server.Clients[i]).Send(new DataPacket() { Data = BitConverter.GetBytes((ushort)(j + 2)) });

                                }

                                controllers.Add(c);
                                inputServers.Add(iS);

                            }
                    }



                    for (int i = 0; i < controllers.Count; i++)
                    {
                        controllers[i].Update(game);
                        inputServers[i].Update(game.Elapsed);
                    }
                    serverSyncer.Update(game.Elapsed);
                    ScriptLayer.Physics.UpdateScene(game.Elapsed, serverScene);

                }

            };



            game.Run();
        }

        [Test]
        public void TestPlayerOnlineClient()
        {

            ClientPacketManagerNetworked client = null;
            ClientSyncer clientSyncer = null;
            IClientPacketTransporter<DataPacket> dataPacketTransporter = null;

            var game = new XNAGame();
            game.InputDisabled = true;
            EstablishDefaultScope(game);

            List<PlayerController> controllerClients = new List<PlayerController>();

            PlayerThirdPersonCamera cam = null;
            PlayerInputClient inputClient = null;
            game.InitializeEvent += delegate
            {


            };

            game.UpdateEvent += delegate
            {
                ScriptLayer.Scene = ScriptLayer.Physics.Scene;
                if (game.FrameNumber == 1)
                {

                }
                else if (game.FrameNumber == 2)
                {
                    //var conn = NetworkingClientTest.ConnectTCP(10045, "127.0.0.1");
                    var conn = NetworkingClientTest.ConnectTCP(10045, "5.149.17.16");
                    conn.Receiving = true;
                    client = new ClientPacketManagerNetworked(conn);

                    clientSyncer = new ClientSyncer(client);
                    dataPacketTransporter = client.CreatePacketTransporter("PlayerControllerID",
                                                                           new DataPacket.Factory(), PacketFlags.TCP);


                    client.WaitForUDPConnected();
                    client.SyncronizeRemotePacketIDs();



                    inputClient = new PlayerInputClient(client);
                }
                else
                {
                    while (dataPacketTransporter.PacketAvailable)
                    {
                        var p = dataPacketTransporter.Receive();
                        var c = new PlayerController(new PlayerData());
                        c.Initialize(game);
                        game.AddXNAObject(c);

                        var clientA = clientSyncer.CreateActor(c);
                        clientA.ID = BitConverter.ToUInt16(p.Data, 0);


                        if (cam == null)
                        {
                            cam = new PlayerThirdPersonCamera(game, c.Player);
                            cam.Enabled = true;
                            game.AddXNAObject(cam);
                            game.SetCamera(cam);
                        }

                    }

                    for (int i = 0; i < controllerClients.Count; i++)
                    {
                        controllerClients[i].Update(game);
                    }
                    clientSyncer.Update(game.Elapsed);

                    if (cam != null)
                        inputClient.HorizontalLookAngle = cam.LookAngleHorizontal;
                    inputClient.Update(game);
                }

            };



            game.Run();
        }

        [Test]
        public void TestPlayerMovementOnline()
        {
            var server = new ServerPacketManagerNetworked(10045, 10046);


            ClientPacketManagerNetworked client = null;

            PlayerThirdPersonCamera cam = null;

            StillDesign.PhysX.Scene serverScene = null;

            PhysicsDebugRenderer debugRendererServer;
            var game = new XNAGame();

            EstablishDefaultScope(game);

            PlayerController controller = null;
            PlayerInputClient inputClient = null;
            PlayerInputServer inputServer = null;
            IServerPacketTransporter<PlayerInputPacket> inputTrans = null;

            game.InitializeEvent += delegate
            {


            };

            game.UpdateEvent += delegate
            {
                ScriptLayer.Scene = ScriptLayer.Physics.Scene;
                if (game.FrameNumber == 1)
                {

                    serverScene = ScriptLayer.Physics.Core.CreateScene(ScriptLayer.Physics.Scene.Gravity, true);
                    debugRendererServer = new PhysicsDebugRenderer(game, serverScene);
                    game.AddXNAObject(debugRendererServer);
                    debugRendererServer.Initialize(game);

                    ScriptLayer.Scene = serverScene;


                    controller = new PlayerController(new PlayerData());
                    controller.Initialize(game);

                    inputTrans = PlayerInputServer.CreateTransporter(server);


                    server.Start();
                }
                else if (game.FrameNumber == 2)
                {
                    var conn = NetworkingClientTest.ConnectTCP(10045, "127.0.0.1");
                    conn.Receiving = true;
                    client = new ClientPacketManagerNetworked(conn);

                    client.WaitForUDPConnected();
                    client.SyncronizeRemotePacketIDs();



                    inputClient = new PlayerInputClient(client);


                    cam = new PlayerThirdPersonCamera(game, controller.Player);
                    cam.Enabled = true;
                    game.AddXNAObject(cam);
                    game.SetCamera(cam);

                }
                else if (game.FrameNumber == 3)
                {

                    inputServer = new PlayerInputServer(inputTrans.GetTransporterForClient(server.Clients[0]), controller);

                }
                else
                {
                    controller.Update(game);
                    ScriptLayer.Physics.UpdateScene(game.Elapsed, serverScene);
                    inputServer.Update(game.Elapsed);


                    inputClient.HorizontalLookAngle = cam.LookAngleHorizontal;
                    inputClient.Update(game);
                }

            };



            game.Run();

        }

        [Test]
        public void TestThirdPersonCamera()
        {


            XNAGame game = new XNAGame();


            PlayerData player = createPlayerData();

            PlayerThirdPersonCamera playerCamera = new PlayerThirdPersonCamera(game, player);
            game.AddXNAObject(playerCamera);
            game.SetCamera(playerCamera);
            playerCamera.Enabled = true;


            game.InitializeEvent += delegate
             {

             };

            game.UpdateEvent += delegate
                {
                };


            game.DrawEvent +=
                delegate
                {
                    game.LineManager3D.AddCenteredBox(player.Position, 1, Color.Red);
                };

            game.Run();
        }

        [Test]
        public void TestEnergyOrb()
        {
           /* var orb = new EnergyOrb();
            orb.Position = new Vector3(-20, 2, 0);
            runScript(orb, delegate
            {
                orb.Fire(new Vector3(4, 18, 0));
            });*/
        }

        [Test]
        public void TestThrowMove()
        {
            var game = new XNAGame();
            EstablishDefaultScope(game);

            var player = createPlayerData();

            var controller = new HelperPlayerController(game, player);
            game.AddXNAObject(controller);
            var move = new ThrowMove(controller.Controller);
            ScriptLayer.ScriptRunner.RunScript(move);

            game.UpdateEvent += delegate
                                    {
                                        ProcessPlayerInputDirect(controller.Controller, game);
                                        move.FireDirection = CalculateFireDirection(controller.ThirdPersonCamera);

                                        if (game.Mouse.LeftMouseJustPressed)
                                            move.StartPrimaryAttack();
                                        if (game.Mouse.LeftMouseJustReleased)
                                            move.EndPrimaryAttack();
                                    };


            game.DrawEvent += delegate { game.LineManager3D.DrawGroundShadows = true; };

            game.Run();
        }

        [Test]
        public void TestFireMove()
        {
            throw new NotImplementedException();
            var game = new XNAGame();
            EstablishDefaultScope(game);

            var player = createPlayerData();

            var controller = new HelperPlayerController(game, player);

            game.AddXNAObject(controller);
            FireMove move = null;// new FireMove(controller.Controller);
            ScriptLayer.ScriptRunner.RunScript(move);

            game.UpdateEvent += delegate
            {
                ProcessPlayerInputDirect(controller.Controller, game);
                move.FireDirection = CalculateFireDirection(controller.ThirdPersonCamera);

                if (game.Mouse.LeftMouseJustPressed)
                    move.StartPrimaryAttack();
                if (game.Mouse.LeftMouseJustReleased)
                    move.EndPrimaryAttack();
                if (game.Mouse.RightMouseJustPressed)
                    move.StartSecondaryAttack();
                if (game.Mouse.RightMouseJustReleased)
                    move.EndSecondaryAttack();
            };


            game.DrawEvent += delegate { game.LineManager3D.DrawGroundShadows = true; };

            game.Run();
        }

        [Test]
        public void TestHorse()
        {
            var game = new XNAGame();
            EstablishDefaultScope(game);


            InitializePlayer();

            var seeder = new Seeder(84354);


            game.InitializeEvent += delegate
            {
                var horse = new Horse();
                horse.Position = new Vector3(4, 2, 3);
                ScriptLayer.ScriptRunner.RunScript(horse);

                var range = new Vector3(50, 0, 50);

                for (int i = 0; i < 20; i++)
                {
                    horse = new Horse();
                    horse.Position = Vector3.Up * 1 + seeder.NextVector3(-range, range);
                    ScriptLayer.ScriptRunner.RunScript(horse);

                }

            };


            game.DrawEvent += delegate { game.LineManager3D.DrawGroundShadows = true; };

            game.Run();
        }

        [Test]
        public void TestPlayer()
        {
            var game = new XNAGame();
            EstablishDefaultScope(game);

            InitializePlayer();

            game.Run();
        }

      
        public void InitializePlayer()
        {
            throw new NotImplementedException();
            var player = createPlayerData();
            player.Position = new Vector3(0, 3, 0);



            var game = (XNAGame)ScriptLayer.Game;

            var controller = new HelperPlayerController(game, player);


            FireMove fireMove=null;// = new FireMove(controller.Controller);
            ScriptLayer.ScriptRunner.RunScript(fireMove);


            var throwMove = new ThrowMove(controller.Controller);
            ScriptLayer.ScriptRunner.RunScript(throwMove);


            game.AddXNAObject(controller);



            IPlayerMove move = fireMove;

            game.UpdateEvent += delegate
                                    {
                                        ProcessPlayerInputDirect(controller.Controller, game);
                                        fireMove.FireDirection = CalculateFireDirection(controller.ThirdPersonCamera);
                                        throwMove.FireDirection = CalculateFireDirection(controller.ThirdPersonCamera);

                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D1))
                                            move = fireMove;
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D2))
                                            move = throwMove;

                                        if (game.Mouse.LeftMouseJustPressed)
                                            move.StartPrimaryAttack();
                                        if (game.Mouse.LeftMouseJustReleased)
                                            move.EndPrimaryAttack();
                                        if (game.Mouse.RightMouseJustPressed)
                                            move.StartSecondaryAttack();
                                        if (game.Mouse.RightMouseJustReleased)
                                            move.EndSecondaryAttack();
                                    };


        }

        public static PlayerData createPlayerData()
        {
            PlayerData player = new PlayerData();
            player.Name = "MHGameWork";
            player.Position = Vector3.One;
            return player;
        }


        public static void EstablishDefaultScope(XNAGame game)
        {
            ScriptLayer.Game = game;
            ScriptLayer.ScriptRunner = new ScriptRunner(game);

            PhysicsEngine engine = new PhysicsEngine();

            ScriptLayer.Physics = engine;

            game.InitializeEvent +=
                delegate
                {
                    engine.Initialize(game);
                    ScriptLayer.Scene = engine.Scene;

                    PhysicsDebugRenderer debugRenderer =
                        new PhysicsDebugRenderer(game, engine.Scene);

                    game.AddXNAObject(debugRenderer);

                    var boxDesc = new BoxShapeDescription();
                    boxDesc.Dimensions = new Vector3(100, 1, 100);
                    boxDesc.LocalPosition = Vector3.Up * -1;
                    var actorDesc = new ActorDescription(boxDesc);

                    engine.Scene.CreateActor(actorDesc);


                };

            game.UpdateEvent +=
                 delegate
                 {
                     engine.Update(game);
                 };




        }


        public static Vector3 CalculateFireDirection(PlayerThirdPersonCamera cam)
        {
            var pos = Vector3.Transform(Vector3.Zero, cam.ViewInverse);
            Vector3 dir = Vector3.Transform(Vector3.Forward, cam.ViewInverse);
            return Vector3.Normalize(dir - pos);
        }

        private void runScript(IStateScript script)
        {
            runScript(script, delegate { });
        }

        private void runScript(IStateScript script, Action initialize)
        {
            var game = new XNAGame();

            EstablishDefaultScope(game);

            game.InitializeEvent += delegate
            {
                ScriptLayer.ScriptRunner.RunScript(script);
                initialize();
            };


            game.Run();
        }
    }
}
