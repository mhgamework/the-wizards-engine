using System;
using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class RoadType : GameVoxelType
    {
        public RoadType()
            : base("Road")
        {


        }

        public override void Tick(IVoxelHandle handle)
        {
            handle.Data.Inventory.ChangeCapacity(1);

            handle.EachRandomInterval(0.5f, () => tryTransfer(handle));

        }

        private void tryTransfer(IVoxelHandle handle)
        {
            if (handle.Data.Inventory.ItemCount == 0) return;

            var type = handle.Data.Inventory.Items.First();

            if (tryTransferToNeighbourInventory(handle, type)) return;

            tryTransferToBestNeighbour(handle, type);
        }

        private bool tryTransferToNeighbourInventory(IVoxelHandle handle, ItemType type)
        {
            var neighbours = getNeighbourValidInventories(type, handle);
            if (!neighbours.Any()) return false;

            handle.Data.Inventory.TransferItemsTo(neighbours.First().Data.Inventory, type, 1);

            return true;
        }

        private void tryTransferToBestNeighbour(IVoxelHandle handle, ItemType type)
        {
            var neighbours = getNeighbourRoads(handle);
            var options = neighbours.Select(
                n =>
                new { Road = n, Target = findConnectedInventories(n, type).OrderBy(t => t.Item2).FirstOrDefault() });
            var target = options.Where(o => o.Target != null).OrderBy(o => o.Target.Item2).FirstOrDefault().With(v => v.Road);

            if (target == null)
                return;

            handle.Data.Inventory.TransferItemsTo(target.Data.Inventory, type, 1);
        }


        public override bool CanAcceptItemType(IVoxelHandle voxelHandle, ItemType type)
        {
            var potentialTargets = findConnectedInventories(voxelHandle, type).ToArray();
            if (!potentialTargets.Any()) return false;

            return true;
        }

        private IEnumerable<Tuple<IVoxelHandle, int>> findConnectedInventories(IVoxelHandle start, ItemType type)
        {
            var toVisit = new Queue<Tuple<IVoxelHandle, int>>();
            var visited = new HashSet<IVoxelHandle>();

            toVisit.Enqueue(new Tuple<IVoxelHandle, int>(start, 0));

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

                foreach (var target in getNeighbourValidInventories(type, curr))
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
                    return Matrix.Translation(0, 2, 0);
                };
            yield return inv;
        }
    }
}