using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Networking;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Networking;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration
{
    /// <summary>
    /// Configures components for Rendering
    /// </summary>
    public class RenderingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            loadConsole(builder);

            builder.RegisterType<WorldRenderingSimulator>().SingleInstance();
            builder.RegisterType<UIRenderingService>().SingleInstance();

            builder.RegisterType<WorldRenderingService>().SingleInstance();

            builder.RegisterType<ChunkedVoxelWorldRenderer>().As<IVoxelWorldRenderer>().SingleInstance();

        }

        private static void loadConsole(ContainerBuilder builder)
        {
            builder.RegisterType<DeveloperConsoleService>().SingleInstance();
            builder.RegisterType<AllCommandProvider>().As<ICommandProvider>().SingleInstance();
        }
    }
}