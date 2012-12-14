using MHGameWork.TheWizards.Common.GeoMipMap;
using Microsoft.Xna.Framework.Graphics;
using ITerrainBlock = MHGameWork.TheWizards.Terrain.Geomipmap.ITerrainBlock;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Terrain
{
    public class SimpleTerrainBlock : ITerrainBlock
    {
        public IndexBuffer IndexBuffer { get; set; }
        public int DetailLevel { get; set; }

        public int TriangleCount { get; set; }

        public float[] MinDistancesSquared { get; set; }

        private ITerrainBlock[] neighbours = new ITerrainBlock[4];

        public ITerrainBlock GetNeighbour(TerrainBlockEdge edge)
        {
            return neighbours[(int)edge];
        }
        public void SetNeightbour(TerrainBlockEdge edge, ITerrainBlock value)
        {
            neighbours[(int)edge] = value;
        }

        public SimpleTerrainBlock()
        {
            DetailLevel = -1;
        }

    }
}
