using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// Villages need a number of different resources to operate (a number of each resource type).
    /// Resources types can only be those processed by markets.
    /// Villages consume these resources over time.
    /// If no resources of a type are left, the village is disabled.
    /// </summary>
    public class VillageType : GameVoxelType
    {
        private int totalResourceCapacity;

        private struct VillageResource
        {
            public ItemType ItemType;
            public int MaxResourceLevel; //max nb of resources of this type this village can store
            public int MinResourceLevel; //min nb of resources of this type this village needs to operate
            public int ConsummationRate; //average time to consume one resource of this type
        }

        private List<VillageResource> neededResources;

        private Random rnd;

        public VillageType()
            : base("Village")
        {
            neededResources = new[]
                {
                    new VillageResource { ItemType = Crop.GetCropItemType(), MaxResourceLevel = 3, MinResourceLevel = 0, ConsummationRate = 10}, 
                    new VillageResource { ItemType = Fishery.GetFishItemType(), MaxResourceLevel = 2, MinResourceLevel = 0, ConsummationRate = 10}
                }.ToList();

            totalResourceCapacity = 0;
            foreach (var res in neededResources)
            {
                totalResourceCapacity += res.MaxResourceLevel;
            }

            rnd = new Random();
        }

        public override void Tick(IVoxelHandle handle)
        {
            //handle.EachRandomInterval(1, () => doWork(handle));

            //todo: should not be done every tick??
            handle.Data.Inventory.ChangeCapacity(totalResourceCapacity); //should be done at start
            checkResourceLevels(handle); //should be done at start (and is also done after each consume)

            foreach (var resource in neededResources)
            {
                handle.EachRandomInterval(resource.ConsummationRate, () => consume(resource, handle));
            }

        }

        private void checkResourceLevels(IVoxelHandle handle)
        {
            foreach (var resource in neededResources)
            {
                if (!hasEnough(resource, handle))
                {
                    handle.Data.DataValue = 1; //not supplied
                    return;
                }
            }
            handle.Data.DataValue = 0; //all supplied
        }

        private bool hasEnough(VillageResource resource, IVoxelHandle handle)
        {
            return handle.Data.Inventory.GetAmountOfType(resource.ItemType) >= resource.MinResourceLevel;
        }

        private void consume(VillageResource resource, IVoxelHandle handle)
        {
            if (handle.Data.Inventory.GetAmountOfType(resource.ItemType) == 0) return;

            //var itemToConsume = handle.Data.Inventory.Items.ToArray()[rnd.Next(handle.Data.Inventory.ItemCount)];
            handle.Data.Inventory.DestroyItems(resource.ItemType, 1);
            checkResourceLevels(handle);
        }

        public override bool CanAcceptItemType(IVoxelHandle handle, IVoxelHandle deliveryHandle, ItemType type)
        {
            if (!(deliveryHandle.Type is MarketType)) return false;
            if (!neededResources.Any(e => e.ItemType == type)) return false;

            return getNbItemsOfTypeOrKanban(handle.Data.Inventory, type) < neededResources.First(e => e.ItemType == type).MaxResourceLevel;
        }

        private int getNbItemsOfTypeOrKanban(Inventory inventory, ItemType type)
        {
            return inventory.Items.Sum(item => Road.IsItemOrKanbanOfType(type, item) ? 1 : 0);
        }

        /// <summary>
        /// Old village code
        /// </summary>
        /// <param name="handle"></param>
        private void doWork(IVoxelHandle handle)
        {
            var warehouse =
                handle.GetRange(5).FirstOrDefault(v => v.Type == Warehouse && v.Data.DataValue < 20);
            if (warehouse == null) return;

            var forest = handle.GetRange(5).FirstOrDefault(v => v.Type == Forest && v.Data.DataValue > 0);
            if (forest == null) return;

            forest.Data.DataValue--;
            warehouse.Data.DataValue++;
        }
    }
}