using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.Tiling
{
    /// <summary>
    /// Provides access to all tile data in the model
    /// </summary>
    public class TileModel : EngineModelObject
    {
        public TileModel()
        {

        }

        public TiledEntity GetTileAt(Point3 position)
        {
            foreach( var tile in TW.Data.Objects.Where(o => o is TiledEntity).Select(o=>(TiledEntity)o))
            {
                if (tile.Position == position)
                    return tile;
            }
            return null;
        }
    }
}
