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

        public override void Tick(Internal.IVoxelHandle handle)
        {
            handle.EachRandomInterval(1f, () => tryFish(handle));
        }

        private void tryFish(IVoxelHandle handle)
        {
            if (!handle.Get4Connected().Any(e => e.Type is WaterType))
                return;

            var target = handle.Get4Connected().FirstOrDefault(v => v.CanAcceptItemType(fishType));
            if (target == null) return;

            target.Data.Inventory.AddNewItems(fishType, 1);
        }
    }
}
