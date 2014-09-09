using System;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Types;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Internal.Model
{
    /// <summary>
    /// Split this class into a 'enginemodelobject' part, for data storage, and a 'domain model part', 
    /// which should be merged with the IVoxelHandle probably
    /// Somewhat of a configuration class?
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

        public void ChangeType(GameVoxelType type)
        {
            if (type == null) throw new InvalidOperationException("Cannot set null type!!");
            
            Data = new ObservableVoxelData(new VoxelDataStore(), () =>
                {
                    world.NotifyVoxelChanged(this);
                    if (Type != oldType)
                        TypeChanged = true;
                });
            Data.Type = type;
        }

        public GameVoxelType Type
        {
            get { return Data.Type; }
            private set { Data.Type = value; }
        }

        private GameVoxelType oldType;
        public bool TypeChanged { get; set; }

        public IVoxelData Data { get; set; }

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