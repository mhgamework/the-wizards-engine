using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.TileEngine
{
    public interface ISnappableWorldTarget
    {
        SnapInformation SnapInformation { get; }
        Transformation Transformation { get; }
    }
}
