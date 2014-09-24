using System;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Types;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Internal.Model
{
    /// <summary>
    /// Represents a voxel in the world, identified by the coordinate.
    /// The voxel contains a ITile which represents the contents of a voxel
    /// Somewhat of a configuration class?
    /// </summary>
    public class GameVoxel : IVoxel
    {
        private readonly World world;
        public Point2 Coord { get; private set; }

        public GameVoxel(World world, Point2 coord)
        {
            this.world = world;
            this.Coord = coord;

            Data = new ObservableVoxelData(new VoxelDataStore(), () =>
            {
                world.NotifyVoxelChanged(this);
                if (Type != oldType)
                    TypeChanged = true;
                oldType = Type;
            });

        }

        public GameVoxelType Type
        {
            get { return Data.Type; }
        }

        private GameVoxelType oldType;
        public bool TypeChanged { get; set; }

        public IVoxel GetRelative(Point2 offset)
        {
            return world.GetVoxel(Coord + offset);
        }

        public Point2 GetOffset(IVoxel other)
        {
            return ((GameVoxel) other).Coord - Coord;
        }

        public IVoxelData Data { get; set; }

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