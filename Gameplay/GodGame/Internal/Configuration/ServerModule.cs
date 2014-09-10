using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Networking;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration
{
    /// <summary>
    /// Configures components for running the game as a networked server
    /// </summary>
    public class ServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GodGameServer>().SingleInstance();

            builder.RegisterType<WorldSimulationService>().SingleInstance();
            builder.RegisterType<ClearGameStateChangesService>().SingleInstance();
            //builder.RegisterType<PlayerState>();

            builder.Register(createNetworkedPlayerFactory).SingleInstance();

            builder.RegisterType<PlayerInputHandler>();

            builder.RegisterInstance(new NetworkConnectorServer(15005, 15006)).As<INetworkConnectorServer>();

        }

        private NetworkedPlayerFactory createNetworkedPlayerFactory(IComponentContext ctx)
        {
            var state = ctx.Resolve<GameState>();
            var createPlayerInputHandler =
                ctx.Resolve<Func<PlayerState, PlayerInputHandler>>();

            //TODO: register networkedplayerfactory?
            return new NetworkedPlayerFactory((transporter, handler) => new NetworkPlayerInputForwarder(transporter, handler, state.World), createPlayerInputHandler, state);
        }
    }
}