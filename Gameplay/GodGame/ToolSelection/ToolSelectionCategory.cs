using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.GodGame.ToolSelection
{
    public class ToolSelectionCategory : IToolSelectionItem
    {
        public string DisplayName;
        public List<IToolSelectionItem> SelectionItems = new List<IToolSelectionItem>(); 

        public string GetDisplayName()
        {
            return DisplayName;
        }

        public void Select(ToolSelectionMenu menu)
        {
            menu.SetToolSelectionItem(SelectionItems);
        }
    }
}
