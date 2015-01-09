using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Scattered.Model;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class ConstantFactory
    {
        public Inventory Inventory { get; private set; }

        public float Rate;
        public ItemSet CreatedItemsSet;

    }

    /// <summary>
    /// Immutable set of items
    /// </summary>
    public struct ItemSet
    {
        private ItemType[] items;

        public ItemSet(IEnumerable<ItemType> items)
        {
            this.items = items.ToArray();
        }

        public ItemSet(ItemType type, int amount)
        {
            items = Enumerable.Repeat(type, amount).ToArray();
        }

        public ItemSet Add(ItemType type, int amount)
        {
            return new ItemSet(items.Concat(Enumerable.Repeat(type, amount)));


        }
        public ItemSet Subtract(ItemType type, int amount)
        {
            if (items.Count(i => i == type) < amount) throw new InvalidOperationException();
            //TODO
        }

        public IEnumerable<ItemType> allItems()
        {
            //return entries.SelectMany(e => Enumerable.Repeat(e.Type, e.Amount));
        }

        public struct Entry
        {
            public ItemType Type { get; private set; }
            public int Amount { get; private set; }

            public Entry(ItemType type, int amount)
                : this()
            {
                Type = type;
                Amount = amount;
            }
        }
    }


}