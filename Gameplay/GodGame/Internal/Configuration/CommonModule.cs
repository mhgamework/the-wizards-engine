using Autofac;
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
    /// Idea: merge the server and client module into this module, and use a constructor switch between client and server
    /// The reason is that the  common, server and client module are very linked, whereas the idea of modules is to configure independent components
    /// Now the common is used to place all things that are the same between server and client, which is exactly the configuration of the whole server+client construct, which
    /// should be one module
    /// Configures components for running the game as a networked client
    /// </summary>
    public class CommonModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Engine
            builder.RegisterInstance(EngineFactory.CreateEngine()).SingleInstance();


            // Gamestate
            builder.RegisterType<GameState>().SingleInstance();
            builder.RegisterType<ClearGameStateChangesService>().SingleInstance();

            // User input
            builder.RegisterType<PlayerToolsFactory>().SingleInstance();
            builder.RegisterType<UserInputService>().SingleInstance();

            // Shared networking
            builder.RegisterType<GameStateDeltaPacketBuilder>().SingleInstance();


            // Serialization
            builder.RegisterType<WorldPersisterService>().SingleInstance();
            builder.RegisterType<GameplayObjectsSerializer>().SingleInstance();

        }
    }
}