using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Common.GeoMipMap
{
    public interface ITerrainBlock : IDisposable
    {
        void AssignToQuadtreeNode( Wereld.IQuadtreeNode node );

        Wereld.IQuadtreeNode GetIQuadtreeNode();

        ITerrainBlock GetNeighbour( TerrainBlockEdge edge );

        void SetNeighbour( TerrainBlockEdge edge, ITerrainBlock neighbour );

    }
}
