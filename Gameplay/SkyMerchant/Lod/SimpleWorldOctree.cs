using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Engine.Worlding;
using SlimDX;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;

namespace MHGameWork.TheWizards.SkyMerchant.Lod
{
    /// <summary>
    /// Fake octree implementation (non-optimized)
    /// </summary>
    public class SimpleWorldOctree : IWorldOctree
    {
        private readonly Vector3 radius;


        public SimpleWorldOctree(Vector3 radius)
        {
            this.radius = radius;
        }

        /// <summary>
        /// Returns all physicals that are partially or completely inside this chunk
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public IEnumerable<Physical> GetWorldObjects(ChunkCoordinate coord)
        {
            var bb = coord.GetBoundingBox(new Vector3(), radius*2);
            return
                TW.Data.Objects.OfType<Physical>()
                  .Where(p => bb.xna().Contains(p.GetBoundingBox().xna()) != ContainmentType.Disjoint);
        }

        public Vector3 GetChunkRadius(ChunkCoordinate chunk)
        {
            return chunk.GetChunkSize(radius * 2) * 0.5f;
        }

        public Vector3 GetChunkCenter(ChunkCoordinate parent)
        {
            var bb = parent.GetBoundingBox(new Vector3(), radius * 2);
            return (bb.Maximum + bb.Minimum) * 0.5f;
        }

        public BoundingBox GetChunkBoundingBox(ChunkCoordinate chunk)
        {
            return chunk.GetBoundingBox(new Vector3(), radius*2);
        }

        public bool IsLeaf(ChunkCoordinate parent)
        {
            return false; // Infinite tree!
        }
    }
}