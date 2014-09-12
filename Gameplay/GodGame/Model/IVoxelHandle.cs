using System;
using System.Collections.Generic;
using DirectX11;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Internal.Model
{
    /// <summary>
    /// TODO: maybe add an IVoxelHandle property to each GameVoxel?
    /// The problem now is that we cannot hold references to IVoxelHandles, since the objects themselves are flyweights and thus cannot be used to identify voxels
    /// </summary>
    public class IVoxelHandle
    {
        private readonly World world;
        private GameVoxel currentVoxel;

        private static Seeder staticSeeder = new Seeder((new Random()).Next());

        public IVoxelHandle(World world)
        {
            this.world = world;
            Seeder = staticSeeder;//TODO: Warning: possible problems!  //new Seeder((new Random()).Next());
        }
        public IVoxelHandle(World world, GameVoxel gameVoxel)
            : this(world)
        {
            CurrentVoxel = gameVoxel;
        }
        public IVoxelHandle(GameVoxel gameVoxel)
            : this(gameVoxel.World, gameVoxel)
        {
        }
        private IVoxelHandle encapsulate(GameVoxel v)
        {
            var ret = new IVoxelHandle(world); // TODO: use pool, or is this solved by gc?
            ret.CurrentVoxel = v;
            return ret;
        }

        /// <summary>
        /// Internal use only!
        /// </summary>
        public GameVoxel CurrentVoxel
        {
            private get
            {
                if (currentVoxel == null) throw new InvalidOperationException("Tickhandle has no voxel set!"); return currentVoxel;
            }
            set { currentVoxel = value; }
        }

        /// <summary>
        /// DO NOT USE THIS IN GAMEPLAY LAYER!
        /// Simply exists because some design issues are to time consuming and not relevant enough to solve.
        /// </summary>
        /// <returns></returns>
        public GameVoxel GetInternalVoxel()
        {
            return currentVoxel;
        }

        #region Spatial
        public IEnumerable<IVoxelHandle> Get8Connected()
        {
            return world.Get8Connected(CurrentVoxel.Coord).Where(s => s != null).Select(encapsulate);
        }
        /// <summary>
        /// Return order: (1,0) (0,1) (-1,0) (0,-1)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IVoxelHandle> Get4Connected()
        {
            var ret = new IVoxelHandle[4];
            ret[0] = GetRelative(new Point2(1, 0));
            ret[1] = GetRelative(new Point2(0, 1));
            ret[2] = GetRelative(new Point2(-1, 0));
            ret[3] = GetRelative(new Point2(0, -1));
            return ret.Where(s => s != null);
        }
        public IEnumerable<IVoxelHandle> GetRange(int radius)
        {
            return world.GetRange(CurrentVoxel, radius).Select(encapsulate);
        }
        public IEnumerable<IVoxelHandle> GetRangeCircle(int radius)
        {
            return world.GetRange(CurrentVoxel, radius).Where(v => (v.Coord - currentVoxel.Coord).GetLength() <= radius).Select(encapsulate);
        }
        public IVoxelHandle GetRelative(Point2 p)
        {
            return encapsulate(world.GetVoxel(CurrentVoxel.Coord + p));
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

        public IVoxelData Data { get { return currentVoxel.Data; } }

        public Seeder Seeder { get; private set; }

        public void ChangeType(GameVoxelType type)
        {
            //note: put gameplay-related changes here
            var prevHeight = currentVoxel.Data.Height;
            currentVoxel.ChangeType(type);
            currentVoxel.Data.Height = prevHeight;

            if (type is WaterType)
            {
                var minHeight = Get4Connected().Min(e => e.Data.Height);
                currentVoxel.Data.Height = minHeight;
            }

        }
        public GameVoxelType Type
        {
            get { return currentVoxel.Type; }
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
            return Type.CanAddWorker(this);
        }

        protected bool Equals(IVoxelHandle other)
        {
            return Equals(currentVoxel, other.currentVoxel);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IVoxelHandle)obj);
        }

        public override int GetHashCode()
        {
            return (currentVoxel != null ? currentVoxel.GetHashCode() : 0);
        }

        public float DistanceTo(IVoxelHandle handle)
        {
            return (handle.currentVoxel.Coord - currentVoxel.Coord).GetLength();
        }
    }
}