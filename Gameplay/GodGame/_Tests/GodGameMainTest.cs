using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Autofac.Core;
using Castle.DynamicProxy;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Configuration;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Networking;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.GodGame.Persistence;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;
using NSubstitute;
using NUnit.Framework;
using SlimDX;
using System.Linq;
using Autofac;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    /// <summary>
    /// Test for the full game + helpers for integration testing.
    /// </summary>
    [TestFixture]
    public class GodGameMainTest
    {
        [Test]
        public void TestClientGame()
        {
            var game = new GodGameServerClient();
            var client = game.CreateClient(null);

            client.AddSimulatorsToEngine(EngineFactory.CreateEngine());

            client.ConnectToServer("7.63.207.22", 15005);

        }
        [Test]
        public void TestServerGame()
        {
            var server = new GodGameServerClient().CreateServer(null);
            server.AddSimulatorsToEngine(EngineFactory.CreateEngine());

            server.Start();

        }

        [Test]
        public void TestServerClientGame()
        {
            var game = new GodGameServerClient();

            // Create
            var server = game.CreateServer(null);
            var client = game.CreateClient(null);

            // Connect
            server.Start();

            Thread.Sleep(1000);

            client.ConnectToServer("127.0.0.1", server.TcpPort);

            // Initialize gameloop

            var engine = EngineFactory.CreateEngine();

            server.AddSimulatorsToEngine(engine);
            client.AddSimulatorsToEngine(engine);

        }

        [Test]
        public void TestServerClientGameVirtual()
        {
            var game = new GodGameServerClient();

            // Virtual connection setup

            var virtualNetworkConnectorServer = new VirtualNetworkConnectorServer();
            var virtualNetworkConnectorClient = virtualNetworkConnectorServer.CreateClient();

            // Create
            var server = game.CreateServer(virtualNetworkConnectorServer);
            var client = game.CreateClient(virtualNetworkConnectorClient);

            // Initialize gameloop

            var engine = EngineFactory.CreateEngine();

            server.AddSimulatorsToEngine(engine);
            client.AddSimulatorsToEngine(engine);

        }

        [Test]
        public void TestOfflineGame()
        {

            var builder = new ContainerBuilder();
            builder.RegisterModule<ExternalDependenciesModule>();
            builder.RegisterModule<PersistenceModule>();
            builder.RegisterModule<RenderingModule>();
            builder.RegisterModule<SimulationModule>();
            builder.RegisterModule<UserInputModule>();
            builder.RegisterType<GodGameOffline>();
            builder.Register(
                ctx => GodGameServerClient.createWorld(ctx.Resolve<LandType>(), ctx.Resolve<ProxyGenerator>())).SingleInstance();

            // Wire: register a single global playerstate
            builder.RegisterType<LocalPlayerService>().SingleInstance();
            builder.Register(ctx => ctx.Resolve<LocalPlayerService>().Player).SingleInstance();

            //TODO: fix localplayerservice

            var cont = builder.Build();

            var offline = cont.Resolve<GodGameOffline>();

            var engine = EngineFactory.CreateEngine();
            offline.AddSimulatorsToEngine(engine);

        }

        /*public static GodGameOffline CreateGame()
        {
            var world = new Internal.Model.World(100, 10);
            buildDemoWorld(world);




            var ret = new GodGameOffline(EngineFactory.CreateEngine(), world);

            return ret;
        }*/

        private static void buildDemoWorld(Internal.Model.World world, VoxelTypesFactory typesFactory)
        {
            world.ForEach((v, p) =>
                {
                    if (Vector2.Distance(p, new Vector2(100, 100)) < 100)
                    {
                        v.Data.Type = typesFactory.Get<LandType>();
                    }
                    else if (Vector2.Distance(p, new Vector2(8, 8)) < 7)
                    {
                        v.Data.Type = typesFactory.Get<LandType>();
                    }
                    else if (Vector2.Distance(p, new Vector2(25, 25)) < 15)
                    {
                        v.Data.Type = typesFactory.Get<LandType>();
                    }
                    //v.ChangeType(GameVoxelType.Infestation);
                    else
                    {
                        v.Data.Type = typesFactory.Get<LandType>();
                    }
                });


            /*var worldPersister = new WorldPersister(getTypeFromName, getItemFromName);
            var simpleWorldRenderer = new SimpleWorldRenderer(world);
            var ret = new GodGameOffline(EngineFactory.CreateEngine(),
                world,
                new UserInputProcessingService(createPlayerInputs(world).ToArray(), world, worldPersister, simpleWorldRenderer),
                worldPersister,
                simpleWorldRenderer);

            return ret;*/
        }


    }
}