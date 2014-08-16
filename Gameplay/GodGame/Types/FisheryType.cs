using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class FisheryType : GameVoxelType
    {
        private ItemType fishType;
        private int maxItemCount = 4;

        public FisheryType()
            : base("Fishery")
        {
            Color = Color.Blue;
            fishType = new ItemType { Name = "Fish", Mesh = UtilityMeshes.CreateBoxColored(Color.DeepSkyBlue, new Vector3(1)) };
        }

        public ItemType GetFishItemType()
        {
            return fishType;
        }

        public override void Tick(IVoxelHandle handle)
        {
            handle.Data.Inventory.ChangeCapacity(maxItemCount);
            handle.EachRandomInterval(1f, () =>
            {
                tryFish(handle);
                updateDataVal(handle);
            });
            handle.EachRandomInterval(1f, () => { tryOutput(handle); updateDataVal(handle); });
        }

        private void tryOutput(IVoxelHandle handle)
        {
            if (handle.Data.Inventory[fishType] < maxItemCount) return;

            var target = handle.Get4Connected().FirstOrDefault(v => v.CanAcceptItemType(fishType));
            if (target == null || !target.CanAcceptItemType(fishType)) return;

            for (int i = 0; i < maxItemCount; i++)
            {
                if (handle.Data.Inventory[fishType] == 0) break;
                if (target.Type is RoadType)
                    Road.DeliverItemClosest(target, handle, fishType);
            }
        }

        private void tryFish(IVoxelHandle handle)
        {
            if (!handle.Get4Connected().Any(e => e.Type is WaterType))
                return;

            if (handle.Data.Inventory.CanAdd(fishType, 1))
                handle.Data.Inventory.AddNewItems(fishType, 1);
        }

        private void updateDataVal(IVoxelHandle handle)
        {
            handle.Data.DataValue = handle.Data.Inventory.GetAmountOfType(fishType);
        }
    }
}
