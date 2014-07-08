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
            Data = new VoxelData();
        }

        public GameVoxelType Type { get; private set; }

        public VoxelData Data { get; set; }

        public int MagicLevel
        {
            get { return Data.MagicLevel; }
            set { Data.MagicLevel = value; }
        }

        public int DataValue
        {
            get { return Data.DataValue; }
            set { Data.DataValue = value; }
        }

        public BoundingBox GetBoundingBox()
        {
            return world.GetBoundingBox(Coord);
        }
    }
}