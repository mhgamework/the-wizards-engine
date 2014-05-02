using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant.Lod;
using MHGameWork.TheWizards.SkyMerchant._Engine.Spatial;
using SlimDX;

namespace MHGameWork.TheWizards.Simulation.Spatial
{
    /// <summary>
    /// Responsible for providing hierarchial chunk based access to the world.
    /// </summary>
    public interface IWorldOctree<out T> where T : IWorldObject
    {
        IEnumerable<T> GetWorldObjects(ChunkCoordinate coord);
        Vector3 GetChunkRadius(ChunkCoordinate chunk);
        Vector3 GetChunkCenter(ChunkCoordinate chunk);
        BoundingBox GetChunkBoundingBox(ChunkCoordinate chunk);
        bool IsLeaf(ChunkCoordinate chunk);
    }
}