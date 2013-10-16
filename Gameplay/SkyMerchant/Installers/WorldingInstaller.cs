using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1._Tests;
using MHGameWork.TheWizards.SkyMerchant.Worlding;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.Installers
{
    /// <summary>
    /// Installs the quest editor's production dependencies
    /// </summary>
    public class WorldingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IWorldLocator>().ImplementedBy<SimpleWorldLocator>());
            container.Register(Component.For<IWorld>().ImplementedBy<Worlding.World>());
            //container.Register(Component.For<worldo>());
            //container.Register(Component.For<LocalPlayer>());

            //container.Register(Classes.FromThisAssembly()
            //       .InNamespace("MHGameWork.TheWizards.SkyMerchant.Worlding", true)
            //       .WithServiceSelf()
            //       .WithServiceDefaultInterfaces());
        }
    }
}