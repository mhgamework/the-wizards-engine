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
        public static IEnumerable<ChunkCoordinate> FindChunksDown(this IWorldOctree tree, int maxDepth,
                                                              Func<ChunkCoordinate, bool> condition)
        {
            return FindChunksDown(tree, ChunkCoordinate.Root, maxDepth, condition);
        }
        private static IEnumerable<ChunkCoordinate> FindChunksDown(IWorldOctree tree, ChunkCoordinate parent, int maxDepth, Func<ChunkCoordinate, bool> condition)
        {
            if (!condition(parent)) yield break;

            if (tree.IsLeaf(parent) || parent.Depth == maxDepth)
            {
                yield return parent;
                yield break;
            }

            foreach (var child in parent.GetChildren())
            {
                foreach (var res in FindChunksDown(tree, child, maxDepth, condition))
                    yield return res;
            }

        }

        /// <summary>
        /// Returns all chunks with depth maxdepth for which condition is true.
        /// The condition has as constraint:
        ///     condition(chunk) => condition(parent(chunk))
        /// </summary>
        /// <returns></returns>
        public static ChunkCoordinate FindChunkUp(this IWorldOctree tree, ChunkCoordinate start,
                                                              Func<ChunkCoordinate, bool> condition)
        {
            if (start.IsEmtpy) return start;
            if (condition(start)) return start;

            return tree.FindChunkUp(start.GetParent(),condition);

        }

        public static IEnumerable<ChunkCoordinate> GetChunksInRange(this IWorldOctree worldOctree, Vector3 position, float minRange, float maxRange, int depth)
        {
            return worldOctree.FindChunksDown(depth, delegate(ChunkCoordinate c)
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