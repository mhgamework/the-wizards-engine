using System;
using System.Threading;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Tests.Features.Core.Networking;
using MHGameWork.TheWizards.World;
using MHGameWork.TheWizards.World.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Tests.Features.Various.World
{
    [TestFixture]
    public class TestSyncedActor
    {
        [Test]
        public void TestSyncDirect()
        {
            var client = new ClientSyncedActor();
            var server = new ServerSyncedActor();

            var physicsEngine = new PhysicsEngine();
            StillDesign.PhysX.Scene serverScene = null;

            PhysicsDebugRendererXNA debugRenderer;
            PhysicsDebugRendererXNA debugRendererServer;
            var game = new XNAGame();
            float totalTime = 0;
            float timeSinceTick = 0;
            float tickRate = 1 / 30f;
            int tickNumber = 0;
            float packetLoss = 0.25f;

            var rand = new Random();
            game.InitializeEvent += delegate
                                    {
                                        physicsEngine.Initialize();
                                        serverScene = physicsEngine.CreateScene(physicsEngine.Scene.Gravity, true);


                                        debugRenderer = new PhysicsDebugRendererXNA(game, physicsEngine.Scene);
                                        game.AddXNAObject(debugRenderer);
                                        debugRendererServer = new PhysicsDebugRendererXNA(game, serverScene);
                                        game.AddXNAObject(debugRendererServer);

                                        ActorDescription actorDesc;
                                        SphereShapeDescription shape;

                                        shape = new SphereShapeDescription(1);
                                        actorDesc = new ActorDescription(shape);
                                        actorDesc.BodyDescription = new BodyDescription(10);

                                        server.Actor = new WorldPhysxSyncActor(serverScene.CreateActor(actorDesc));
                                        ((WorldPhysxSyncActor)server.Actor).Actor.AddForce(Vector3.UnitX * 200, ForceMode.Impulse);

                                        shape = new SphereShapeDescription(1);
                                        actorDesc = new ActorDescription(shape);
                                        actorDesc.BodyDescription = new BodyDescription(10);

                                        client.Actor =
                                            new WorldPhysxSyncActor(physicsEngine.Scene.CreateActor(actorDesc));
                                    };

            game.UpdateEvent += delegate
                                {
                                    physicsEngine.Update(game);
                                    physicsEngine.UpdateScene(game.Elapsed, serverScene);
                                    totalTime += game.Elapsed;
                                    timeSinceTick += game.Elapsed;
                                    int totalMiliseconds = (int)(totalTime * 1000);

                                    while (timeSinceTick > tickRate)
                                    {
                                        timeSinceTick -= tickRate;
                                        //Do a tick
                                        tickNumber++;
                                        server.Tick();
                                        var p = new UpdateEntityPacket();
                                        p.Positie = server.Positie;
                                        p.RotatieQuat = server.RotatieQuat;
                                        if (rand.NextDouble() < 1 - packetLoss)
                                            client.AddEntityUpdate(tickNumber, p);

                                    }

                                    client.Process(totalMiliseconds, tickRate);





                                };

            game.Run();

            physicsEngine.Dispose();
        }

        [Test]
        public void TestSyncOffline()
        {
            var serverpm = new SimpleServerPacketManager();
            var clientpm = serverpm.CreateClient();


            var clientSyncer = new ClientSyncer(clientpm);
            var serverSyncer = new ServerSyncer(serverpm);

            var physicsEngine = new PhysicsEngine();
            StillDesign.PhysX.Scene serverScene = null;

            PhysicsDebugRendererXNA debugRenderer;
            PhysicsDebugRendererXNA debugRendererServer;
            var game = new XNAGame();
            float totalTime = 0;
            float timeSinceTick = 0;
            float tickRate = 1 / 30f;
            int tickNumber = 0;
            float packetLoss = 0.25f;


            var rand = new Random();
            game.InitializeEvent += delegate
                                    {
                                        physicsEngine.Initialize();
                                        serverScene = physicsEngine.CreateScene(physicsEngine.Scene.Gravity, true);


                                        debugRenderer = new PhysicsDebugRendererXNA(game, physicsEngine.Scene);
                                        game.AddXNAObject(debugRenderer);
                                        debugRendererServer = new PhysicsDebugRendererXNA(game, serverScene);
                                        game.AddXNAObject(debugRendererServer);

                                        ActorDescription actorDesc;
                                        SphereShapeDescription shape;

                                        shape = new SphereShapeDescription(1);
                                        actorDesc = new ActorDescription(shape);
                                        actorDesc.BodyDescription = new BodyDescription(10);

                                        var server = serverSyncer.CreateActor(serverScene.CreateActor(actorDesc));
                                        ((WorldPhysxSyncActor)server.Actor).Actor.AddForce(Vector3.UnitX * 200, ForceMode.Impulse);

                                        shape = new SphereShapeDescription(1);
                                        actorDesc = new ActorDescription(shape);
                                        actorDesc.BodyDescription = new BodyDescription(10);

                                        var client = clientSyncer.CreateActor(physicsEngine.Scene.CreateActor(actorDesc));

                                        client.ID = server.ID; // Identify
                                    };

            game.UpdateEvent += delegate
                                {
                                    physicsEngine.Update(game);
                                    physicsEngine.UpdateScene(game.Elapsed, serverScene);

                                    serverSyncer.Update(game.Elapsed);
                                    clientSyncer.Update(game.Elapsed);


                                };

            game.Run();

            physicsEngine.Dispose();
        }

        [Test]
        public void TestSyncOnline()
        {
            var server = new ServerPacketManagerNetworked(10045, 10046);
            var success = new AutoResetEvent(false);







            var serverSyncer = new ServerSyncer(server);



            server.Start();

            var conn = NetworkingUtilities.ConnectTCP(10045, "127.0.0.1");
            conn.Receiving = true;
            var client = new ClientPacketManagerNetworked(conn);

            var clientSyncer = new ClientSyncer(client);



            var physicsEngine = new PhysicsEngine();
            StillDesign.PhysX.Scene serverScene = null;

            PhysicsDebugRendererXNA debugRenderer;
            PhysicsDebugRendererXNA debugRendererServer;
            var game = new XNAGame();

            game.InitializeEvent += delegate
                                    {
                                        physicsEngine.Initialize();
                                        serverScene = physicsEngine.CreateScene(physicsEngine.Scene.Gravity, true);


                                        debugRenderer = new PhysicsDebugRendererXNA(game, physicsEngine.Scene);
                                        game.AddXNAObject(debugRenderer);
                                        debugRendererServer = new PhysicsDebugRendererXNA(game, serverScene);
                                        game.AddXNAObject(debugRendererServer);

                                        ActorDescription actorDesc;
                                        SphereShapeDescription shape;

                                        shape = new SphereShapeDescription(1);
                                        actorDesc = new ActorDescription(shape);
                                        actorDesc.BodyDescription = new BodyDescription(10);

                                        var server1 = serverSyncer.CreateActor(serverScene.CreateActor(actorDesc));
                                        ((WorldPhysxSyncActor)server1.Actor).Actor.AddForce(Vector3.UnitX * 200, ForceMode.Impulse);

                                        shape = new SphereShapeDescription(1);
                                        actorDesc = new ActorDescription(shape);
                                        actorDesc.BodyDescription = new BodyDescription(10);

                                        var client1 = clientSyncer.CreateActor(physicsEngine.Scene.CreateActor(actorDesc));

                                        client1.ID = server1.ID; // Identify
                                    };

            game.UpdateEvent += delegate
                                {
                                    physicsEngine.Update(game);
                                    physicsEngine.UpdateScene(game.Elapsed, serverScene);

                                    serverSyncer.Update(game.Elapsed);
                                    clientSyncer.Update(game.Elapsed);


                                };

            client.SyncronizeRemotePacketIDs();
            client.WaitForUDPConnected();

            game.Run();

            physicsEngine.Dispose();
        }

        [Test]
        public void TestSyncOnlineServer()
        {
            var server = new ServerPacketManagerNetworked(10045, 10046);


            var serverSyncer = new ServerSyncer(server);



            server.Start();


            var physicsEngine = new PhysicsEngine();
            StillDesign.PhysX.Scene serverScene = null;

            PhysicsDebugRendererXNA debugRenderer;
            PhysicsDebugRendererXNA debugRendererServer;
            var game = new XNAGame();
            ServerSyncedActor server1 = null;
            game.InitializeEvent += delegate
            {
                physicsEngine.Initialize();
                serverScene = physicsEngine.CreateScene(physicsEngine.Scene.Gravity, true);


                debugRenderer = new PhysicsDebugRendererXNA(game, physicsEngine.Scene);
                game.AddXNAObject(debugRenderer);
                debugRendererServer = new PhysicsDebugRendererXNA(game, serverScene);
                game.AddXNAObject(debugRendererServer);

                ActorDescription actorDesc;
                SphereShapeDescription shape;

                shape = new SphereShapeDescription(1);
                actorDesc = new ActorDescription(shape);
                actorDesc.BodyDescription = new BodyDescription(10);

                server1 = serverSyncer.CreateActor(serverScene.CreateActor(actorDesc));
                ((WorldPhysxSyncActor)server1.Actor).Actor.AddForce(Vector3.UnitX * 200, ForceMode.Impulse);
                server1.ID = 2; // Identify

            };

            game.UpdateEvent += delegate
            {
                physicsEngine.Update(game);
                physicsEngine.UpdateScene(game.Elapsed, serverScene);

                serverSyncer.Update(game.Elapsed);

                if (game.Keyboard.IsKeyDown(Keys.Up))
                {
                    ((WorldPhysxSyncActor)server1.Actor).Actor.AddForce(Vector3.UnitX * 200, ForceMode.Force);

                }
                if (game.Keyboard.IsKeyDown(Keys.Down))
                {
                    ((WorldPhysxSyncActor)server1.Actor).Actor.AddForce(-Vector3.UnitX * 200, ForceMode.Force);

                }


            };

            game.Run();

            physicsEngine.Dispose();
        }

        [Test]
        public void TestSyncOnlineClient()
        {
            var conn = NetworkingUtilities.ConnectTCP(10045, "5.149.17.16");
            //var conn = NetworkingClientTest.ConnectTCP(10045, "127.0.0.1");
            conn.Receiving = true;
            var client = new ClientPacketManagerNetworked(conn);

            var clientSyncer = new ClientSyncer(client);



            var physicsEngine = new PhysicsEngine();
            StillDesign.PhysX.Scene serverScene = null;

            PhysicsDebugRendererXNA debugRenderer;
            PhysicsDebugRendererXNA debugRendererServer;
            var game = new XNAGame();

            game.InitializeEvent += delegate
            {
                physicsEngine.Initialize();
                serverScene = physicsEngine.CreateScene(physicsEngine.Scene.Gravity, true);


                debugRenderer = new PhysicsDebugRendererXNA(game, physicsEngine.Scene);
                game.AddXNAObject(debugRenderer);
                debugRendererServer = new PhysicsDebugRendererXNA(game, serverScene);
                game.AddXNAObject(debugRendererServer);

                ActorDescription actorDesc;
                SphereShapeDescription shape;


                shape = new SphereShapeDescription(1);
                actorDesc = new ActorDescription(shape);
                actorDesc.BodyDescription = new BodyDescription(10);

                var client1 = clientSyncer.CreateActor(physicsEngine.Scene.CreateActor(actorDesc));

                client1.ID = 2; // Identify
            };

            game.UpdateEvent += delegate
            {
                physicsEngine.Update(game);
                physicsEngine.UpdateScene(game.Elapsed, serverScene);

                clientSyncer.Update(game.Elapsed);


            };

            client.SyncronizeRemotePacketIDs();
            client.WaitForUDPConnected();

            game.Run();

            physicsEngine.Dispose();
        }
    }
}
