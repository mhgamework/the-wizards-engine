﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class WoodworkerType : GameVoxelType
    {
        private int harvestRange = 5;

        public WoodworkerType()
            : base("Woodworker")
        {
            Color = Color.SandyBrown;
        }

        public override void Tick(IVoxelHandle handle)
        {
            handle.Data.Inventory.ChangeCapacity(5);
            handle.EachRandomInterval(2f, () => tryHarvest(handle));
            handle.EachRandomInterval(2f, () => tryOutput(handle));
        }

        private void tryHarvest(IVoxelHandle handle)
        {
            var forestToHarvest = getHarvestableVoxels(handle).FirstOrDefault(e => e.Data.DataValue == ForestType.HarvestDataValue);
            if (forestToHarvest == null) return;

            if (handle.Data.Inventory.CanAdd(Forest.GetWoodItemType(), 1))
            {
                handle.Data.Inventory.AddNewItems(Forest.GetWoodItemType(), 1);
                forestToHarvest.Data.DataValue = ForestType.ResetDataValue;
            }

        }

        private void tryOutput(IVoxelHandle handle)
        {
            var itemType = Forest.GetWoodItemType();
            if (handle.Data.Inventory[itemType] == 0) return;

            var target = handle.Get4Connected().FirstOrDefault(v => v.CanAcceptItemType(itemType));
            if (target == null || !target.CanAcceptItemType(itemType)) return;

            if (target.Type is RoadType)
                Road.DeliverItemClosest(target, handle, itemType);
        }

        private IEnumerable<IVoxelHandle> getHarvestableVoxels(IVoxelHandle handle)
        {
            return handle.GetRange(harvestRange).Where(e => e.Type is ForestType);
        }

        public override IEnumerable<IVoxelInfoVisualizer> GetInfoVisualizers(IVoxelHandle handle)
        {
            foreach (var e in base.GetInfoVisualizers(handle))
                yield return e;

            yield return new RangeVisualizer(handle, harvestRange);
            yield return new HighlightVoxelsVisualizer(handle, getHarvestableVoxels);
            yield return new InventoryVisualizer(handle);
        }
    }
}
