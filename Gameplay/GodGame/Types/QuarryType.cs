using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// The quarry excavates stone. After every batch (10 items), the mesh is updated. After 5 batches, the quarry is depleted and changes into a hole.
    /// </summary>
    public class QuarryType : GameVoxelType
    {
        private ItemType stoneType;
        private int inventoryCapacity = 10;
        private int batchSize = 10;
        private int datavalOffset = 100;
        private int maxDataVal;

        public QuarryType()
            : base("Quarry")
        {
            stoneType = new ItemType { Name = "Stone", Mesh = UtilityMeshes.CreateBoxColored(Color.Gray, new Vector3(1)) };
            maxDataVal = datavalOffset + batchSize * 5;
        }

        public override void Tick(IVoxelHandle handle)
        {
            handle.Data.Inventory.ChangeCapacity(inventoryCapacity);
            handle.Data.DataValue = handle.Data.DataValue < datavalOffset ? datavalOffset : handle.Data.DataValue;

            handle.EachRandomInterval(1f, () => tryExcavate(handle));
            handle.EachRandomInterval(5f, () =>
                {
                    tryOutput(handle);
                    checkIfDepleted(handle);
                });
        }

        public ItemType GetStoneItemType()
        {
            return stoneType;
        }

        public override IMesh GetMesh(IVoxelHandle handle)
        {
            if (handle.Data.DataValue < datavalOffset)
                handle.Data.DataValue = datavalOffset;
            var index = (int)Math.Floor((handle.Data.DataValue - datavalOffset) / (float)batchSize);
            if (index > 4)
                index = 4;

            IMesh ret;
            datavalueMeshes.TryGetValue(index, out ret);
            if (ret == null)
                return UtilityMeshes.CreateBoxColored(Color.Gray, new Vector3(0.5f, 0.05f, 0.5f));
            return ret;
        }

        private void tryExcavate(IVoxelHandle handle)
        {
            if (!handle.Data.Inventory.CanAdd(stoneType, 1) || handle.Data.DataValue >= maxDataVal) return;
            handle.Data.Inventory.AddNewItems(stoneType, 1);
            handle.Data.DataValue++;
        }

        private void tryOutput(IVoxelHandle handle)
        {
            if (handle.Data.Inventory.GetAmountOfType(stoneType) == 0) return;

            var target = handle.Get4Connected().FirstOrDefault(v => v.CanAcceptItemType(stoneType));
            if (target == null || !target.CanAcceptItemType(stoneType)) return;

            for (int i = 0; i < inventoryCapacity; i++)
            {
                if (!target.CanAcceptItemType(stoneType)) break;
                if (handle.Data.Inventory[stoneType] == 0) break;
                if (target.Type is RoadType)
                    Road.DeliverItemClosest(target, handle, stoneType);
            }
        }

        private void checkIfDepleted(IVoxelHandle handle)
        {
            if (handle.Data.DataValue >= maxDataVal && handle.Data.Inventory.ItemCount == 0)
                handle.ChangeType(Hole);
        }
    }
}
