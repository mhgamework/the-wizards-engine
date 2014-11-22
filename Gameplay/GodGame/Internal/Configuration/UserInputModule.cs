using Autofac;
using Castle.DynamicProxy;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using MHGameWork.TheWizards.GodGame.Internal.Inputting;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.GodGame.Persistence;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration
{
    /// <summary>
    /// Configures the User input components
    /// TODO: split this into 'gameplay player logic' and 'hardware/api user input'
    ///     then move the gameplay player logic to the simulationModule
    /// </summary>
    public class UserInputModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PlayerToolsFactory>().SingleInstance();
            builder.RegisterType<UserInputService>().SingleInstance();
            builder.RegisterType<UserInputProcessingService>().SingleInstance();

            builder.RegisterType<ActiveToolInputHandler>().As<IPlayerInputHandler>().AsSelf();

            builder.RegisterModule<ToolSelectionMenuModule>();


            builder.RegisterType<NullPlayerTool>();


        }
    }
}