using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.TileEngine.SnapEngine
{
    public class WorldTileSnapper
    {
        private World world;

        public WorldTileSnapper(World world)
        {
            this.world = world;
        }

        public Transformation CalculateSnap(TileData data, Transformation currentTransformation)
        {
            return currentTransformation;
        }
    }
}
