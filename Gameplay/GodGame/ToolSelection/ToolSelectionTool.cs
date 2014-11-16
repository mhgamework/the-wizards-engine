using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.GodGame.ToolSelection
{
    /// <summary>
    /// Toolselection menu item that actives an IPlayerTool when selected
    /// </summary>
    public class ToolSelectionTool : IToolSelectionItem
    {
        public string DisplayName;
        private IPlayerTool playerTool;
        private readonly ActiveToolInputHandler handler;

        /// <summary>
        /// For autofac
        /// </summary>
        public delegate ToolSelectionTool Factory(IPlayerTool playerTool, string displayName);

        public ToolSelectionTool(IPlayerTool playerTool, string displayName, ActiveToolInputHandler handler)
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
