using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace MHGameWork.TheWizards.SkyMerchant.Installers
{
    /// <summary>
    /// Installs the production use dependencies to the engine
    /// </summary>
    public class EngineInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                .InNamespace("MHGameWork.TheWizards.SkyMerchant._Engine")
                .WithServiceSelf().
                WithServiceDefaultInterfaces());
        }

    }
}