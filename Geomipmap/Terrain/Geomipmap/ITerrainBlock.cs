using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Common.GeoMipMap;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Terrain.Geomipmap
{
    public interface ITerrainBlock
    {
        IndexBuffer IndexBuffer { get; set; }
        ITerrainBlock GetNeighbour(TerrainBlockEdge edge);
        int DetailLevel { get; set; }
        int TriangleCount { get; set; }
    }
}
