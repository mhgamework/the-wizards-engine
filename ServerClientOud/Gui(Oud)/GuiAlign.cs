using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Gui
{
    [Flags]
    public enum GuiAlign
    {
        None = 0,
        Left = 1,
        Center = 2,
        Right = 4,
        Top = 8,
        Middle = 16,
        Bottom = 32,

        TopLeft = Top | Left,
        TopCenter = Top | Center,
        TopBottom = Top | Right,

        MiddleLeft = Middle | Left,
        MiddleCenter = Middle | Center,
        MiddleRight = Middle | Right,

        BottomLeft = Bottom | Left,
        BottomCenter = Bottom | Center,
        BottomRight = Bottom | Right
    }
}
