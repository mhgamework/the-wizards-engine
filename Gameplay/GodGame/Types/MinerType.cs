using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Types.Towns;
using MHGameWork.TheWizards.GodGame.Types.Towns.Workers;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class MinerType : GameVoxelType, IIndustryBuildingType
    {
        private int mineRadius = 3;

        public MinerType()
            : base("Miner")
        {

        }
        public override void Tick(IVoxelHandle handle)
        {
            // TODO: should actually be done on type change of voxel, not every tick
            handle.EachRandomInterval(1, () => tryMine(handle));
            handle.EachRandomInterval(1, () => tryOutput(handle));
            ReceiveCreationEvents = true;
        }

        private void tryOutput(IVoxelHandle handle)
        {
            if (handle.Data.Inventory.ItemCount == 0) return;

            for (int i = 0; i < 5; i++)
            {
                if (handle.Data.Inventory.ItemCount == 0) return;

                var type = handle.Data.Inventory.Items.First();
                var target = handle.Get4Connected().FirstOrDefault(v => v.CanAcceptItemType(type));
                if (target == null) break;

                if (target.Type is RoadType)
                    Road.DeliverItemClosest(target, handle, type);
                else
                    handle.Data.Inventory.TransferItemsTo(target.Data.Inventory, type, 1);
            }

        }

        private void tryMine(IVoxelHandle handle)
        {
            var mineable = getMineableVoxels(handle);
            if (!mineable.Any()) return;
            var target = mineable.First();

            ((OreType)target.Type).MineResource(target, handle.Data.Inventory);
        }



        private IEnumerable<IVoxelHandle> getMineableVoxels(IVoxelHandle handle)
        {
            return handle.GetRange(mineRadius).Where(v => v.Type is OreType);
        }

        public override IEnumerable<IRenderable> GetInfoVisualizers(IVoxelHandle handle)
        {
            foreach (var e in base.GetInfoVisualizers(handle))
                yield return e;

            yield return new RangeVisualizer(handle, mineRadius);
            yield return new HighlightVoxelsVisualizer(handle, getMineableVoxels);
            yield return new InventoryVisualizer(handle);
        }

        private Dictionary<IVoxelHandle, IWorkerConsumer> workerConsumers =
            new Dictionary<IVoxelHandle, IWorkerConsumer>();
        public override void OnCreated(IVoxelHandle handle)
        {
            handle.Data.Inventory.ChangeCapacity(5);
            workerConsumers[handle] = new SimpleWorkerConsumer() { RequestedWorkersCount = 5 };
        }

        public override void OnDestroyed(IVoxelHandle handle)
        {
            workerConsumers.Remove(handle);
        }

        public IWorkerConsumer GetWorkerConsumer(IVoxelHandle handle)
        {
            return workerConsumers[handle];
        }
    }
}