using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame
{
    public class GameVoxel
    {
        private readonly World world;
        private readonly Point2 coord;

        public GameVoxel(World world, Point2 coord)
        {
            this.world = world;
            this.coord = coord;
        }

        public void ChangeType(GameVoxelType air)
        {
            Type = air;
        }

        public GameVoxelType Type { get; private set; }

        public BoundingBox GetBoundingBox()
        {
            return world.GetBoundingBox(coord);
        }
    }
}