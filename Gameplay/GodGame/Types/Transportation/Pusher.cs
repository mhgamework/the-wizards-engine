using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types.Transportation.Generic;
using MHGameWork.TheWizards.GodGame.Types.Transportation.Services;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types.Transportation
{
    public class Pusher : ITickable
    {
        public float PushSpeed = 1;

        private float nextTick = 0;
        private IVoxel voxel;

        public Pusher(IVoxel voxel)
        {
            this.voxel = voxel;
        }

        public void Tick()
        {
            if (TW.Graphics.TotalRunTime < nextTick) return;
            nextTick = TW.Graphics.TotalRunTime + PushSpeed;

            var item = WarehouseService.GetItemsInWarehouses(voxel).FirstOrDefault();
            if (item == null) return;
            //TODO: if cant push first item, try items of other types too!
            if (!RoadService.CanPush(voxel, item)) return;
            WarehouseService.TakeFromWarehouse(voxel, new[] { item });
            RoadService.Push(voxel, item);

        }
    }
}