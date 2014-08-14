﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class FarmType : GameVoxelType
    {
        public FarmType()
            : base("Farm")
        {
            Color = Color.Purple;
        }

        public override void Tick(Internal.IVoxelHandle handle)
        {
            // TODO: should actually be done on type change of voxel, not every tick
            handle.Data.Inventory.ChangeCapacity(5);

            handle.EachRandomInterval(1, () =>
                {
                    tryHarvest(handle);
                    tryOutput(handle);
                });
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

        private IEnumerable<IVoxelHandle> getHarvestableVoxels(IVoxelHandle handle)
        {
            return handle.Get8Connected().Where(v => v.Type is CropType && v.Data.DataValue >= CropType.HarvestDataVal);
        }

        public override IEnumerable<IVoxelInfoVisualizer> GetInfoVisualizers(IVoxelHandle handle)
        {
            foreach (var e in base.GetInfoVisualizers(handle))
                yield return e;

            yield return new RangeVisualizer(handle, 1);
            yield return new HighlightVoxelsVisualizer(handle, getHarvestableVoxels);
            //yield return new InventoryVisualizer(handle);
        }

    }
}
