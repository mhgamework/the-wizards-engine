using Autofac;
using Castle.DynamicProxy;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.GodGame.Persistence;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration
{
    /// <summary>
    /// Configures services that exposes third-party functionality 
    /// (third party is considered to be all non-godgame)
    /// </summary>
    public class ExternalDependenciesModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Engine
            builder.RegisterInstance(EngineFactory.CreateEngine()).SingleInstance();

            // Windsor
            builder.RegisterType<ProxyGenerator>().SingleInstance();

        }
    }
}