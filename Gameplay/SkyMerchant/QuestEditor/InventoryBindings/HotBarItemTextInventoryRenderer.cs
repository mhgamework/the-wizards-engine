using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings
{
    public class HotBarItemTextInventoryRenderer : IInventoryNodeRenderer
    {
        private readonly IInventoryNodeRenderer decorated;

        public HotBarItemTextInventoryRenderer(IInventoryNodeRenderer decorated)
        {
            this.decorated = decorated;
        }

        public bool MakeVisible(IInventoryNode item, BoundingBox bb)
        {
            var hbi = item as HotBarItemInventoryNode;
            if (hbi == null)
                return decorated.MakeVisible(item, bb);

            var phys = getPhysical(hbi.Item);

            phys.WorldMatrix =
            Matrix.Scaling(MathHelper.One * bb.GetSize().MinComponent()) *
            Matrix.Translation(bb.GetCenter());
            phys.Visible = true;

            return true;

        }

        private Dictionary<IHotbarItem, Physical> physicals = new Dictionary<IHotbarItem, Physical>();

        private Physical getPhysical(IHotbarItem item)
        {
            if (physicals.ContainsKey(item)) return physicals[item];

            var physical = new Physical();
            physical.Mesh = UtilityMeshes.CreateMeshWithText(0.5f, item.Name, TW.Graphics);

            physicals[item] = physical;

            return physical;
        }

        /// <summary>
        /// TODO: Run at frame start
        /// </summary>
        public void MakeAllInvisible()
        {
            foreach (var value in physicals.Values)
            {
                value.Visible = false;
            }
        }
    }
}