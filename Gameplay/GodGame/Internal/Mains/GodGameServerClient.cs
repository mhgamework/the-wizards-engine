using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Autofac;
using Castle.DynamicProxy;
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

        public GodGameServerClient()
        {

        }

        public GodGameClient CreateClient(VirtualNetworkConnectorClient virtualNetworkConnectorClient)
        {
            var bClient = new ContainerBuilder();
            bClient.RegisterModule<ExternalDependenciesModule>();
            bClient.RegisterModule<SimulationModule>();
            bClient.RegisterModule<RenderingModule>();
            bClient.RegisterModule<UserInputModule>();
            bClient.RegisterModule<PersistenceModule>();
            bClient.RegisterModule(NetworkedModule.CreateClient());
            bClient.Register(ctx => createWorld(ctx.Resolve<LandType>(), ctx.Resolve<ProxyGenerator>())).SingleInstance();


            //TODO: re-enable the hack to allow client console to access the server
            // bClient.RegisterType<AllCommandProvider>().As<ICommandProvider>().WithParameter(TypedParameter.From(server.World));

            if (virtualNetworkConnectorClient != null)
                bClient.RegisterInstance(virtualNetworkConnectorClient).As<INetworkConnectorClient>().SingleInstance();

            return bClient.Build().Resolve<GodGameClient>();

        }

        public GodGameServer CreateServer(VirtualNetworkConnectorServer virtualNetworkConnectorServer)
        {
            var bServer = new ContainerBuilder();
            bServer.RegisterModule<ExternalDependenciesModule>();
            bServer.RegisterModule<SimulationModule>();
            bServer.RegisterModule<PersistenceModule>();
            bServer.RegisterModule<UserInputModule>();
            bServer.RegisterModule(NetworkedModule.CreateServer());
            bServer.Register(ctx => createWorld(ctx.Resolve<LandType>(), ctx.Resolve<ProxyGenerator>())).SingleInstance();

            if (virtualNetworkConnectorServer != null)
                bServer.RegisterInstance(virtualNetworkConnectorServer).As<INetworkConnectorServer>().SingleInstance();


            return bServer.Build().Resolve<GodGameServer>();
        }


        public static Model.World createWorld(LandType landType, ProxyGenerator proxyGenerator)
        {
            var world = TestWorldBuilder.createTestWorld(100, 10);

            world.ForEach((v, _) =>
                {
                    v.Data.Type = landType;
                });
            return world;
        }

    }
}