using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types.Transportation.Generic;
using MHGameWork.TheWizards.GodGame.Types.Transportation.Services;
using System.Linq;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Types.Transportation
{
    public class Puller : ITickable, IRoadComponent
    {
        public float PushSpeed = 1;



        private float nextTick = 0;
        private IVoxel voxel;

        public Puller(IVoxel voxel)
        {
            this.voxel = voxel;
        }

        public void Tick()
        {
            foreach (var item in voxel.Data.Inventory.Items.Where(i => !i.Name.Contains("Kanban")).ToArray())
            {
                if (!WarehouseService.CanMoveToWarehouse(voxel, item.Single()))
                    continue; // This is currently a problem scenario, something should happen here (send back, block, ...)

                voxel.Data.Inventory.DestroyItems(item, 1);
                WarehouseService.MoveToWarehouse(voxel, item.Single());
            }
        }

        public bool CanAcceptItem(IVoxel voxel, ItemType item)
        {
            return WarehouseService.CanMoveToWarehouse(voxel, new[] { item });
        }
    }
}