using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings
{
    /// <summary>
    /// Shows meshes for the mesh spawner in the inventory renderer!
    /// </summary>
    public class MeshSpawnerInventoryRenderer : IInventoryNodeRenderer
    {
        private readonly IInventoryNodeRenderer decorated;
        private Dictionary<MeshSpawnerItem, Physical> physicals = new Dictionary<MeshSpawnerItem, Physical>();

        public MeshSpawnerInventoryRenderer(IInventoryNodeRenderer decorated)
        {
            this.decorated = decorated;
        }

        public bool MakeVisible(IInventoryNode item, BoundingBox bb)
        {
            var hbi = item as HotBarItemInventoryNode;
            if (hbi == null || !(hbi.Item is MeshSpawnerItem))
                return decorated.MakeVisible(item, bb);

            var phys = getPhysical(hbi.Item as MeshSpawnerItem);

            phys.WorldMatrix =
                Matrix.Scaling(MathHelper.One * bb.GetSize().MaxComponent()) *
                Matrix.Translation(bb.GetCenter());
            phys.Visible = true;

            return true;
        }

        private Physical getPhysical(MeshSpawnerItem item)
        {
            if (physicals.ContainsKey(item)) return physicals[item];

            var physical = new Physical();
            physical.Mesh = TW.Assets.LoadMesh(item.MeshPath);

            // Make unit size
            var meshBB = TW.Assets.GetBoundingBox(physical.Mesh);
            physical.ObjectMatrix =
                Matrix.Translation(-meshBB.GetCenter())*
                Matrix.Scaling(MathHelper.One/meshBB.GetSize().MaxComponent());

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