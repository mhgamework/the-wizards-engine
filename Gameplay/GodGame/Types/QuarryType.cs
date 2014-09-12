using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
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
        private int neededWorkerCount;

        public QuarryType(ItemTypesFactory itemTypeFactory)
            : base("Quarry")
        {
            stoneType = itemTypeFactory.StoneType;
            maxDataVal = datavalOffset + batchSize * 5;
            neededWorkerCount = 5;
        }

        public override void Tick(IVoxelHandle handle)
        {
            handle.Data.Inventory.ChangeCapacity(inventoryCapacity);
            handle.Data.DataValue = handle.Data.DataValue < datavalOffset ? datavalOffset : handle.Data.DataValue;

            if(handle.Data.WorkerCount > 0)
            {
                handle.EachRandomInterval(getEfficiency(handle, 1f, 1f * neededWorkerCount), () => tryExcavate(handle));
                handle.EachRandomInterval(getEfficiency(handle, 5f, 5f * neededWorkerCount), () =>
                {
                    tryOutput(handle);
                    checkIfDepleted(handle);
                });
            }
        }

        public override bool CanAddWorker(IVoxelHandle handle)
        {
            return handle.Data.WorkerCount < neededWorkerCount;
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

            IMesh tmp;
            datavalueMeshes.TryGetValue(index, out tmp);
            if (tmp == null)
                tmp = UtilityMeshes.CreateBoxColored(Color.FromArgb(255, 86, 86, 86), new Vector3(0.5f, 0.05f, 0.5f));

            var meshBuilder = new MeshBuilder();
            meshBuilder.AddMesh(tmp, Matrix.Identity);
            var groundMesh = GetDefaultGroundMesh(handle.Data.Height, Color.FromArgb(255, 86, 86, 86));
            if (groundMesh == null) return tmp;
            meshBuilder.AddMesh(groundMesh, Matrix.Translation(0, -0.9f, 0));
            return meshBuilder.CreateMesh();
        }

        /// <summary>
        /// efficiency values are times used for eachrandominterval. !! maxefficiency is less than or equal to minefficiency !!
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="maxEfficiency"></param>
        /// <param name="minEfficiency"></param>
        /// <returns></returns>
        private float getEfficiency(IVoxelHandle handle, float maxEfficiency, float minEfficiency)
        {
            return minEfficiency - (float)handle.Data.WorkerCount / (float)neededWorkerCount * (minEfficiency - maxEfficiency);
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

        public override IEnumerable<IRenderable> GetCustomVisualizers(IVoxelHandle handle)
        {
            var workerVis = new WorkerCountVisualizer(handle, neededWorkerCount);
            yield return workerVis;
        }
    }
}
