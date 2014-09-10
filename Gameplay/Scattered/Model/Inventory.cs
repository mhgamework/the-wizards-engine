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

        public void AddNewItems(ItemType type, int amount)
        {
            if (!CanAdd(type,amount)) throw new InvalidOperationException("Cannot add new items!");
            changeAmountOfType(type, amount);
            //var effectiveChange = changeAmountOfType(type, amount);
            //return effectiveChange;
        }


        /// <summary>
        /// Returns the number of items transferred, untested
        /// </summary>
        /// <param name="targetInventory"></param>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public void TransferItemsTo(Inventory targetInventory, ItemType type, int amount)
        {
            if (!targetInventory.CanAdd(type, amount)) throw new InvalidOperationException("Cannot add new items!");
            if (!CanRemove(type,amount)) throw new InvalidOperationException("Cannot remove items!");
            
            targetInventory.changeAmountOfType(type, amount);//amount = targetInventory.changeAmountOfType(type, amount);
            this.changeAmountOfType(type, -amount);
            //return amount;
        }

        public int GetAmountOfType(ItemType type)
        {
            return itemAmounts.GetOrCreate(type, () => 0);
        }
        private void changeAmountOfType(ItemType type, int relativeChange)
        {
            if (ItemCount + relativeChange > Capacity)
                relativeChange = Capacity - ItemCount;

            var curr = GetAmountOfType(type);
            curr += relativeChange;
            if (curr < 0) throw new InvalidOperationException();
            if (curr == 0) itemAmounts.Remove(type);
            itemAmounts[type] = curr;

            //return relativeChange;
        }

        /// <summary>
        /// TODO: probably doesn't work properly
        /// </summary>
        /// <param name="inventory"></param>
        public void TakeAll(Inventory inventory)
        {
            foreach (var el in new Dictionary<ItemType, int>(inventory.itemAmounts))
                inventory.TransferItemsTo(this, el.Key, el.Value);
        }

        public int this[ItemType type]
        {
            get { return GetAmountOfType(type); }
        }

        public void DestroyItems(ItemType type, int amount)
        {
            if (!CanRemove(type,amount)) throw new InvalidOperationException("Cannot remove given amount of items");
            changeAmountOfType(type, -amount);
        }

        public void Clear()
        {
            itemAmounts.Clear();
        }

        public IEnumerable<ItemType> Items
        {
            get
            {
                foreach (var p in itemAmounts)
                {
                    for (int i = 0; i < p.Value; i++)
                    {
                        yield return p.Key;
                    }
                }
            }
        }

        public int AvailableSlots
        {
            get { return Capacity - ItemCount; }
        }

        public bool CanAdd(ItemType type, int i)
        {
            return AvailableSlots >= i;
        }
        public bool CanRemove(ItemType type, int i)
        {
            return this[type] >= i;
        }

    }
}