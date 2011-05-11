using System;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.TileEngine.SnapEngine
{
    public class SnapPoint
    {
        //Position = relative to the center of gravity of its worldObject
        public Vector3 Position { get; set; }
        public SnapType SnapType { get; set; }

        public Vector3 Normal { get; set; }
        public Vector3 Up { get; set; }
        public Boolean ClockwiseWinding { get; set; }

        //some foefeling
        public TileFaceType TileFaceType { get; set;}

    }
}
