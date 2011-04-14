using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Gui
{
    /// <summary>
    /// This class is only used by the window internally. It is created when the borders of the window are dragged.
    /// Other controls can check in the OnDragDrop event for this object and can so dock windows etc.
    /// </summary>
    internal class GuiWindowBorder : GuiControl
    {
        GuiWindow parentWindow;
        private GuiWindow.Border border;

        public GuiWindow.Border Border
        {
            get { return border; }
            set { border = value; }
        }
	

        public GuiWindowBorder( GuiWindow nParentWindow, GuiWindow.Border nBorder )
        {
            parentWindow = nParentWindow;
            border = nBorder;
            Visible = false;
        }


    }
}
