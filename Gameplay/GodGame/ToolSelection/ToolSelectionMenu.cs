using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.ToolSelection
{
    /// <summary>
    /// Responsible for display a text-based tool selection menu
    /// Responsible for processing info for the menu
    /// The menu consists of a list of actions for each D1 to d9 key.
    /// </summary>
    public class ToolSelectionMenu
    {
        private readonly Textarea[] textAreas;
        private readonly List<Key> keyList;
        private List<IToolSelectionItem> currentDisplayedItems;
        private List<IToolSelectionItem> rootItems;

        private readonly int maxNbItems;
        private bool initialized;


        public ToolSelectionMenu()
        {
            maxNbItems = 9;
            textAreas = new Textarea[maxNbItems];
            for (int i = 0; i < maxNbItems; i++)
            {
                var ta = new Textarea();
                ta.Position = new Vector2(10 + i * 125, 25);
                //ta.BackgroundColor = new Color4(0.2f, 0.5f, 0.5f, 1f);
                textAreas[i] = ta;
            }

            currentDisplayedItems = new List<IToolSelectionItem>();

            keyList = new List<Key> { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9 };
            updateTextAreas();
        }

        public void ProcessUserInput()
        {
            var kb = TW.Graphics.Keyboard;
            for (int i = 0; i < keyList.Count; i++)
            {
                if (kb.IsKeyPressed(keyList[i]))
                {
                    onSelectItem(i);
                    break;
                }
            }

            if (kb.PressedKeys.Any(e => !keyList.Contains(e)))
                DisplayRootItems();
        }

        private void onSelectItem(int index)
        {
            if (!initialized)
                throw new InvalidOperationException("ToolSelectionMenu not initialized!");

            if (currentDisplayedItems.Count <= index)
                return;
            currentDisplayedItems[index].Select(this);
        }

        private void updateTextAreas()
        {
            for (int i = 0; i < textAreas.Length; i++)
            {
                textAreas[i].Text = getItemText(i);
            }
        }

        private string getItemText(int itemIndex)
        {
            var indexstr = "" + (itemIndex + 1) + ": ";
            if (currentDisplayedItems.Count <= itemIndex)
                return indexstr;
            return indexstr + (currentDisplayedItems[itemIndex] == null ? "" : currentDisplayedItems[itemIndex].GetDisplayName());
        }

        public void SetToolSelectionItem(List<IToolSelectionItem> selectionItems)
        {
            currentDisplayedItems = selectionItems.ToList();
            updateTextAreas();
        }

        public void Initialize(List<IToolSelectionItem> rootToolSelectionItems)
        {
            rootItems = rootToolSelectionItems;
            SetToolSelectionItem(rootItems);
            initialized = true;
        }

        public void DisplayRootItems()
        {
            if (!initialized)
                throw new InvalidOperationException("ToolSelectionMenu not initialized!");

            SetToolSelectionItem(rootItems);
        }


    }
}
