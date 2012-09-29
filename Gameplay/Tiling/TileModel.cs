using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Building;
using MHGameWork.TheWizards.ModelContainer;

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
            foreach( var tile in TW.Model.Objects.Where(o => o is TiledEntity).Select(o=>(TiledEntity)o))
            {
                if (tile.Position == position)
                    return tile;
            }
            return null;
        }
    }
}
