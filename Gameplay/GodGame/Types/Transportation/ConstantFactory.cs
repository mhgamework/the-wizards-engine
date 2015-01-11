using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types.Transportation.Generic;
using MHGameWork.TheWizards.GodGame.Types.Transportation.Services;
using MHGameWork.TheWizards.Scattered.Model;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types.Transportation
{
    public class ConstantFactory : ITickable
    {
        private readonly IVoxel handle;

        public ConstantFactory(IVoxel handle)
        {
            this.handle = handle;
        }

        public float Rate = 10;

        public ItemType[] ItemsToGenerate = new ItemType[0];

        public void MoveItemsIntoNearbyWarehouse(ItemType[] items)
        {
            foreach (var i in items)
            {
                var warehouse = handle.Get4Connected().FirstOrDefault(v => v.Data.Type is WarehouseType && v.Data.Inventory.AvailableSlots > 0);
                if (warehouse == null) return;
                warehouse.Data.Inventory.AddNewItems(i, 1);
            }
        }

        private float nextTick;
        public void Tick()
        {
            //TODO: use scheduler
            if (nextTick > TW.Graphics.TotalRunTime) return;
            nextTick = TW.Graphics.TotalRunTime + Rate;
            WarehouseService.MoveToWarehouse(handle,ItemsToGenerate);
        }

    }
}