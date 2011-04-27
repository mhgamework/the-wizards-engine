using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.TileEngine
{
    public class SimpleSnappableWorldTarget : ISnappableWorldTarget
    {
        public SnapInformation SnapInformation { get; set; }
        public Transformation Transformation { get; set; }
        public TileData TileData { get; set; }
        

    }
}
