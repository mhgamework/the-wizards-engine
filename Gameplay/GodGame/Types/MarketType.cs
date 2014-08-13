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
    /// <summary>
    /// The market collects items that need to be supplied to villages.
    /// After collection, the items are transformed into 'processed' variants, which are suited to distribute to houses.
    /// For each item type to distribute, the market has a number of inventory spaces reserved.
    /// When there is free space, the market tries to get the needed items from a nearby warehouse (must be within range).
    /// The market regularly checks houses in range if they need processed resources.
    /// </summary>
    public class MarketType : GameVoxelType
    {
        private int totalCapacity;
        private const int capacityPerType = 5;
        private const int distributionRange = 3;
        private const int collectRange = 10;

        private List<MarketResourceType> marketInventory;

        private struct MarketResourceType
        {
            public ItemType ItemType;
            public ItemType ProcessedItemType;
            public int MaxResourceLevel; //max nb of this resourcetype to store in market
        }

        private Random rnd;

        public ItemType ProcessedCropType { get; private set; }
        public ItemType ProcessedFishType { get; private set; }

        public MarketType()
            : base("Market")
        {
            Color = Color.Orange;

            ProcessedFishType = new ItemType { Name = "ProcessedFish", Mesh = UtilityMeshes.CreateBoxColored(Color.DeepSkyBlue, new Vector3(1)) };
            ProcessedCropType = new ItemType { Name = "ProcessedCrop", Mesh = UtilityMeshes.CreateBoxColored(Color.Orange, new Vector3(1)) };

            marketInventory = new List<MarketResourceType>
                {
                    new MarketResourceType
                        {
                            ItemType = Crop.GetCropItemType(),
                            ProcessedItemType = ProcessedCropType,
                            MaxResourceLevel = 5
                        },
                    new MarketResourceType
                        {
                            ItemType = Fishery.GetFishItemType(),
                            ProcessedItemType = ProcessedFishType,
                            MaxResourceLevel = 8
                        }
                };

            totalCapacity = 0;
            foreach (var resourceType in marketInventory)
            {
                totalCapacity += resourceType.MaxResourceLevel;
            }

            rnd = new Random();
        }

        public override void Tick(IVoxelHandle handle)
        {
            //todo: should not be done every tick??
            handle.Data.Inventory.ChangeCapacity(totalCapacity);

            handle.EachRandomInterval(0.1f, () => tryCollect(handle));
            handle.EachRandomInterval(0.1f, () => tryDistribute(handle));
        }

        public override bool CanAcceptItemType(IVoxelHandle handle, ItemType type)
        {
            if (!marketInventory.Any(e => e.ItemType == type)) return false;

            return handle.Data.Inventory.AvailableSlots > 0 && handle.Data.Inventory.GetAmountOfType(type) < capacityPerType;
        }

        private void tryCollect(IVoxelHandle handle)
        {
            if (handle.Data.Inventory.ItemCount == totalCapacity) return;
            var warehousesInRange = getWareHousesInRange(handle);
            foreach (var marketResource in marketInventory.Where(e => handle.Data.Inventory.GetAmountOfType(e.ItemType) < e.MaxResourceLevel))
            {
                foreach (var warehouse in warehousesInRange.Where(warehouse => warehouse.Data.Inventory.GetAmountOfType(marketResource.ItemType) > 0))
                {
                    warehouse.Data.Inventory.TransferItemsTo(handle.Data.Inventory, marketResource.ItemType, 1);
                    break;
                }
            }
        }

        /// <summary>
        /// Checks if there is a road connected.
        /// Checks if there are houses reachable via that road that need processedItem and that are in distributionRange.
        /// If so, outputs processedItem to the road.
        /// </summary>
        /// <param name="handle"></param>
        private void tryDistribute(IVoxelHandle handle)
        {
            if (handle.Data.Inventory.ItemCount == 0) return;

            var itemToTransport = handle.Data.Inventory.Items.ToArray()[rnd.Next(handle.Data.Inventory.ItemCount)];
            var marketResource = marketInventory.Where(e => e.ItemType == itemToTransport);
            if (!marketResource.Any()) return;
            var itemToTransportProcessed = marketResource.First().ProcessedItemType;

            var road = handle.Get4Connected().FirstOrDefault(v => v.CanAcceptItemType(itemToTransportProcessed) && v.Type is RoadType);
            if (road == null) return;

            var housesInRange = handle.GetRange(distributionRange).Where(e => e.Type is VillageType);
            var houses = Road.FindConnectedInventories(road, itemToTransportProcessed).Select(e => e.Item1).Where(housesInRange.Contains);
            if (!houses.Any()) return;

            handle.Data.Inventory.DestroyItems(itemToTransport, 1);
            road.Data.Inventory.AddNewItems(itemToTransportProcessed, 1);

        }

        public override IEnumerable<IVoxelInfoVisualizer> GetInfoVisualizers(IVoxelHandle handle)
        {
            foreach (var e in base.GetInfoVisualizers(handle))
                yield return e;

            yield return new RangeVisualizer(handle, distributionRange);
            yield return new HighlightVoxelsVisualizer(handle, getWareHousesInRange);
        }


        private IEnumerable<IVoxelHandle> getWareHousesInRange(IVoxelHandle handle)
        {
            return handle.GetRange(collectRange).Where(v => v.Type is WarehouseType);
        }
    }
}
