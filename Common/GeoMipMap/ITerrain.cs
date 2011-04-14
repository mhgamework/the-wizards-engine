using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Common.GeoMipMap
{
    public interface ITerrain
    {
        ITerrainBlock CreateBlock ( int x, int z );

    }
}
