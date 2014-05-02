using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.SkyMerchant._Engine.Spatial;
using SlimDX;

namespace MHGameWork.TheWizards.Simulation.Spatial
{
    public static class WorldOctreeExtensions
    {
        /// <summary>
        /// Returns all chunks with depth maxdepth for which condition is true.
        /// The condition has as constraint:
        ///     condition(chunk) => condition(parent(chunk))
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ChunkCoordinate> FindChunksDown<T>(this IWorldOctree<T> tree, int maxDepth,
                                                              Func<ChunkCoordinate, bool> condition) where T : IWorldObject
        {
            return FindChunksDown(tree, ChunkCoordinate.Root, maxDepth, condition);
        }
        public static IEnumerable<ChunkCoordinate> FindChunksDown<T>(this IWorldOctree<T> tree, ChunkCoordinate parent, int maxDepth, Func<ChunkCoordinate, bool> condition) where T : IWorldObject
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
        public static ChunkCoordinate FindChunkUp<T>(this IWorldOctree<T> tree, ChunkCoordinate start,
                                                              Func<ChunkCoordinate, bool> condition) where T : IWorldObject
        {
            if (start.IsEmtpy) return start;
            if (condition(start)) return start;

            return tree.FindChunkUp(start.GetParent(),condition);

        }

        public static IEnumerable<ChunkCoordinate> GetChunksInRange<T>(this IWorldOctree<T> worldOctree, Vector3 position, float minRange, float maxRange, int depth) where T : IWorldObject
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