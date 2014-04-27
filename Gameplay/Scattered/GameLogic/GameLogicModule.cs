using Autofac;
using MHGameWork.TheWizards.Scattered.GameLogic.Services;
using MHGameWork.TheWizards.Scattered.Model;
using Autofac.Extras.AggregateService;

namespace MHGameWork.TheWizards.Scattered.GameLogic
{
    public class GameLogicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            registerDefaults(builder);

            // Objects

            // Single player config!
            builder.Register(c => c.Resolve<Level>().LocalPlayer).SingleInstance();


            // Services

            // Config enemy spawn rate
            builder.Register(c => new EnemySpawningService(c.Resolve<Level>(), 100)).SingleInstance();


        }

        private void registerDefaults(ContainerBuilder builder)
        {
            var myNamespace = typeof (GameLogicModule).Namespace;


            builder.RegisterAssemblyTypes(ThisAssembly)
                   .Where(t => t.Namespace != null && t.Namespace.StartsWith(myNamespace + ".Objects"));

            builder.RegisterAssemblyTypes(ThisAssembly)
                   .Where(t => t.Namespace != null && t.Namespace.StartsWith(myNamespace + ".Services"))
                   .SingleInstance();

            builder.RegisterType<GameSimulationService>().SingleInstance();

            builder.Register(c => c.Resolve<Level>().Node.CreateChild()); // TODO: this is verrrry shady

            builder.RegisterAggregateService<WorldGenerationService.IGameObjectsFactory>();
        }
    }
}