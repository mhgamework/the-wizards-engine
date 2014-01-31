﻿using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace MHGameWork.TheWizards.Scattered.Model
{
    /// <summary>
    /// Responsible for managing an item container, and item container interactions
    /// </summary>
    public class Inventory
    {
        private Dictionary<ItemType, int> itemAmounts = new Dictionary<ItemType, int>();

         public string CreateItemsString()
         {
             var ret = "";
             foreach ( var p in itemAmounts)
             {
                 if (ret != "") ret += "\n";
                 ret += p.Key.Name + ": " + p.Value;
             }
             if (ret == "") return "No items";

             return ret;
         }

        public void AddNewItems(ItemType type, int amount)
        {
            changeAmountOfType(type, amount);
            
        }


        public void TransferItemsTo(Inventory targetInventory, ItemType type, int amount)
        {
            if (this.GetAmountOfType(type) < amount ) throw new InvalidOperationException("The source inventory does not contain enough items");
            this.changeAmountOfType(type,-amount);
            targetInventory.changeAmountOfType(type,amount);
        }

        public int GetAmountOfType(ItemType type)
        {
            return itemAmounts.GetOrCreate(type, () => 0);
        }
        private void changeAmountOfType(ItemType type, int relativeChange)
        {
            var curr = GetAmountOfType(type);
            curr += relativeChange;
            if (curr < 0) throw new InvalidOperationException();
            if (curr == 0) itemAmounts.Remove(type);
            itemAmounts[type] = curr;
        }
    }
}