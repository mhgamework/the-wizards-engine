using System;
using System.Collections.Generic;
using SlimDX.DirectInput;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor
{
    /// <summary>
    /// Responsible for converting user input into hotbar commands.
    /// Also manages the view
    /// </summary>
    public class HotbarController
    {
        private readonly Hotbar bar;
        private readonly HotbarTextView view;

        private Key[] slotKeys = new Key[] { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9 };

        private IHotbarItem nullItem = new NullHotbarItem();

        public HotbarController(Hotbar bar, HotbarTextView view)
        {
            this.bar = bar;
            this.view = view;

            lastSelectedItem = nullItem;
        }


        public void Update()
        {
            trySelect();
            trySwap();

            updateSelectedItem();

            view.Update();
        }

        private IHotbarItem lastSelectedItem;
        private void updateSelectedItem()
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
            var down = GetDownKeys();
            var pressed = GetPressedKeys();
            if (down.Count() != 2 || pressed.Count() != 1) return;

            // swap
            var slot1 = Array.IndexOf(slotKeys, down.First());
            var slot2 = Array.IndexOf(slotKeys, pressed.First());

            var item1 = bar.GetHotbarItem(slot1);
            var item2 = bar.GetHotbarItem(slot2);

            bar.SetHotbarItem(slot1, item2);
            bar.SetHotbarItem(slot2, item1);
        }

        private void trySelect()
        {
            var down = GetDownKeys();
            var pressed = GetPressedKeys();
            if (down.Count() != 1 || pressed.Count() != 1) return;

            bar.SelectedSlot = Array.IndexOf(slotKeys, pressed.First());
        }

        private IEnumerable<Key> GetDownKeys()
        {
            foreach (var key in slotKeys)
                if (TW.Graphics.Keyboard.IsKeyDown(key)) yield return key;
        }

        private IEnumerable<Key> GetPressedKeys()
        {
            foreach (var key in slotKeys)
                if (TW.Graphics.Keyboard.IsKeyPressed(key)) yield return key;
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