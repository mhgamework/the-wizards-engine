using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings
{
    /// <summary>
    /// Allows placing and randomizing islands
    /// Note that a randomized island randomizes island structure (voxels), as well as island decorations (trees, rocks)
    /// </summary>
    public class IslandToolItem : IHotbarItem
    {
        public string Name { get { return "IslandTool"; } }
        public void OnSelected()
        {

        }

        public void OnDeselected()
        {
        }

        public void Update()
        {
        }
    }
}