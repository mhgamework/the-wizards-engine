using DirectX11;
using MHGameWork.TheWizards.DualContouring.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring.Building
{
    public class Chunk
    {
        public Point3 ChunkCoord { get; private set; }
        public HermiteDataGrid Grid { get; private set; }
        private VoxelSurface surface;
        public BoundingBox Box { get; private set; }

        /* private readonly HermiteDataGrid grid;
        public Vector3 Position;
        public BoundingBox Box;
        public VoxelSurface Surface;*/

        public Chunk(Point3 chunkCoord)
        {
            SurfaceDirty = true;
            this.ChunkCoord = chunkCoord;
            Box = new BoundingBox(chunkCoord.ToVector3() * BuilderConfiguration.VoxelSize * BuilderConfiguration.ChunkNumVoxels,
                (chunkCoord + new Point3(1, 1, 1)).ToVector3() * BuilderConfiguration.VoxelSize * BuilderConfiguration.ChunkNumVoxels);
        }

        public bool SurfaceDirty { get; private set; }


        public void UpdateSurface(VoxelCustomRenderer customRenderer)
        {
            if (surface != null)
                surface.Delete();
            surface = customRenderer.CreateSurface(Grid,
                                                    Matrix.Scaling(new Vector3(BuilderConfiguration.VoxelSize)) *
                                                    Matrix.Translation(ChunkCoord.ToVector3() *
                                                                        BuilderConfiguration.VoxelSize *
                                                                        BuilderConfiguration.ChunkNumVoxels));
            SurfaceDirty = false;
        }

        public void SetGrid(HermiteDataGrid grid)
        {
            this.Grid = grid;
            SurfaceDirty = true;
        }

        public void Raycast( Raycaster<Chunk> raycaster, Ray ray )
        {
            float? dist = ray.xna().Intersects(Box.xna());
            if (!dist.HasValue) return;
            if (surface == null) return;

            if ( surface.Mesh == null ) return;
            raycaster.AddIntersection(ray, surface.Mesh.Positions, surface.WorldMatrix, this);
        }


    }
}