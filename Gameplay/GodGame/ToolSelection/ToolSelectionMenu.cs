using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.ToolSelection
{
    public class ToolSelectionMenu
    {
        private Textarea[] textAreas;
        private List<IToolSelectionItem> currentDisplayedItems;

        private readonly int maxNbItems;

        private List<Key> keyList;

        public ToolSelectionMenu()
        {
            maxNbItems = 9;
            textAreas = new Textarea[maxNbItems];
            for (int i = 0; i < maxNbItems; i++)
            {
                var ta = new Textarea();
                ta.Position = new Vector2(i * 125, 25);
                textAreas[i] = ta;
            }

            currentDisplayedItems = new List<IToolSelectionItem>();

            keyList = new List<Key> {Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9};
            updateTextAreas();
        }

        public void ProcessUserInput()
        {
            var kb = TW.Graphics.Keyboard;
            for (int i = 0; i < keyList.Count; i++)
            {
                if (kb.IsKeyPressed(keyList[i]))
                {
                    OnSelectItem(i);
                    break;
                }
            }

            if(kb.PressedKeys.Any(e => !keyList.Contains(e)))
                OnCancel();
            
        }

        private void OnSelectItem(int index)
        {
            if (currentDisplayedItems.Count <= index)
                return;
            currentDisplayedItems[index].Select(this);
        }

        private void OnCancel()
        {
            //todo
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
    }
}
