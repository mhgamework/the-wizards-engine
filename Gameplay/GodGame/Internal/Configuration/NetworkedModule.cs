using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Networking;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Networking;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration
{
    /// <summary>
    /// Configures components for running the game with networking capabilities
    /// </summary>
    public class NetworkedModule : Module
    {
        private NetworkedModule(bool isServer)
        {
            this.isServer = isServer;
        }

        public static NetworkedModule CreateServer()
        {
            return new NetworkedModule(true);
        }
        public static NetworkedModule CreateClient()
        {
            return new NetworkedModule(false);

        }

        private bool isServer;

        protected override void Load(ContainerBuilder builder)
        {
            // Shared networking
            builder.RegisterType<GameStateDeltaPacketBuilder>().SingleInstance();

            if (isServer)
                loadServer(builder);
            else
                loadClient(builder);

        }

        private static void loadClient(ContainerBuilder builder)
        {
            builder.RegisterType<GodGameClient>().SingleInstance();

            builder.RegisterType<LocalPlayerService>().SingleInstance();

            // Override the player input handler with a proxy input handler
            builder.Register(ctx =>
                {
                    var connector = ctx.Resolve<INetworkConnectorClient>();
                    return
                        new ProxyPlayerInputHandler(
                            new LazyClientPacketTransporter<UserInputHandlerPacket>(() => connector.UserInputHandlerTransporter));
                }).As<IPlayerInputHandler>().SingleInstance();


            builder.RegisterType<NetworkConnectorClient>().As<INetworkConnectorClient>().SingleInstance();
        }

        private void loadServer(ContainerBuilder builder)
        {
            builder.RegisterType<GodGameServer>().SingleInstance();

            builder.Register(createNetworkedPlayerFactory).SingleInstance();

            builder.RegisterInstance(new NetworkConnectorServer(15005, 15006)).As<INetworkConnectorServer>();
        }

        private NetworkedPlayerFactory createNetworkedPlayerFactory(IComponentContext ctx)
        {
            var state = ctx.Resolve<GameState>();
            var createPlayerInputHandler =
                ctx.Resolve<Func<PlayerState, PlayerInputHandler>>();

            return new NetworkedPlayerFactory((transporter, handler) => new NetworkPlayerInputForwarder(transporter, handler, state.World), createPlayerInputHandler, state);
        }
    }
}