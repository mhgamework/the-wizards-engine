using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class FarmType : GameVoxelType
    {
        private static int counter = 0;
        private Timer myTimer;

        public FarmType()
            : base("Farm")
        {
            Color = Color.Purple;
            myTimer = new Timer(100);
            myTimer.Elapsed += incrementCounter;
            myTimer.Enabled = true;
        }

        public override void Tick(IVoxelHandle handle)
        {
            // TODO: should actually be done on type change of voxel, not every tick
            handle.Data.Inventory.ChangeCapacity(5);

            handle.EachRandomInterval(1, () =>
                {
                    tryHarvest(handle);
                    tryOutput(handle);
                });

            updateDataVal(handle);
        }

        private void tryOutput(IVoxelHandle handle)
        {
            if (handle.Data.Inventory.ItemCount == 0) return;
            var type = handle.Data.Inventory.Items.First();
            var target = handle.Get4Connected().FirstOrDefault(v => v.CanAcceptItemType(type));

            if (target == null) return;

            for (int i = 0; i < 5; i++)
            {
                if (!target.CanAcceptItemType(type)) break;
                if (handle.Data.Inventory[type] == 0) break;
                if (target.Type is RoadType)
                    Road.DeliverItemClosest(target, handle, type);
                else
                    handle.Data.Inventory.TransferItemsTo(target.Data.Inventory, type, 1);
            }

        }

        private void tryHarvest(IVoxelHandle handle)
        {
            var toHarvest = getHarvestableVoxels(handle);
            if (!toHarvest.Any()) return;
            var target = toHarvest.First();

            ((CropType)target.Type).Harvest(target, handle.Data.Inventory);
        }

        private static void incrementCounter(Object source, ElapsedEventArgs e)
        {
            counter++;
            if (counter >= 6)
                counter = 0;
        }

        private void updateDataVal(IVoxelHandle handle)
        {
            handle.Data.DataValue = counter;
        }

        private IEnumerable<IVoxelHandle> getHarvestableVoxels(IVoxelHandle handle)
        {
            return handle.Get8Connected().Where(v => v.Type is CropType && v.Data.DataValue >= CropType.HarvestDataVal);
        }

        public override IEnumerable<IRenderable> GetInfoVisualizers(IVoxelHandle handle)
        {
            foreach (var e in base.GetInfoVisualizers(handle))
                yield return e;

            yield return new RangeVisualizer(handle, 1);
            yield return new HighlightVoxelsVisualizer(handle, getHarvestableVoxels);
            //yield return new InventoryVisualizer(handle);
        }

    }
}
