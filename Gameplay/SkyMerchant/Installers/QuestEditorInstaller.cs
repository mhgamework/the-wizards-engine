using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.Scripting;

namespace MHGameWork.TheWizards.SkyMerchant.Installers
{
    /// <summary>
    /// Installs the quest editor's production dependencies
    /// </summary>
    public class QuestEditorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ScriptsRepository>().UsingFactoryMethod(k => new ScriptsRepository(k)));
            
            container.Register(Classes.FromThisAssembly()
                   .InNamespace("MHGameWork.TheWizards.SkyMerchant.QuestEditor", true)
                   .WithServiceSelf()
                   .WithServiceDefaultInterfaces());
        }
    }
}