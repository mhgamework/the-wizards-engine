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
    /// Configures the persistence components
    /// </summary>
    public class PersistenceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            // Serialization
            builder.RegisterType<WorldPersisterService>().SingleInstance();
            builder.RegisterType<GameplayObjectsSerializer>().SingleInstance();

        }
    }
}