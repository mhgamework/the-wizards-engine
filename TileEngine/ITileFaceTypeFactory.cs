using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.TileEngine
{
    public interface ITileFaceTypeFactory
    {
        TileFaceType GetTileFaceType(Guid guid);

    }
}
