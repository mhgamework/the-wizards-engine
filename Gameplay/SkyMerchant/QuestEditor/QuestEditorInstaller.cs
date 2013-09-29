using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor
{
    /// <summary>
    /// Installs the quest editor's production dependencies
    /// </summary>
    public class QuestEditorInstaller:IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IInventoryNode>().UsingFactoryMethod(delegate(IKernel input)
                {
                    var builder = input.Resolve<DefaultInventoryBuilder>();
                    return builder.CreateTree();
                }));

            container.Register(Classes.FromThisAssembly()
                   .InNamespace("MHGameWork.TheWizards.SkyMerchant.QuestEditor",true)
                   .WithServiceSelf()
                   .WithServiceDefaultInterfaces());
        }
    }
}