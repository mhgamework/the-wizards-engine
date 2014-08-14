using System;
using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Rendering;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// TODO: handle kanban cleanup when voxels are changed type
    /// </summary>
    public class RoadType : GameVoxelType
    {
        private FourWayModelBuilder meshBuilder;
        private ItemType outgoingKanbanType;
        private ItemType incomingKanbanType;

        public RoadType()
            : base("Road")
        {
            meshBuilder = new FourWayModelBuilder
            {
                BaseMesh = datavalueMeshes.ContainsKey(0) ? datavalueMeshes[0] : null,
                WayMesh = datavalueMeshes.ContainsKey(1) ? datavalueMeshes[1] : null,
                NoWayMesh = datavalueMeshes.ContainsKey(2) ? datavalueMeshes[2] : null
            };

            outgoingKanbanType = new ItemType() { Name = "OutgoingKanban" };
            outgoingKanbanType.Mesh = UtilityMeshes.CreateBoxColored(Color.Red, new Vector3(0.5f));

            incomingKanbanType = new ItemType() { Name = "IncomingKanban" };
            incomingKanbanType.Mesh = UtilityMeshes.CreateBoxColored(Color.Green, new Vector3(0.5f));



        }

        public void DeliverItemClosest(IVoxelHandle startRoad, IVoxelHandle sourceInventory, ItemType type)
        {
            DeliverItem(startRoad, sourceInventory,
                findClosestPossibleDestination(type, startRoad).Item1, type);
        }
        /// <summary>
        /// Takes an item from source, and moves it to the given destination. The item enters the road at given startRoad
        /// </summary>
        public void DeliverItem(IVoxelHandle startRoad, IVoxelHandle sourceInventory, IVoxelHandle targetInventory, ItemType type)
        {
            if (!FindConnectedVoxels(startRoad, v => v.Equals(targetInventory)).Any()) throw new InvalidOperationException("Target is not connected to given startRoad");

            sourceInventory.Data.Inventory.TransferItemsTo(startRoad.Data.Inventory, type, 1);
            sourceInventory.Data.Inventory.AddNewItems(outgoingKanbanType, 1);
            targetInventory.Data.Inventory.AddNewItems(incomingKanbanType, 1);

            startRoad.Data.Road.Items.Add(new TargetedItem(sourceInventory.GetInternalVoxel(), type, targetInventory.GetInternalVoxel()));

        }

        public override void Tick(IVoxelHandle handle)
        {
            handle.Data.DataValue += (int)(handle.TickLength * 1000);
            if (handle.Data.DataValue < 500) return;

            handle.Data.DataValue = 0;

            foreach (var targetedItem in handle.Data.Road.Items.ToArray())
                tryTransfer(handle, targetedItem);


        }

        private void tryTransfer(IVoxelHandle road, TargetedItem item)
        {
            if (tryTransferToDestination(road, item)) return;

            if (tryMoveCloserTo(road, haxorIVoxelHandelize(road, item.Destination), item)) return;
            if (tryMoveCloserTo(road, haxorIVoxelHandelize(road, item.Source), item)) return;

            destroyTargetedItem(road, item);
        }

        private IVoxelHandle haxorIVoxelHandelize(IVoxelHandle callingVoxel, GameVoxel voxel)
        {
            return new IVoxelHandle(callingVoxel.GetInternalVoxel().World, voxel);

        }

        /// <summary>
        /// This causes permanent removal of an in game item, used when the user somehow causes an in-transport item to have nowhere to go
        /// </summary>
        /// <param name="item"></param>
        private void destroyTargetedItem(IVoxelHandle handle, TargetedItem item)
        {
            handle.Data.Inventory.DestroyItems(item.ItemType, 1);
            item.Source.Data.Inventory.DestroyItems(outgoingKanbanType, 1);
            item.Destination.Data.Inventory.DestroyItems(incomingKanbanType, 1);
        }

        private bool tryTransferToDestination(IVoxelHandle road, TargetedItem item)
        {
            if (!road.Get4Connected().Contains(haxorIVoxelHandelize(road, item.Destination))) return false;

            item.Source.Data.Inventory.DestroyItems(outgoingKanbanType, 1);
            item.Destination.Data.Inventory.DestroyItems(incomingKanbanType, 1);
            road.Data.Inventory.TransferItemsTo(item.Destination.Data.Inventory, item.ItemType, 1);
            road.Data.Road.Items.Remove(item);
            return true;
        }

        private bool tryMoveCloserTo(IVoxelHandle road, IVoxelHandle target, TargetedItem item)
        {
            if (!FindConnectedVoxels(road, v => v.Equals(target)).Any()) return false; // target not reacheable
            var neighbours = getNeighbourRoads(road);
            var options = neighbours.Select(
                n =>
                new { Road = n, Target = FindConnectedVoxels(n, v => v.Equals(target)).FirstOrDefault() });
            var bestNextRoad = options.Where(o => o.Target != null).OrderBy(o => o.Target.Item2).First().Road;


            road.Data.Inventory.TransferItemsTo(bestNextRoad.Data.Inventory, item.ItemType, 1);
            road.Data.Road.Items.Remove(item);
            bestNextRoad.Data.Road.Items.Add(item);

            return true;
        }

        private Tuple<IVoxelHandle, int> findClosestPossibleDestination(ItemType type, IVoxelHandle road)
        {
            return FindConnectedInventories(road, type).OrderBy(t => t.Item2).FirstOrDefault();
        }


        public override bool CanAcceptItemType(IVoxelHandle voxelHandle, ItemType type)
        {
            if (type == incomingKanbanType) return false;
            if (type == outgoingKanbanType) return false;
            var potentialTargets = FindConnectedInventories(voxelHandle, type).ToArray();
            if (!potentialTargets.Any()) return false;

            return true;
        }
        /// <summary>
        /// Returns a list of connected inventories ordered by proximity. Each tuple contains (inventory, distance).
        /// </summary>
        /// <param name="start"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Tuple<IVoxelHandle, int>> FindConnectedInventories(IVoxelHandle start, ItemType type)
        {
            return FindConnectedVoxels(start, v => v.CanAcceptItemType(type));
        }


        public IEnumerable<Tuple<IVoxelHandle, int>> FindConnectedVoxels(IVoxelHandle startRoad, Func<IVoxelHandle, bool> predicate)
        {
            if (!(startRoad.Type is RoadType)) throw new InvalidOperationException();

            var toVisit = new Queue<Tuple<IVoxelHandle, int>>();
            var visited = new HashSet<IVoxelHandle>();

            toVisit.Enqueue(new Tuple<IVoxelHandle, int>(startRoad, 0));

            while (toVisit.Any())
            {
                var currT = toVisit.Dequeue();
                var curr = currT.Item1;
                var currentDist = currT.Item2;

                foreach (var neighbour in getNeighbourRoads(curr))
                {
                    if (visited.Contains(neighbour)) continue;
                    visited.Add(neighbour);
                    toVisit.Enqueue(new Tuple<IVoxelHandle, int>(neighbour, currentDist + 1));
                }

                foreach (var target in curr.Get4Connected().Where(v => !(v.Type is RoadType) && predicate(v)))
                    yield return new Tuple<IVoxelHandle, int>(target, currentDist);

            }
        }

        private static IEnumerable<IVoxelHandle> getNeighbourValidInventories(ItemType type, IVoxelHandle curr)
        {
            return curr.Get4Connected().Where(v => !(v.Type is RoadType) && v.CanAcceptItemType(type));
        }

        private static IEnumerable<IVoxelHandle> getNeighbourRoads(IVoxelHandle curr)
        {
            return curr.Get4Connected().Where(v => v.Type is RoadType);
        }



        public override IEnumerable<IVoxelInfoVisualizer> GetInfoVisualizers(IVoxelHandle handle)
        {
            yield break;
        }

        public override System.Collections.Generic.IEnumerable<IVoxelInfoVisualizer> GetCustomVisualizers(IVoxelHandle handle)
        {

            var inv = new InventoryVisualizer(handle);
            inv.ItemRelativeTransformationProvider = i =>
                {
                    return InventoryVisualizer.getItemCircleTransformation(i, handle.GetInternalVoxel(), 0.25f);
                };
            yield return inv;
        }

        public override bool DontShowDataValue { get { return true; } }

        public override IMesh GetMesh(IVoxelHandle handle)
        {
            var conn = handle.Get4Connected().ToArray();
            return meshBuilder.CreateMesh(isConnectedType(conn[0]), isConnectedType(conn[1]), isConnectedType(conn[2]), isConnectedType(conn[3]));
        }

        private bool isConnectedType(IVoxelHandle handle)
        {
            return handle.Type is RoadType || handle.Type is FarmType || handle.Type is WarehouseType || handle.Type is MinerType;
        }

        public struct RoadData
        {
            public List<TargetedItem> Items;

            public static RoadData Empty { get { return new RoadData() { Items = new List<TargetedItem>() }; } }
        }
    }

    public class TargetedItem
    {
        public GameVoxel Source;
        public ItemType ItemType;
        public GameVoxel Destination;

        public TargetedItem(GameVoxel source, ItemType itemType, GameVoxel destination)
        {
            Source = source;
            ItemType = itemType;
            Destination = destination;
        }
    }
}