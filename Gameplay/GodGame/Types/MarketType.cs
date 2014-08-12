using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class MarketType : GameVoxelType
    {
        private int totalCapacity;
        private int nbItemTypes = 1;
        private int capacityPerType = 5;
        private int distributionRange = 3;
        private int collectRange = 10;

        private Dictionary<ItemType, ItemType> itemProcessingDictionary;

        private Random rnd;

        public ItemType ProcessedCropType { get; private set; }

        public MarketType()
            : base("Market")
        {
            Color = Color.Orange;
            totalCapacity = nbItemTypes * capacityPerType;

            itemProcessingDictionary = new Dictionary<ItemType, ItemType>();
            ProcessedCropType = new ItemType();
            itemProcessingDictionary.Add(GameVoxelType.Crop.GetCropItemType(), ProcessedCropType);

            rnd = new Random();
        }

        public override void Tick(Internal.IVoxelHandle handle)
        {
            //todo: should not be done every tick??
            handle.Data.Inventory.ChangeCapacity(totalCapacity);

            handle.EachRandomInterval(0.1f, () => tryCollect(handle));
            handle.EachRandomInterval(0.1f, () => tryDistribute(handle));
        }

        public override bool CanAcceptItemType(IVoxelHandle handle, ItemType type)
        {
            if (type != GameVoxelType.Crop.GetCropItemType()) return false;

            return handle.Data.Inventory.AvailableSlots > 0 && handle.Data.Inventory.GetAmountOfType(type) < capacityPerType;
        }
        
        private void tryCollect(IVoxelHandle handle)
        {
            if (handle.Data.Inventory.ItemCount == totalCapacity) return;
            var warehousesInRange = handle.GetRange(collectRange).Where(e => e.Type == Warehouse);
            foreach (var itemType in itemProcessingDictionary.Keys.Where(itemType => handle.Data.Inventory.GetAmountOfType(itemType) <= capacityPerType))
            {
                foreach (var warehouse in warehousesInRange.Where(warehouse => warehouse.Data.Inventory.GetAmountOfType(itemType) > 0))
                {
                    warehouse.Data.Inventory.TransferItemsTo(handle.Data.Inventory, itemType, 1);
                    break;
                }
            }
        }

        private void tryDistribute(IVoxelHandle handle)
        {
            //todo: check road connection

            if (handle.Data.Inventory.ItemCount == 0) return;

            var itemToTransport = handle.Data.Inventory.Items.ToArray()[rnd.Next(handle.Data.Inventory.ItemCount)];
            ItemType itemToTransportProcessed;
            itemProcessingDictionary.TryGetValue(itemToTransport, out itemToTransportProcessed);
            if (itemToTransportProcessed == null) return;

            var houses = handle.GetRange(distributionRange).Where(e => e.Type is VillageType);
            foreach (var house in houses.Where(house => house.CanAcceptItemType(itemToTransportProcessed)))
            {
                handle.Data.Inventory.DestroyItems(itemToTransport, 1);
                house.Data.Inventory.AddNewItems(itemToTransportProcessed, 1);
                return;
            }
        }
    }
}
