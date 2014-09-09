using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// A grinder accepts stone and outputs pigment items.
    /// In order to work, the grinder must be next to a water voxel, with max height difference 10.
    /// </summary>
    public class GrinderType : GameVoxelType
    {
        private int inventorySize = 5;

        private ItemType pigmentType;
        private ItemType oreType;

        private int datavalGrindRangeLow = 1000;
        private int datavalGrindRangeHigh;

        private static int counter = 0;
        private Timer myTimer;

        public GrinderType()
            : base("Grinder")
        {
            pigmentType = new ItemType { Name = "Pigment", Mesh = UtilityMeshes.CreateBoxColored(Color.DarkViolet, new Vector3(1)) };
            oreType = Ore.GetOreItemType(null);
            datavalGrindRangeHigh = datavalGrindRangeLow + inventorySize;

            myTimer = new Timer(100);
            myTimer.Elapsed += incrementCounter;
            myTimer.Enabled = true;
        }

        private static void incrementCounter(Object source, ElapsedEventArgs e)
        {
            counter++;
            if (counter >= 4) //nb animation frames
                counter = 0;
        }

        public override void Tick(IVoxelHandle handle)
        {
            handle.Data.Inventory.ChangeCapacity(inventorySize);

            handle.EachRandomInterval(1f, () => tryGatherResources(handle));
            handle.EachRandomInterval(1f, () => tryGrind(handle));
            handle.EachRandomInterval(1f, () => tryDeliverPigment(handle));
        }

        public override IMesh GetMesh(IVoxelHandle handle)
        {
            IMesh tmp;
            if (!hasWaterNeighbour(handle))
                tmp = GetDataValueMesh(999);//error mesh
            else
            {
                var ret = (handle.Data.DataValue > datavalGrindRangeLow && handle.Data.DataValue < datavalGrindRangeHigh) ? GetDataValueMesh(counter) : GetDataValueMesh(0);
                tmp = MeshBuilder.Transform(ret, getYRot(handle));
            }

            var meshBuilder = new MeshBuilder();
            meshBuilder.AddMesh(tmp, Matrix.Identity);
            var groundMesh = GetDefaultGroundMesh(handle.Data.Height);
            if (groundMesh == null) return tmp;
            meshBuilder.AddMesh(groundMesh, Matrix.Identity);
            return meshBuilder.CreateMesh();
        }

        private Matrix getYRot(IVoxelHandle handle)
        {
            var yRot = Matrix.Identity;
            var neigbours = handle.Get4Connected().ToList();
            if (checkIsAccesibleWater(handle, neigbours[0]))
                yRot = Matrix.RotationY((float)Math.PI * 0.5f);
            if (checkIsAccesibleWater(handle, neigbours[2]))
                yRot = Matrix.RotationY(-(float)Math.PI * 0.5f);
            if (checkIsAccesibleWater(handle, neigbours[3]))
                yRot = Matrix.RotationY((float)Math.PI);
            return yRot;
        }

        private bool checkIsAccesibleWater(IVoxelHandle currentHandle, IVoxelHandle handleToCheck)
        {
            return handleToCheck.Type is WaterType &&
                   Math.Abs(Math.Abs(handleToCheck.Data.Height - currentHandle.Data.Height) - 0f) < 0.001f;
        }

        private void tryGatherResources(IVoxelHandle handle)
        {
            //if grinding or delivering or inventory full
            if (handle.Data.DataValue >= datavalGrindRangeLow || countResourcesIncludingKanban(pigmentType, handle) > 0 || handle.Data.Inventory.ItemCount >= inventorySize) return;

            var warehousesInRange = getWareHousesInRange(handle, 100);
            foreach (var warehouse in warehousesInRange.Where(warehouse => warehouse.Data.Inventory.GetAmountOfType(oreType) > 0))
            {
                var road = Road.IsConnected(warehouse, handle);
                if (road != null)
                {
                    Road.DeliverItem(road, warehouse, handle, oreType);
                    break;
                }
            }
        }

        private void tryGrind(IVoxelHandle handle)
        {
            if (!hasWaterNeighbour(handle))
            {
                handle.Data.DataValue = 0; return;
            }
            if (handle.Data.Inventory.GetAmountOfType(oreType) < inventorySize && handle.Data.DataValue < datavalGrindRangeLow) return;
            if (countResourcesIncludingKanban(pigmentType, handle) != 0) return; //deliver state

            handle.Data.DataValue = handle.Data.DataValue < datavalGrindRangeLow
                                        ? datavalGrindRangeLow
                                        : handle.Data.DataValue + 1;

            if (handle.Data.DataValue >= datavalGrindRangeHigh)
            {
                handle.Data.Inventory.AddNewItems(pigmentType, 1);
                return;
            }

            handle.Data.Inventory.DestroyItems(oreType, 1);
        }

        private void tryDeliverPigment(IVoxelHandle handle)
        {
            if (handle.Data.Inventory.GetAmountOfType(pigmentType) == 0) return;

            var target = handle.Get4Connected().FirstOrDefault(v => v.CanAcceptItemType(pigmentType));

            if (target == null) return;
            if (!target.CanAcceptItemType(pigmentType)) return;
            if (!(target.Type is RoadType)) return;

            Road.DeliverItemClosest(target, handle, pigmentType);
            handle.Data.DataValue = 0;
        }

        private bool hasWaterNeighbour(IVoxelHandle handle)
        {
            return handle.Get4Connected().Any(e => checkIsAccesibleWater(handle, e));
        }

        private int countResourcesIncludingKanban(ItemType type, IVoxelHandle handle)
        {
            var inventory = handle.Data.Inventory;
            return inventory.GetAmountOfType(type) + inventory.GetAmountOfType(Road.GetIncomingKanban(type)) + inventory.GetAmountOfType(Road.GetOutgoingKanban(type));
        }

        private IEnumerable<IVoxelHandle> getWareHousesInRange(IVoxelHandle handle, int range)
        {
            return handle.GetRange(range).Where(v => v.Type is WarehouseType);
        }

        public override IEnumerable<IRenderable> GetInfoVisualizers(IVoxelHandle handle)
        {
            yield break;
        }

        public override IEnumerable<IRenderable> GetCustomVisualizers(IVoxelHandle handle)
        {
            var inv = new InventoryVisualizer(handle);
            inv.ItemRelativeTransformationProvider = i =>
            {
                var size = 5;
                var x = (float)(i % size);
                var y = (float)(i / size);
                var tileSize = 10;

                var cellSize = tileSize / (float)size;

                x -= size / 2f;
                y -= size / 2f;


                return Matrix.Scaling(MathHelper.One * 0.5f) *
                       Matrix.Translation(cellSize * (x + 0.5f), 0.5f, cellSize * (y + 0.5f)) * getYRot(handle);
            };
            yield return inv;
        }

    }
}
