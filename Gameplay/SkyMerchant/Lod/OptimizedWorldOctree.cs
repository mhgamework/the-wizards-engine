using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.Worlding;
using SlimDX;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;
using System.Linq;
using Castle.Core.Internal;

namespace MHGameWork.TheWizards.SkyMerchant.Lod
{
    /// <summary>
    /// Responsible for providing access to the physicals in the world, using an octree. 
    /// Physicals should be accessible in O(1).
    /// Note that each physical is stored multiple times, and at least once at each depth.
    /// Note: this class is probably pretty slow for dynamic objects
    /// 
    /// 
    /// 
    /// TODO: use IWorldObject instead of Physical
    /// TODO: think about using a leaf cell size as to define the tree, instead of a size and a depth
    /// TODO: try a version where there each physical is in a single node.
    /// </summary>
    public class OptimizedWorldOctree : IWorldOctree
    {
        private readonly Vector3 size;
        private readonly int maxDepth;

        /// <summary>
        /// Unrolled tree data
        /// </summary>
        private List<Physical>[] data;

        private Dictionary<Physical, List<int>> getPhysicalChunks = new Dictionary<Physical, List<int>>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="maxDepth">Root has 0 depth</param>
        public OptimizedWorldOctree(Vector3 size, int maxDepth)
        {
            this.size = size;
            this.maxDepth = maxDepth;
            data = new List<Physical>[ChunkCoordinate.GetCumulativeNbChunks(maxDepth + 1)];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new List<Physical>();
            }
        }

        public IEnumerable<Physical> GetWorldObjects(ChunkCoordinate coord)
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

        public void AddWorldObject(Physical p)
        {
            var physBB = p.GetBoundingBox().xna();

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
        public void UpdateWorldObject(Physical p)
        {
            RemoveWorldObject(p);
            AddWorldObject(p);
        }
        public void RemoveWorldObject(Physical p)
        {
            var chunks = getPhysicalChunks[p];
            foreach (var c in chunks)
            {
                data[c].Remove(p);
            }
        }
    }
}