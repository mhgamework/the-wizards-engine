using System.Collections.Generic;
using System.Collections.ObjectModel;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore
{
    public class GroupInventoryNode : IInventoryNode
    {
        private List<IInventoryNode> children = new List<IInventoryNode>();

        public GroupInventoryNode()
        {
            Children = new ReadOnlyCollection<IInventoryNode>(children);
        }

        public ReadOnlyCollection<IInventoryNode> Children { get; private set; }

        public void AddChild(IInventoryNode node)
        {
            children.Add(node);
        }
    }
}