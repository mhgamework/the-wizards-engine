using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Terrain.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Rendering
{
    public class TerrainQuadTreeNodeData
    {
        public TerrainBlock TerrainBlock;
        public BoundingBox TerrainBounding;

        public bool Visible;
    }
}