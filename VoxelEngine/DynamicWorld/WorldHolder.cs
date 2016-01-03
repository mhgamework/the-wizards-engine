using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld
{
    /// <summary>
    /// Manages chunk data storage and run-time lifetime
    /// This world representation assumes a chunk-based data structure
    /// </summary>
    public class WorldHolder
    {
        private readonly Point3 size;

        private Array3D<Chunk> chunks;

        public WorldHolder(Point3 size)
        {
            this.size = size;

            chunks = new Array3D<Chunk>(size);
            chunks.ForEach((c, p) => chunks[p] = new Chunk(p));
        }

        public Chunk GetChunk(Point3 coord)
        {
            return chunks[coord];
        }
    }
}