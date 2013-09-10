using System;
using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Lod
{
    public static class WorldOctreeExtensions
    {
        /// <summary>
        /// Returns all chunks with depth maxdepth for which condition is true.
        /// The condition has as constraint:
        ///     condition(chunk) => condition(parent(chunk))
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ChunkCoordinate> FindChunks(this IWorldOctree tree, int maxDepth,
                                                              Func<ChunkCoordinate, bool> condition)
        {
            return FindChunks(tree, ChunkCoordinate.Root, maxDepth, condition);
        }
        private static IEnumerable<ChunkCoordinate> FindChunks(IWorldOctree tree, ChunkCoordinate parent, int maxDepth, Func<ChunkCoordinate, bool> condition)
        {
            if (!condition(parent)) yield break;

            if (tree.IsLeaf(parent) || parent.Depth == maxDepth)
            {
                yield return parent;
                yield break;
            }

            foreach (var child in parent.GetChildren())
            {
                foreach (var res in FindChunks(tree, child, maxDepth, condition))
                    yield return res;
            }

        }

        public static IEnumerable<ChunkCoordinate> GetChunksInRange(this IWorldOctree worldOctree, Vector3 position, float minRange, float maxRange, int depth)
        {
            return worldOctree.FindChunks(depth, delegate(ChunkCoordinate c)
            {
                // Check if chunk is in range, assume spherical chunks
                var size = worldOctree.GetChunkRadius(c).Length();
                var center = worldOctree.GetChunkCenter(c);

                var dist = Vector3.Distance(position, center);
                var minDist = dist - size;
                var maxDist = dist + size;

                if (maxDist < minRange || minDist > maxRange) return false; // not in range
                return true;
            });
        }
    }
}