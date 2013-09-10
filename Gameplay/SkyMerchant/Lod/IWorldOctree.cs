using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.Worlding;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Lod
{
    /// <summary>
    /// Responsible for providing hierarchial chunk based access to the world.
    /// </summary>
    public interface IWorldOctree
    {
        IEnumerable<Physical> GetPhysicals(ChunkCoordinate coord);
        Vector3 GetChunkRadius(ChunkCoordinate chunk);
        Vector3 GetChunkCenter(ChunkCoordinate chunk);
        BoundingBox GetChunkBoundingBox(ChunkCoordinate chunk);
        bool IsLeaf(ChunkCoordinate chunk);
    }
}