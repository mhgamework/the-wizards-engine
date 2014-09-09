using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Networking;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration
{
    public class ServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(EngineFactory.CreateEngine()).SingleInstance();
            builder.RegisterType<GodGameServer>().SingleInstance();

            builder.RegisterType<GameState>().SingleInstance();

            builder.RegisterType<TickSimulator>().SingleInstance();
            builder.RegisterType<ClearStateChangesSimulator>().SingleInstance();
            builder.RegisterType<PlayerState>().SingleInstance();

            builder.Register(createNetworkedPlayerFactory).SingleInstance();

            builder.RegisterType<PlayerInputHandler>();
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