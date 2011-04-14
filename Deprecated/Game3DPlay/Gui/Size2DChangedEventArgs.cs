using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.Game3DPlay.Gui
{
    public class Size2DChangedEventArgs : EventArgs
    {
        public Size2DChangedEventArgs(Vector2 nOldSize, Vector2 nNewSize)
            : base()
        {
            OldSize = nOldSize;
            NewSize = nNewSize;
        }

        private Vector2 _oldSize;

        public Vector2 OldSize
        {
            get { return _oldSize; }
            private set { _oldSize = value; }
        }

        private Vector2 _newSize;

        public Vector2 NewSize
        {
            get { return _newSize; }
            private set { _newSize = value; }
        }


    }
}
