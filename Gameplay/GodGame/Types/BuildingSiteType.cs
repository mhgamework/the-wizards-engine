﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class BuildingSiteType : GameVoxelType
    {
        private GameVoxelType building;
        private List<ItemAmount> neededResources;
        private int totalNbNeededResources;

        public struct ItemAmount
        {
            public ItemType Type;
            public int Amount;
        }

        public BuildingSiteType(GameVoxelType building, List<ItemAmount> neededResources)
            : base(building.Name + "BuildingSite")
        {
            this.building = building;
            this.neededResources = neededResources;
            totalNbNeededResources = 0;
            foreach (var res in neededResources)
            {
                totalNbNeededResources += res.Amount;
            }

            Color = Color.White;
        }

        public override void Tick(IVoxelHandle handle)
        {
            handle.EachRandomInterval(0.25f, () =>
            {
                tryGatherResources(handle);
                updateAppearance(handle);
                checkComplete(handle);
            });
        }

        private void tryGatherResources(IVoxelHandle handle)
        {
            var warehousesInRange = getWareHousesInRange(handle, 100);
            foreach (var resAmnt in neededResources.Where(e => handle.Data.Inventory.GetAmountOfType(e.Type) < e.Amount))
            {
                foreach (var warehouse in warehousesInRange.Where(warehouse => warehouse.Data.Inventory.GetAmountOfType(resAmnt.Type) > 0))
                {
                    warehouse.Data.Inventory.TransferItemsTo(handle.Data.Inventory, resAmnt.Type, 1);
                    break;
                }
            }
        }

        private IEnumerable<IVoxelHandle> getWareHousesInRange(IVoxelHandle handle, int range)
        {
            return handle.GetRange(range).Where(v => v.Type is WarehouseType);
        }

        private void updateAppearance(IVoxelHandle handle)
        {
            var nbStoredRes = handle.Data.Inventory.ItemCount;
            var completionRate = (float)nbStoredRes / (float)totalNbNeededResources;

            //todo update dataVal instead, make models
            if (completionRate < 0.2f)
            {
                Color = Color.White;
                return;
            }
            if (completionRate < 0.4f)
            {
                Color = Color.LightGray;
                return;
            }
            if (completionRate < 0.6f)
            {
                Color = Color.DarkGray;
                return;
            }
            if (completionRate < 0.8f)
            {
                Color = Color.Gray;
            }

            Color = Color.Black;
        }

        private void checkComplete(IVoxelHandle handle)
        {
            if (neededResources.All(e => e.Amount == handle.Data.Inventory.GetAmountOfType(e.Type)))
                handle.ChangeType(building);
        }
    }
}