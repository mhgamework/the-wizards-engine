using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.Scripting;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor
{
    /// <summary>
    /// Installs the quest editor's production dependencies
    /// </summary>
    public class QuestEditorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ScriptsRepository>().UsingFactoryMethod(k => new ScriptsRepository(k)));
            container.Register(Classes.FromThisAssembly().BasedOn<IWorldScript>().LifestyleTransient());
            container.Register(Classes.FromThisAssembly()
                   .InNamespace("MHGameWork.TheWizards.SkyMerchant.QuestEditor", true)
                   .WithServiceSelf()
                   .WithServiceDefaultInterfaces());
        }
    }
}