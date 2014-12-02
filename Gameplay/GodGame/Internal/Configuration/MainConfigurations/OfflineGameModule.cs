using Autofac;
using Castle.DynamicProxy;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal.Networking;
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration.MainConfigurations
{
    /// <summary>
    /// Configures everything to get the offline game working
    /// </summary>
    public class OfflineGameModule :Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ExternalDependenciesModule>();
            builder.RegisterModule<PersistenceModule>();
            builder.RegisterModule<RenderingModule>();
            builder.RegisterModule<SimulationModule>();
            builder.RegisterModule<UserInputModule>();
            builder.RegisterType<GodGameOffline>();
            /*builder.Register(
                ctx => GodGameServerClient.createWorld(ctx.Resolve<LandType>(), ctx.Resolve<ProxyGenerator>())).SingleInstance();*/

            // Wire: register a single global playerstate
            builder.RegisterType<LocalPlayerService>().SingleInstance();
            builder.Register(ctx => ctx.Resolve<LocalPlayerService>().Player).SingleInstance();

        }
    }
}