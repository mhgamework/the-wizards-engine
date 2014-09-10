using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
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
                if (!target.CanAcceptItemType(fishType)) break;
                if (handle.Data.Inventory[fishType] == 0) break;
                if (target.Type is RoadType)
                    Road.DeliverItemClosest(target, handle, fishType);
            }
        }

        private void tryFish(IVoxelHandle handle)
        {
            if (!hasWaterNeighbour(handle))
                return;

            if (handle.Data.Inventory.CanAdd(fishType, 1))
                handle.Data.Inventory.AddNewItems(fishType, 1);
        }

        public override IMesh GetMesh(IVoxelHandle handle)
        {
            IMesh tmp = GetDataValueMesh(!hasWaterNeighbour(handle) ? 999 : handle.Data.DataValue);

            var meshBuilder = new MeshBuilder();
            meshBuilder.AddMesh(tmp, Matrix.Identity);
            var groundMesh = GetDefaultGroundMesh(handle.Data.Height);
            if (groundMesh == null) return tmp;
            meshBuilder.AddMesh(groundMesh, Matrix.Identity);
            return meshBuilder.CreateMesh();
        }

        private void updateDataVal(IVoxelHandle handle)
        {
            handle.Data.DataValue = handle.Data.Inventory.GetAmountOfType(fishType);
        }

        private bool hasWaterNeighbour(IVoxelHandle handle)
        {
            return handle.Get4Connected().Any(e => checkIsAccesibleWater(handle, e));
        }

        private bool checkIsAccesibleWater(IVoxelHandle currentHandle, IVoxelHandle handleToCheck)
        {
            return handleToCheck.Type is WaterType &&
                   Math.Abs(Math.Abs(handleToCheck.Data.Height - currentHandle.Data.Height) - 0f) < 0.001f;
        }
    }
}
