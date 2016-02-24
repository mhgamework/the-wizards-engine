using DirectX11;
using MHGameWork.TheWizards.DualContouring;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld
{
    /// <summary>
    /// Generates a world where the chunk at height y has the first 'height' y chunks are solid and the rest is air.
    /// </summary>
    public class FlatWorldGenerator
    {
        private int height;
        private int size;
        private SignedOctreeNode planeGrid;

        public FlatWorldGenerator(int height, int chunkSize)
        {
            this.height = height;
            this.size = chunkSize;

            var builder = new SignedOctreeBuilder();
            planeGrid = builder.ConvertHermiteGridToOctree(new DensityFunctionHermiteGrid(p => chunkSize / 2.0f - p.Y, new Point3(chunkSize + 1, chunkSize + 1, chunkSize + 1)));

        }

        public void GenerateChunk(Chunk c)
        {
            if ( c.Coord.Y == height )
            {
                c.SignedOctree = planeGrid;
                return;
            }
            var sign = c.Coord.Y <= height;

            size = 128;
            var octree = new SignedOctreeNode()
            {
                Children = null,
                Depth = 0,
                LowerLeft = new Point3(),
                Size = size,
                Signs = new bool[] { sign, sign, sign, sign, sign, sign, sign, sign }

            };
            c.SignedOctree = octree;
        }
    }
}