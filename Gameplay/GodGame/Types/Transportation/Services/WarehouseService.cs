using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Types.Transportation.Services
{
    public static class WarehouseService
    {
        public static void MoveToWarehouse(IVoxel voxel, ItemType[] items)
        {
            foreach (var i in items)
            {
                var warehouse = GetConnectedWarehouses(voxel).FirstOrDefault(v => v.Data.Inventory.AvailableSlots > 0);
                if (warehouse == null) return;
                warehouse.Data.Inventory.AddNewItems(i, 1);
            }
        }

        public static bool CanMoveToWarehouse(IVoxel voxel, ItemType[] output)
        {
            return GetConnectedWarehouses(voxel).Sum(v => v.Data.Inventory.AvailableSlots) >= output.Length;
        }

        public static IEnumerable<IVoxel> GetConnectedWarehouses(IVoxel voxel)
        {
            return voxel.Get4Connected().Where(v => v.Data.Type is WarehouseType);
        }

        public static void TakeFromWarehouse(IVoxel voxel, ItemType[] input)
        {
            if (!CanTakeFromWarehouse(voxel, input)) throw new InvalidOperationException("Can't take items");
            foreach (var i in input)
            {
                var warehouse = GetConnectedWarehouses(voxel).FirstOrDefault(v => v.Data.Inventory.Items.Contains(i));
                if (warehouse == null) throw new InvalidOperationException("Algorithm error.");
                warehouse.Data.Inventory.DestroyItems(i, 1);
            }
        }

        public static bool CanTakeFromWarehouse(IVoxel handle, ItemType[] input)
        {
            IEnumerable<ItemType> allInputs = GetConnectedWarehouses(handle).SelectMany(v => v.Data.Inventory.Items);
            var counts = getCounts(allInputs);
            foreach (var c in getCounts(input))
            {
                if (!counts.ContainsKey(c.Key)) return false;
                if (counts[c.Key] < c.Value) return false;
            }
            return true;
        }

        public static IEnumerable<ItemType> GetItemsInWarehouses(IVoxel handle)
        {
            return GetConnectedWarehouses(handle).SelectMany(w => w.Data.Inventory.Items);
        }

        private static Dictionary<ItemType, int> getCounts(IEnumerable<ItemType> input)
        {
            return input.GroupBy(x => x).ToDictionary(group => @group.Key, group => @group.Count());
        }
    }
}