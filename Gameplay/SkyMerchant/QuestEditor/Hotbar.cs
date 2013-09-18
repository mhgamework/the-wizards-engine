using System;
using MHGameWork.TheWizards.SkyMerchant._Tests.Development;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor
{
    /// <summary>
    /// Responsible for holding the hotbar state
    /// </summary>
    public class Hotbar
    {
        private IHotbarItem[] items;
        private int selectedSlot;

        public Hotbar()
        {
            items = new IHotbarItem[NumSlots];
        }

        public void SetHotbarItem(int slot, IHotbarItem item)
        {
            if (!IsValidSlot(slot)) throw new InvalidOperationException();

            items[slot] = item;
        }

      public IHotbarItem GetHotbarItem(int slot)
        {
            return items[slot];
        }

        private bool IsValidSlot(int slot)
        {
            return slot >= 0 && slot < NumSlots;
        }
        public int NumSlots { get { return 9; } }

        public int SelectedSlot
        {
            get { return selectedSlot; }
            set
            {
                if (!IsValidSlot(value)) throw new InvalidOperationException();
                selectedSlot = value;
            }
        }
    }
}