﻿using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.SkyMerchant._Engine.Spatial;
using SlimDX;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;

namespace MHGameWork.TheWizards.Simulation.Spatial
{
    /// <summary>
    /// Responsible for providing access to the physicals in the world, using an octree. 
    /// Physicals should be accessible in O(1).
    /// Note that each physical is stored multiple times, and at least once at each depth.
    /// Note: this class is probably pretty slow for dynamic objects
    /// For tests see the LinebasedLodRenderer test.
    /// 
    /// TODO: using IWorldObject, which is fishy
    /// TODO: think about using a leaf cell size as to define the tree, instead of a size and a depth
    /// TODO: try a version where there each IWorldObject is in a single node.
    /// </summary>
    public class OptimizedWorldOctree<T> : IWorldOctree<T> where T : IWorldObject
    {
        private readonly Vector3 size;
        private readonly int maxDepth;

        /// <summary>
        /// Unrolled tree data
        /// </summary>
        private List<T>[] data;

        private Dictionary<T, List<int>> getPhysicalChunks = new Dictionary<T, List<int>>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="maxDepth">Root has 0 depth</param>
        public OptimizedWorldOctree(Vector3 size, int maxDepth)
        {
            this.size = size;
            this.maxDepth = maxDepth;
            data = new List<T>[ChunkCoordinate.GetCumulativeNbChunks(maxDepth + 1)];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new List<T>();
            }
        }

        public IEnumerable<T> GetWorldObjects(ChunkCoordinate coord)
        {
            var index = coord.GetUnrolledIndex();
            var ret = data[index];
            return ret;
        }

        public Vector3 GetChunkRadius(ChunkCoordinate chunk)
        {
            return chunk.GetChunkSize(size) * 0.5f;
        }

        public Vector3 GetChunkCenter(ChunkCoordinate chunk)
        {
            var bb = chunk.GetBoundingBox(new Vector3(), size);
            return (bb.Minimum + bb.Maximum) * 0.5f;
        }

        public BoundingBox GetChunkBoundingBox(ChunkCoordinate chunk)
        {
            return chunk.GetBoundingBox(new Vector3(), size);
        }

        public bool IsLeaf(ChunkCoordinate chunk)
        {
            return (chunk.Depth == maxDepth);
        }

        public void AddWorldObject(T p)
        {
            var physBB = p.BoundingBox.xna();

            var chunks = new List<int>();
            getPhysicalChunks[p] = chunks;

            this.FindChunksDown(maxDepth, delegate(ChunkCoordinate coordinate)
                {
                    var bb = GetChunkBoundingBox(coordinate).xna();
                    var containment = physBB.Contains(bb);
                    switch (containment)
                    {
                        //TODO: can add speedup when contains, no childrens check is needed.
                        case ContainmentType.Contains:
                        case ContainmentType.Intersects:
                            data[coordinate.GetUnrolledIndex()].Add(p);
                            chunks.Add(coordinate.GetUnrolledIndex());
                            return true;
                        case ContainmentType.Disjoint:
                            return false;
                    }

                    throw new InvalidOperationException();

                }).ForEach(f => f.Depth = f.Depth); // Cheat to make this execute!
        }
        public void UpdateWorldObject(T p)
        {
            RemoveWorldObject(p);
            AddWorldObject(p);
        }
        public void RemoveWorldObject(T p)
        {
            var chunks = getPhysicalChunks[p];
            foreach (var c in chunks)
            {
                data[c].Remove(p);
            }
        }
    }
}