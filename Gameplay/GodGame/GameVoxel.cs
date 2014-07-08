using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Split this class into a 'enginemodelobject' part, for data storage, and a 'domain model part', 
    /// which should be merged with the IVoxelHandle probably
    /// </summary>
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
            DataValue = 0;
        }

        public GameVoxelType Type { get; private set; }

        public int DataValue { get; set; }

        public int MagicLevel { get; set; }


        public BoundingBox GetBoundingBox()
        {
            return world.GetBoundingBox(Coord);
        }
    }
}