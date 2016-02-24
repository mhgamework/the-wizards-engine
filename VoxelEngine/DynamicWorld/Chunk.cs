using DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.Meshes;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld
{
    /// <summary>
    /// Represents a regular grid world Chunk, part of the WorldHolder
    /// </summary>
    public class Chunk
    {
        private readonly Point3 coord;
        public SignedOctreeNode SignedOctree;

        public Chunk( Point3 coord )
        {
            this.coord = coord;
        }

        public Point3 Coord
        {
            get { return coord; }
        }

        public DeferredMeshElement Mesh { get; set; }

        public override string ToString()
        {
            return string.Format( "Coord: {0}", coord );
        }
    }
}