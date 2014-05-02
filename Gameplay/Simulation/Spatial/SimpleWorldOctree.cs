﻿using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant._Engine.Spatial;
using SlimDX;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;

namespace MHGameWork.TheWizards.Simulation.Spatial
{
    /// <summary>
    /// Fake octree implementation (non-optimized)
    /// For tests see the LinebasedLodRenderer test.
    /// </summary>
    public class SimpleWorldOctree<T> : IWorldOctree<T> where T : IWorldObject
    {
        private readonly Vector3 radius;
        private readonly IEnumerable<T> getObjects;


        public SimpleWorldOctree(Vector3 radius, IEnumerable<T> getObjects)
        {
            this.radius = radius;
            this.getObjects = getObjects;
        }

        /// <summary>
        /// Returns all physicals that are partially or completely inside this chunk
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public IEnumerable<T> GetWorldObjects(ChunkCoordinate coord)
        {
            var bb = coord.GetBoundingBox(new Vector3(), radius * 2);
            return getObjects.Where(p => bb.xna().Contains(p.BoundingBox.xna()) != ContainmentType.Disjoint);
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
            return chunk.GetBoundingBox(new Vector3(), radius * 2);
        }

        public bool IsLeaf(ChunkCoordinate parent)
        {
            return false; // Infinite tree!
        }
    }
}