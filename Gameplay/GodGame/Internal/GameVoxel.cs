using DirectX11;
using MHGameWork.TheWizards.GodGame.Types;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Split this class into a 'enginemodelobject' part, for data storage, and a 'domain model part', 
    /// which should be merged with the IVoxelHandle probably
    /// </summary>
    public class GameVoxel
    {
        private readonly World world;
        private GameVoxelType type;
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

        public GameVoxelType Type
        {
            get { return type; }
            private set
            {
                if (type == value) return; type = value;
                TypeChanged = true;
                world.NotifyVoxelChanged(this);
            }
        }

        public bool TypeChanged { get; set; }

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

        public World World
        {
            get { return world; }
        }

        public BoundingBox GetBoundingBox()
        {
            return World.GetBoundingBox(Coord);
        }
    }
}