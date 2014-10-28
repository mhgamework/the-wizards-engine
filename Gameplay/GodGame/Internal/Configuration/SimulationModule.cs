using System;
using System.Linq;
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
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration
{
    /// <summary>
    /// Configures the Simulation components, that is the gameplay part of the game
    /// </summary>
    public class SimulationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Gamestate
            builder.RegisterType<GameState>().SingleInstance();
            builder.RegisterType<ClearGameStateChangesService>().SingleInstance();

            // Other
            builder.RegisterType<WorldSimulationService>().SingleInstance();


            loadVoxelTypes(builder);
        }

        private void loadVoxelTypes(ContainerBuilder builder)
        {
            Type[] types = ThisAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(GameVoxelType))).ToArray();
            builder.RegisterTypes(types)
                   .Except<BuildingSiteType>()
                   .AsSelf()
                   .As<IGameVoxelType>()
                   .OnActivated(
                       o => ((GameVoxelType)o.Instance).InjectVoxelTypesFactory(o.Context.Resolve<VoxelTypesFactory>()))
                   .SingleInstance();

            builder.RegisterType<MonumentVoxelBrain>();

            builder.RegisterType<VoxelTypesFactory>().SingleInstance();

            builder.RegisterType<ItemTypesFactory>().SingleInstance();
        }
    }
}