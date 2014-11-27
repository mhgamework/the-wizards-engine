using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.GodGame.ToolSelection
{
    /// <summary>
    /// Toolselection menu item that actives an PlayerTool when selected
    /// </summary>
    public class ToolSelectionTool : IToolSelectionItem
    {
        public string DisplayName;
        private PlayerTool playerTool;
        private readonly ActiveToolInputHandler handler;

        /// <summary>
        /// For autofac
        /// </summary>
        public delegate ToolSelectionTool Factory(PlayerTool playerTool, string displayName);

        public ToolSelectionTool(PlayerTool playerTool, string displayName, ActiveToolInputHandler handler)
        {

            DisplayName = displayName;
            this.playerTool = playerTool;
            this.handler = handler;
        }

        public string GetDisplayName()
        {
            return DisplayName;
        }

        public void Select(ToolSelectionMenu menu)
        {
            handler.ActivePlayerTool = playerTool;
            menu.DisplayRootItems();
        }
    }
}
