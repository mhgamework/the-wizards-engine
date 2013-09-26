using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore
{
    public class WireframeInventoryNodeRenderer : IInventoryNodeRenderer
    {
        public bool MakeVisible(IInventoryNode parentItem, BoundingBox bb)
        {
            TW.Graphics.LineManager3D.AddBox(bb, new Color4(0, 0, 0));
            return true;
        }
    }
}