using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Gui
{
    public class MouseEventArgs : EventArgs, IHandelable
    {
        private Microsoft.Xna.Framework.Point cursorPosition;

        public Microsoft.Xna.Framework.Point CursorPosition
        {
            get { return cursorPosition; }
            set { cursorPosition = value; }
        }

        private bool handled;

        public bool Handled
        {
            get { return handled; }
            set { handled = value; }
        }

    }
}
