using System.Collections.ObjectModel;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    /// <summary>
    /// Represents a node in a hierarchial inventory.
    /// Note that the children returned should have some consistency, in the sense that it should return the same children, when children did not change.
    /// TODO: probably rename
    /// </summary>
    public interface IInventoryNode
    {
        ReadOnlyCollection<IInventoryNode> Children { get; }
    }
}