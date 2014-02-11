using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX.DirectInput;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.HotbarCore
{
    /// <summary>
    /// Responsible for converting user input into hotbar commands.
    /// Also manages the view
    /// 
    /// Responsible for translating user keys into hotbar slots!
    /// 
    /// NOT responsible for simulating the selected item's behaviour!!
    /// </summary>
    public class HotbarController
    {
        private readonly Hotbar bar;

        public Hotbar Bar { get { return bar; } }

        private readonly HotbarTextView view;

        private Key[] slotKeys = new Key[] { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9 };

        private IHotbarItem nullItem = new NullHotbarItem();

        public HotbarController(Hotbar bar, HotbarTextView view)
        {
            this.bar = bar;
            this.view = view;

            lastSelectedItem = nullItem;
        }


        /// <summary>
        /// WARNING: you have to manually call the update method on the selected item!!!!
        /// </summary>
        public void Update()
        {
            trySelect();
            trySwap();

            view.Update();
        }

        private IHotbarItem lastSelectedItem;
        /// <summary>
        /// Why is this seperate? should it be in update?
        /// </summary>
        public void UpdateSelectedItem()
        {
            var selected = GetHotbarItem(bar.SelectedSlot);
            if (lastSelectedItem != selected)
            {
                lastSelectedItem.OnDeselected();
                selected.OnSelected();
                lastSelectedItem = selected;
            }
            selected.Update();
        }

        private void trySwap()
        {
            var down = GetDownSlots();
            var pressed = GetPressedSlots();
            if (down.Count() != 2 || pressed.Count() != 1) return;

            // swap
            var slot1 = down.First();
            var slot2 = pressed.First();

            var item1 = bar.GetHotbarItem(slot1);
            var item2 = bar.GetHotbarItem(slot2);

            bar.SetHotbarItem(slot1, item2);
            bar.SetHotbarItem(slot2, item1);
        }

        private void trySelect()
        {
            var down = GetDownSlots();
            var pressed = GetPressedSlots();
            if (down.Count() != 1 || pressed.Count() != 1) return;

            bar.SelectedSlot = pressed.First();
        }

        /// <summary>
        /// Returns a list of hotkey bar slots for which the corresponding key is currently pressed down on the keyboard 
        /// </summary>
        public IEnumerable<int> GetDownSlots()
        {
            foreach (var key in slotKeys)
                if (TW.Graphics.Keyboard.IsKeyDown(key)) yield return Array.IndexOf(slotKeys, key);
        }

        /// <summary>
        /// Returns a list of hotkey bar slots for which the corresponding key was pressed down this frame (and not previous frame)
        /// </summary>
        public IEnumerable<int> GetPressedSlots()
        {
            foreach (var key in slotKeys)
                if (TW.Graphics.Keyboard.IsKeyPressed(key)) yield return Array.IndexOf(slotKeys, key);
        }

        private IHotbarItem GetHotbarItem(int slot)
        {
            return bar.GetHotbarItem(slot) ?? nullItem;
        }

        private class NullHotbarItem : IHotbarItem
        {
            public string Name { get { return "---"; } }
            public void OnSelected()
            {
            }

            public void OnDeselected()
            {
            }

            public void Update()
            {
            }
        }
    }


}