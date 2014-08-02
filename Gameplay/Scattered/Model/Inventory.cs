using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Model
{
    /// <summary>
    /// Responsible for managing an item container, and item container interactions
    /// </summary>
    public class Inventory
    {
        public int Capacity { get; private set; }
        private Dictionary<ItemType, int> itemAmounts = new Dictionary<ItemType, int>();

        public Inventory()
        {
            Capacity = int.MaxValue;
        }

        public Inventory(int capacity)
        {
            Capacity = capacity;
        }

        public void ChangeCapacity(int newCapacity)
        {
            if (ItemCount > newCapacity) throw new InvalidOperationException();
            Capacity = newCapacity;
        }

        public int ItemCount
        {
            get { return itemAmounts.Values.Sum(); }
        }

        public string CreateItemsString()
        {
            var ret = "";
            foreach (var p in itemAmounts)
            {
                if (ret != "") ret += "\n";
                ret += p.Key.Name + ": " + p.Value;
            }
            if (ret == "") return "No items";

            return ret;
        }

        public int AddNewItems(ItemType type, int amount)
        {
            var effectiveChange = changeAmountOfType(type, amount);
            return effectiveChange;
        }


        public void TransferItemsTo(Inventory targetInventory, ItemType type, int amount)
        {
            if (this.GetAmountOfType(type) < amount) throw new InvalidOperationException("The source inventory does not contain enough items");
            this.changeAmountOfType(type, -amount);
            targetInventory.changeAmountOfType(type, amount);
        }

        public int GetAmountOfType(ItemType type)
        {
            return itemAmounts.GetOrCreate(type, () => 0);
        }
        private int changeAmountOfType(ItemType type, int relativeChange)
        {
            if (ItemCount + relativeChange > Capacity)
                relativeChange = Capacity - ItemCount;

            var curr = GetAmountOfType(type);
            curr += relativeChange;
            if (curr < 0) throw new InvalidOperationException();
            if (curr == 0) itemAmounts.Remove(type);
            itemAmounts[type] = curr;

            return relativeChange;
        }

        public void TakeAll(Inventory inventory)
        {
            foreach (var el in new Dictionary<ItemType,int>(inventory.itemAmounts))
                inventory.TransferItemsTo(this, el.Key, el.Value);
        }

        public int this[ItemType type]
        {
            get { return GetAmountOfType(type); }
        }

        public void DestroyItems(ItemType type, int amount)
        {
            changeAmountOfType(type,-amount);
        }

        public void Clear()
        {
            itemAmounts.Clear();
        }

        public IEnumerable<ItemType> Items{get
        {
            foreach (var p in itemAmounts)
            {
                for (int i = 0; i < p.Value; i++)
                {
                    yield return p.Key;
                }
            }
        }}
    }
}