using System;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.Items
{
    [ModelObjectChanged]
    public class ItemPart : EngineModelObject, IObjectPart
    {
        public IItem Parent { get; set; }

        public ItemPart()
        {
            Free = true;
        }
        /// <summary>
        /// This is true when noone is holding this object.
        /// </summary>
        public bool Free { get; set; }


        public void PutInStorage(IItemStorage storage)
        {
            if (storage.ItemStorage.IsFull) throw new InvalidOperationException("Storage is full!");

            if (!Free) Drop();

            storage.ItemStorage.Items.Add(Parent);
            Free = false;
          }

        public void Drop()
        {
            var c = GetCurrentInventory();
            if (c != null)
                c.ItemStorage.Items.Remove(Parent);

            Free = true;
        }

        public IItemStorage GetCurrentInventory()
        {
            var arr = TW.Data.Objects.ToArray();
            var arr2 = TW.Data.Objects.OfType<IItemStorage>().ToArray();

            return TW.Data.Objects.OfType<IItemStorage>().FirstOrDefault(i => i.ItemStorage.Items.Contains(Parent));
        }
    }
}