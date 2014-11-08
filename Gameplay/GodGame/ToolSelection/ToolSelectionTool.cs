using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.GodGame.ToolSelection
{
    public class ToolSelectionTool : IToolSelectionItem
    {
        public string DisplayName;
        public IPlayerTool PlayerTool;

        public string GetDisplayName()
        {
            return DisplayName;
        }

        public void Select(ToolSelectionMenu menu)
        {
            menu.ActivateTool(PlayerTool);
        }
    }
}
