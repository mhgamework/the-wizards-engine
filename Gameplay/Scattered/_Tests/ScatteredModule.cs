using System.Reflection;
using Autofac;
using MHGameWork.TheWizards.Scattered.Bindings;
using MHGameWork.TheWizards.Scattered.GameLogic;
using Module = Autofac.Module;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    /// <summary>
    /// This is the main module loading all scattered dependencies
    /// </summary>
    public class ScatteredModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Logging
            builder.RegisterModule<LogRequestsModule>();

            // configuration based binding
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(t => t.Name.EndsWith("Simulator")).SingleInstance();

            // Binding for gameplay objects
            builder.RegisterModule<GameLogicModule>();

            // Core behaviour binding for scattered
            builder.RegisterModule<BindingsModule>(); 
        }
    }
}