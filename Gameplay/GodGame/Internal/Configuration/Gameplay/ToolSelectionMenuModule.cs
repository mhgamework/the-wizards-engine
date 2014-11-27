using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using MHGameWork.TheWizards.GodGame.ToolSelection;
using MHGameWork.TheWizards.GodGame.Types;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration
{
    /// <summary>
    /// Configuration for the ToolSelectionMenu
    /// Included in the UserInputModule
    /// </summary>
    public class ToolSelectionMenuModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ToolSelectionMenu>().SingleInstance().OnActivated(a => initMenu(a.Instance, a.Context));
            builder.RegisterType<ToolMenuBuilder>().SingleInstance();
            builder.RegisterType<ToolSelectionCategory>();
            builder.RegisterType<ToolSelectionTool>();

            builder.RegisterTypes(ThisAssembly.GetTypes().Where(t => t.IsAssignableTo<PlayerTool>()).ToArray())
                   .AsSelf()
                   .Keyed<PlayerTool>(type => type);
        }

        private void initMenu(ToolSelectionMenu menu, IComponentContext context)
        {
            menu.Initialize(context.Resolve<ToolMenuBuilder>().BuildMenu());
        }

    }
}