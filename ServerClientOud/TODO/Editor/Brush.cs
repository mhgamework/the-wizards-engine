using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public struct Brush
    {
        public Microsoft.Xna.Framework.Vector3 Position;
        public float Range;


        public Brush(Microsoft.Xna.Framework.Vector3 nPos, float nRange)
        {
            Position = nPos;
            Range = nRange;
        }
    }
}
