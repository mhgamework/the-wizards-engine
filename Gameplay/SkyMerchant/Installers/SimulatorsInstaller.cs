using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MHGameWork.TheWizards.SkyMerchant.GameObjects;

namespace MHGameWork.TheWizards.SkyMerchant.Installers
{
    /// <summary>
    /// Installs all simulators for skymerchant
    /// </summary>
    public class SimulatorsInstaller:IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<RelativePositioningSimulator>());
        }
    }
}