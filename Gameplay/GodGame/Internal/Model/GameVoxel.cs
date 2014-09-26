using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Internal.Model
{
    /// <summary>
    /// Represents a voxel in the world, identified by the coordinate.
    /// The voxel contains a ITile which represents the contents of a voxel
    /// Somewhat of a configuration class?
    /// </summary>
    public class GameVoxel : IVoxel, IVoxelHandle
    {
        private readonly World world;
        public Point2 Coord { get; private set; }

        public GameVoxel(World world, Point2 coord)
        {
            this.world = world;
            this.Coord = coord;
            initVoxelHandler();
            Data = new ObservableVoxelData(() =>
            {
                world.NotifyVoxelChanged(this);
                if (Type != PreviousType)
                    TypeChanged = true;
                PreviousType = Type;
            });

        }

        public IGameVoxelType PreviousType { get; set; }
        public IGameVoxelType Type
        {
            get { return Data.Type; }
        }

        public bool TypeChanged { get; set; }

        public IVoxel GetRelative(Point2 offset)
        {
            return world.GetVoxel(Coord + offset);
        }

        public Point2 GetOffset(IVoxel other)
        {
            return ((GameVoxel)other).Coord - Coord;
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


        #region "IVoxelHandle"
        private IVoxel currentVoxel;

        private static Seeder staticSeeder = new Seeder((new Random()).Next());

        private void initVoxelHandler()
        {
            Seeder = staticSeeder;//TODO: Warning: possible problems!  //new Seeder((new Random()).Next());
        }

        /// <summary>
        /// Internal use only!
        /// </summary>
        private IVoxel CurrentVoxel
        {
            get { return this; }
        }

        /// <summary>
        /// DO NOT USE THIS IN GAMEPLAY LAYER!
        /// Simply exists because some design issues are to time consuming and not relevant enough to solve.
        /// </summary>
        /// <returns></returns>
        public GameVoxel GetInternalVoxel()
        {
            return (GameVoxel)currentVoxel;
        }

        #region Spatial
        public IEnumerable<IVoxelHandle> Get8Connected()
        {
            return currentVoxel.Get8Connected().Cast<IVoxelHandle>();
        }
        /// <summary>
        /// Return order: (1,0) (0,1) (-1,0) (0,-1)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IVoxelHandle> Get4Connected()
        {
            return currentVoxel.Get4Connected().Cast<IVoxelHandle>();
        }
        public IEnumerable<IVoxelHandle> GetRange(int radius)
        {
            return currentVoxel.GetRange(radius).Cast<IVoxelHandle>();
        }
        public IEnumerable<IVoxelHandle> GetRangeCircle(int radius)
        {
            return currentVoxel.GetRangeCircle(radius).Cast<IVoxelHandle>();
        }

        IVoxelHandle IVoxelHandle.GetRelative(Point2 p)
        {
            return (IVoxelHandle)currentVoxel.GetRelative(p);
        }

        Point2 IVoxelHandle.GetOffset(IVoxel other)
        {
            return currentVoxel.GetOffset(other);
        }

        #endregion


        public float TickLength { get { return TW.Graphics.Elapsed; } }
        public float TotalTime { get { return TW.Graphics.TotalRunTime; } }

        /// <summary>
        /// When called ensures that on average each ('averageInterval' seconds) the function returns true,
        /// as long as the elapsed value is small enough compared to the average interval 
        /// (otherwise multiple events could occur in a single interval)
        /// Uses Poisson distribution for 1 or more events inside a single interval
        /// TODO: only executes action once, could execute multiple times to be more correct 
        /// </summary>
        /// <returns></returns>
        public void EachRandomInterval(float averageInterval, Action action)
        {
            Seeder.EachRandomInterval(averageInterval, action, TickLength);
        }

        public Seeder Seeder { get; private set; }

        public void ChangeType(IGameVoxelType type)
        {
            //note: put gameplay-related changes here
            var prevHeight = currentVoxel.Data.Height;
            currentVoxel.Data.Type = type;
            currentVoxel.Data.Height = prevHeight;

            if (type is WaterType)
            {
                var minHeight = Get4Connected().Min(e => e.Data.Height);
                currentVoxel.Data.Height = minHeight;
            }

        }

        public bool CanAcceptItemType(ItemType type)
        {
            return Type.CanAcceptItemType(this, type);
        }

        public bool CanAcceptItemType(IVoxelHandle deliveryHandle, ItemType type)
        {
            return Type.CanAcceptItemType(this, deliveryHandle, type);
        }

        public bool CanAddWorker()
        {
            return ((GameVoxelType)Type).CanAddWorker(this);
        }

        public float DistanceTo(IVoxelHandle handle)
        {
            return handle.GetInternalVoxel().GetOffset(handle.GetInternalVoxel()).GetLength();
        }

        #endregion
    }
}