using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.Worlding;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Lod
{
    /// <summary>
    /// Responsible for providing access to the physicals in the world, using an octree. 
    /// Physicals should be accessible in O(1)
    /// </summary>
    public class OptimizedWorldOctree : IWorldOctree
    {
        /// <summary>
        /// Unrolled tree data
        /// </summary>
        private List<Physical>[] data;

        public IEnumerable<Physical> GetPhysicals(ChunkCoordinate coord)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetChunkRadius(ChunkCoordinate chunk)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetChunkCenter(ChunkCoordinate chunk)
        {
            throw new System.NotImplementedException();
        }

        public BoundingBox GetChunkBoundingBox(ChunkCoordinate chunk)
        {
            throw new System.NotImplementedException();
        }

        public bool IsLeaf(ChunkCoordinate chunk)
        {
            throw new System.NotImplementedException();
        }
    }
}