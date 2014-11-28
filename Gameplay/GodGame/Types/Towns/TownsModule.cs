using Autofac;
using MHGameWork.TheWizards.GodGame.Types.Towns.Data;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    public class TownsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx => ctx.Resolve<GenericDatastore>().RootRecord)
                .SingleInstance()
                .As<GenericDatastoreRecord>();
        }
    }
}