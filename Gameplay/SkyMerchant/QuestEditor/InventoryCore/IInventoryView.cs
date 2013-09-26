using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore
{
    public interface IInventoryView
    {
        void Update();
        IInventoryNode GetSelectedNode();
    }
}