using System;
using System.Linq;
using Autofac;
using Castle.DynamicProxy;
using DirectX11;
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
            //builder.RegisterType<Model.World>().SingleInstance();
            builder.Register<Func<Model.World, Point2, GameVoxel>>(ctx =>
                {
                    var proxyGenerator = ctx.Resolve<ProxyGenerator>();
                    return (w, p) => new GameVoxel(w, p, proxyGenerator);
                }).AsSelf().SingleInstance();

            // Other
            builder.RegisterType<WorldSimulationService>().SingleInstance();

            // Register all types in the Types namespaces (should be renamed to gameplay namespace)
            builder.RegisterTypes(
                ThisAssembly.GetTypes()
                            .Where(
                                t =>
                                t.Namespace != null && t.Namespace.StartsWith("MHGameWork.TheWizards.GodGame.Types")).ToArray())
                   .AsSelf()
                   .SingleInstance();
            // Auto-load all modules in the types namespace
            var gameplayModules = ThisAssembly.GetTypes()
                           .Where(
                               t =>
                               t.Namespace != null && t.Namespace.StartsWith("MHGameWork.TheWizards.GodGame.Types"))
                               .Where(t => t.IsAssignableTo<Module>()).ToArray();
            foreach (var gameplayModule in gameplayModules)
            {
                var inst = (Module)Activator.CreateInstance(gameplayModule);
                builder.RegisterModule(inst);
            }

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