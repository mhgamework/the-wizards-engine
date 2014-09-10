﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Autofac;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
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
        public GodGameServerClient(bool virtualConnection)
        {

            // Virtual connection setup

            var virtualNetworkConnectorServer = new VirtualNetworkConnectorServer();
            var virtualNetworkConnectorClient = virtualNetworkConnectorServer.CreateClient();

            // Server

            var bServer = new ContainerBuilder();
            bServer.RegisterModule<CommonModule>();
            bServer.RegisterModule<ServerModule>();
            bServer.RegisterInstance(createWorld()).SingleInstance();

            if (virtualConnection)
                bServer.RegisterInstance(virtualNetworkConnectorServer).As<INetworkConnectorServer>().SingleInstance();


            server = bServer.Build().Resolve<GodGameServer>();

            //Client

            var bClient = new ContainerBuilder();
            bClient.RegisterModule<CommonModule>();
            bClient.RegisterModule<ClientModule>();
            bClient.RegisterInstance(createWorld()).SingleInstance();
            bClient.Register(ctx => new AllCommandProvider(ctx.Resolve<WorldPersisterService>(), server.World)).As<ICommandProvider>();

            if (virtualConnection)
                bClient.RegisterInstance(virtualNetworkConnectorClient).As<INetworkConnectorClient>().SingleInstance();

            client = bClient.Build().Resolve<GodGameClient>();

            ConnectLocal();


            // Initialize gameloop

            var engine = EngineFactory.CreateEngine();

            server.AddSimulatorsToEngine(engine);
            client.AddSimulatorsToEngine(engine);



            

        }

        private static Model.World createWorld()
        {
            var world = new Model.World(100, 10);

            world.ForEach((v, _) => v.ChangeType(GameVoxelType.Land));
            return world;
        }

        public void ConnectLocal()
        {
            server.Start();

            Thread.Sleep(1000);

            client.ConnectToServer("127.0.0.1", server.TcpPort);
        }

    }
}