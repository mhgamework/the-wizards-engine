using DirectX11;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld
{
    /// <summary>
    /// Generates a world where the first 'height' y chunks are solid and the rest is air.
    /// </summary>
    public class FlatWorldGenerator
    {
        private int height;

        public FlatWorldGenerator(int height)
        {
            this.height = height;
        }

        public void GenerateChunk(Chunk c)
        {
            var sign = c.Coord.Y <= height;

            var octree = new SignedOctreeNode()
            {
                Children = null,
                Depth = 0,
                LowerLeft = new Point3(),
                Size = 128,
                Signs = new bool[] { sign, sign, sign, sign, sign, sign, sign, sign }

            };
            c.SignedOctree = octree;
        }
    }
}