using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Autofac;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal.Configuration;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame._Tests;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// The serverclient combines a server and a client into a single window. This means that there exist 2 gamestates in parallel.
    /// Should be extended to allow remote connecting via console etc.
    /// </summary>
    public class GodGameServerClient
    {

        private GodGameServer server;
        private GodGameClient client;
        public GodGameServerClient()
        {

            var bServer = new ContainerBuilder();
            bServer.RegisterModule<ServerModule>();



            var bClient = new ContainerBuilder();
            bClient.RegisterModule<ClientModule>();


            bServer.RegisterInstance(createWorld()).SingleInstance();
            bClient.RegisterInstance(createWorld()).SingleInstance();

            bServer.RegisterInstance(new WorldPersisterService(null, null)).SingleInstance();
            bClient.RegisterInstance(new WorldPersisterService(null, null)).SingleInstance();


            bServer.RegisterType<CreateLandTool>().As<IPlayerTool>().SingleInstance();
            bServer.RegisterType<CreateLandTool>().As<IPlayerTool>().SingleInstance();

            server = bServer.Build().Resolve<GodGameServer>();
            client = bClient.Build().Resolve<GodGameClient>();



            ConnectLocal();

            var engine = EngineFactory.CreateEngine();

            engine.AddSimulator(new BasicSimulator(() => Simulate()));
        }

        private static Model.World createWorld()
        {
            var world = new Model.World(200, 10);

            world.ForEach((v, _) => v.ChangeType(GameVoxelType.Land));
            return world;
        }


        public void Simulate()
        {
            server.Tick();

            client.Tick();


        }

        public void ConnectLocal()
        {
            server.Start();

            Thread.Sleep(1000);

            client.ConnectToServer("localhost", server.TcpPort);
        }

    }
}