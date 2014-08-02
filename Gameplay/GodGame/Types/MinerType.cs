using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class MinerType : GameVoxelType
    {
        private int mineRadius = 3;

        public MinerType()
            : base("Miner")
        {

        }
        public override void Tick(IVoxelHandle handle)
        {
            // TODO: should actually be done on type change of voxel, not every tick
            handle.Data.Inventory.ChangeCapacity(5);


            handle.EachRandomInterval(1, () => tryMine(handle));
        }

        private void tryMine(IVoxelHandle handle)
        {
            var mineable = getMineableVoxels(handle);
            if (!mineable.Any()) return;
            var target = mineable.First();

            ((OreType)target.Type).MineResource(target, handle.Data.Inventory);
        }



        private IEnumerable<IVoxelHandle> getMineableVoxels(IVoxelHandle handle)
        {
            return handle.GetRange(mineRadius).Where(v => v.Type is OreType);
        }

        public override IEnumerable<IVoxelInfoVisualizer> GetInfoVisualizers(IVoxelHandle handle)
        {
            foreach (var e in base.GetInfoVisualizers(handle))
                yield return e;

            yield return new RangeVisualizer(handle, mineRadius);
            yield return new HighlightVoxelsVisualizer(handle, getMineableVoxels);
            yield return new InventoryVisualizer(handle);
        }


    }
}