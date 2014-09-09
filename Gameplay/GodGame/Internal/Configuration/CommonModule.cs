using Autofac;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.GodGame.Persistence;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration
{
    /// <summary>
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


            // Shared networking
            builder.RegisterType<GameStateDeltaPacketBuilder>().SingleInstance();


            // Serialization
            builder.RegisterType<WorldPersisterService>().SingleInstance();
            builder.RegisterType<GameplayObjectsSerializer>().SingleInstance();

        }
    }
}