using Autofac;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Networking;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration
{
    public class ClientModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GodGameClient>().SingleInstance();
            builder.RegisterType<WorldRenderingSimulator>().SingleInstance();


            builder.RegisterType<PlayerInputSimulator>().SingleInstance();
            builder.RegisterType<UIRenderer>().SingleInstance();
            builder.RegisterType<DeveloperConsoleSimulator>().SingleInstance();
            builder.RegisterType<ClearStateChangesSimulator>().SingleInstance();
            builder.RegisterType<SimpleWorldRenderer>().SingleInstance();

            builder.RegisterType<GameState>().SingleInstance();
            builder.RegisterType<PlayerState>().SingleInstance();
            builder.Register(ctx =>
            {
                var connector = ctx.Resolve<NetworkConnectorClient>();
                return new ProxyPlayerInputHandler(new LazyClientPacketTransporter<UserInputPacket>(() => connector.UserInputTransporter));
            }).As<IPlayerInputHandler>().SingleInstance();


            builder.RegisterType<NetworkConnectorClient>().SingleInstance();
            builder.RegisterType<ChunkedVoxelWorldRenderer>().As<IVoxelWorldRenderer>().SingleInstance();

            builder.RegisterType<AllCommandProvider>().As<ICommandProvider>().SingleInstance();
        }
    }
}