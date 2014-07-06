using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame
{
    public class GameVoxel
    {
        private readonly World world;
        public Point2 Coord { get; private set; }

        public GameVoxel(World world, Point2 coord)
        {
            this.world = world;
            this.Coord = coord;
        }

        public void ChangeType(GameVoxelType air)
        {
            Type = air;
        }

        public GameVoxelType Type { get; private set; }

        public int DataValue { get; set; }


        public BoundingBox GetBoundingBox()
        {
            return world.GetBoundingBox(Coord);
        }
    }
}