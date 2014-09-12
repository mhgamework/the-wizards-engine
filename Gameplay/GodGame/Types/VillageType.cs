using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// Villages need a number of different resources to operate (a number of each resource type).
    /// Resources types can only be those processed by markets.
    /// Villages consume these resources over time.
    /// If no resources of a type are left, the village is disabled.
    /// 
    /// Dataval explanation:
    /// dataval == 0 => not supplied
    /// dataval > 0 => supplied
    /// if(dataval > 1) 
    ///     dataval - 1 == nb workers providing
    /// </summary>
    public class VillageType : GameVoxelType
    {
        private readonly ItemTypesFactory itemTypesFactory;
        private int totalResourceCapacity;
        private int maxNbWorkers = 10;
        private int workerSupplyRange = 10;

        private struct VillageResource
        {
            public ItemType ItemType;
            public int MaxResourceLevel; //max nb of resources of this type this village can store
            public int MinResourceLevel; //min nb of resources of this type this village needs to operate
            public int ConsummationRate; //average time to consume one resource of this type
        }

        private List<VillageResource> neededResources;

        public VillageType(ItemTypesFactory itemTypesFactory)
            : base("Village")
        {
            this.itemTypesFactory = itemTypesFactory;
            neededResources = new[]
                {
                    new VillageResource { ItemType = itemTypesFactory.CropType, MaxResourceLevel = 3, MinResourceLevel = 1, ConsummationRate = 10}, 
                    new VillageResource { ItemType = itemTypesFactory.FishType, MaxResourceLevel = 2, MinResourceLevel = 1, ConsummationRate = 10}
                }.ToList();

            totalResourceCapacity = 0;
            foreach (var res in neededResources)
            {
                totalResourceCapacity += res.MaxResourceLevel;
            }
        }

        public override void Tick(IVoxelHandle handle)
        {
            //todo: should not be done every tick??
            handle.Data.Inventory.ChangeCapacity(totalResourceCapacity); //should be done at start
            checkResourceLevels(handle); //should be done at start (and is also done after each consume)

            foreach (var resource in neededResources)
            {
                handle.EachRandomInterval(resource.ConsummationRate, () => consume(resource, handle));
            }

            handle.EachRandomInterval(1f, () => trySupplyWorkers(handle));

        }

        #region workers

        private void trySupplyWorkers(IVoxelHandle handle)
        {
            if (getNbWorkersSupplied(handle) >= maxNbWorkers || !isSupplied(handle))
                return;

            var voxelsInRange = handle.GetRange(workerSupplyRange);
            foreach (var vh in voxelsInRange)
            {
                if (vh.CanAddWorker())
                {
                    vh.Data.WorkerCount++;
                    handle.Data.DataValue++;
                    return;
                }
            }
        }
        
        private int getNbWorkersSupplied(IVoxelHandle handle)
        {
            var val = handle.Data.DataValue;
            return val == 0 ? 0 : val - 1;
        }

        private void removeSuppliedWorkers(IVoxelHandle handle)
        {
            var nbWorkersSupplied = getNbWorkersSupplied(handle);

            var voxelsInRange = handle.GetRange(workerSupplyRange);
            foreach (var vh in voxelsInRange)
            {
                if (nbWorkersSupplied == 0)
                    return;

                var nbw = vh.Data.WorkerCount;
                if (nbw <= 0) continue;
                if (nbw > nbWorkersSupplied)
                {
                    vh.Data.WorkerCount -= nbWorkersSupplied;
                    nbWorkersSupplied = 0;
                }
                else
                {
                    vh.Data.WorkerCount = 0;
                    nbWorkersSupplied -= nbw;
                }
            }

            if (nbWorkersSupplied == 0)
                return;

            throw new Exception("Worker leak!!");
        }
        
        #endregion workers

        #region resources

        private void checkResourceLevels(IVoxelHandle handle)
        {
            foreach (var resource in neededResources)
            {
                if (!hasEnough(resource, handle))
                {
                    removeSuppliedWorkers(handle);
                    setSupplyState(handle, false); //not supplied
                    return;
                }
            }
            setSupplyState(handle, true); //all supplied
        }
        
        private bool hasEnough(VillageResource resource, IVoxelHandle handle)
        {
            return handle.Data.Inventory.GetAmountOfType(resource.ItemType) >= resource.MinResourceLevel;
        }
        
        private void setSupplyState(IVoxelHandle handle, bool setSupplied)
        {
            if (isSupplied(handle) == setSupplied) return;
            handle.Data.DataValue = setSupplied ? 1 : 0;
        }
        
        private void consume(VillageResource resource, IVoxelHandle handle)
        {
            if (handle.Data.Inventory.GetAmountOfType(resource.ItemType) == 0) return;

            //var itemToConsume = handle.Data.Inventory.Items.ToArray()[rnd.Next(handle.Data.Inventory.ItemCount)];
            handle.Data.Inventory.DestroyItems(resource.ItemType, 1);
            checkResourceLevels(handle);
        }

        #endregion resources

        public override bool CanAcceptItemType(IVoxelHandle handle, IVoxelHandle deliveryHandle, ItemType type)
        {
            if (!(deliveryHandle.Type is MarketType)) return false;
            if (!neededResources.Any(e => e.ItemType == type)) return false;

            return getNbItemsOfTypeOrKanban(handle.Data.Inventory, type) < neededResources.First(e => e.ItemType == type).MaxResourceLevel;
        }

        private int getNbItemsOfTypeOrKanban(Inventory inventory, ItemType type)
        {
            return inventory.Items.Sum(item => itemTypesFactory.IsItemOrKanbanOfType(type, item) ? 1 : 0);
        }

        private bool isSupplied(IVoxelHandle handle)
        {
            return handle.Data.DataValue > 0;
        }

        public override IMesh GetMesh(IVoxelHandle handle)
        {
            var tmp = isSupplied(handle) ? datavalueMeshes[1] : datavalueMeshes[0];

            var meshBuilder = new MeshBuilder();
            meshBuilder.AddMesh(tmp, Matrix.Identity);
            var groundMesh = GetDefaultGroundMesh(handle.Data.Height, Color.SaddleBrown);
            if (groundMesh == null) return tmp;
            meshBuilder.AddMesh(groundMesh, Matrix.Identity);
            return meshBuilder.CreateMesh();
        }
        
        public override IEnumerable<IRenderable> GetInfoVisualizers(IVoxelHandle handle)
        {
            yield return new RangeVisualizer(handle, workerSupplyRange);
        }
    }
}