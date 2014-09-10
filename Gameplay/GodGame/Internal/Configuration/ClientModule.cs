using Autofac;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Networking;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration
{
    /// <summary>
    /// Configures components for running the game as a networked client
    /// </summary>
    public class ClientModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GodGameClient>().SingleInstance();
            builder.RegisterType<WorldRenderingSimulator>().SingleInstance();


            builder.RegisterType<UserInputProcessingService>().SingleInstance();
            builder.RegisterType<UIRenderingService>().SingleInstance();
            builder.RegisterType<DeveloperConsoleService>().SingleInstance();
            builder.RegisterType<ClearGameStateChangesService>().SingleInstance();
            builder.RegisterType<WorldRenderingService>().SingleInstance();

            builder.RegisterType<GameState>().SingleInstance();
            builder.Register(ctx =>
                {
                    var state = ctx.Resolve<GameState>();
                    var p = new PlayerState();
                    state.AddPlayer(p);
                    return p;
                }).SingleInstance();
            builder.Register(ctx =>
            {
                var connector = ctx.Resolve<INetworkConnectorClient>();
                return new ProxyPlayerInputHandler(new LazyClientPacketTransporter<UserInputPacket>(() => connector.UserInputTransporter));
            }).As<IPlayerInputHandler>().SingleInstance();


            builder.RegisterType<NetworkConnectorClient>().As<INetworkConnectorClient>().SingleInstance();
            builder.RegisterType<ChunkedVoxelWorldRenderer>().As<IVoxelWorldRenderer>().SingleInstance();

            builder.RegisterType<AllCommandProvider>().As<ICommandProvider>().SingleInstance();
        }
    }
}