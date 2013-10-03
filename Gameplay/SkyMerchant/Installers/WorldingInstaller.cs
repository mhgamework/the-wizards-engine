using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace MHGameWork.TheWizards.SkyMerchant.Worlding
{
    /// <summary>
    /// Installs the quest editor's production dependencies
    /// </summary>
    public class WorldingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                   .InNamespace("MHGameWork.TheWizards.SkyMerchant.Worlding",true)
                   .WithServiceSelf()
                   .WithServiceDefaultInterfaces());
        }
    }
}