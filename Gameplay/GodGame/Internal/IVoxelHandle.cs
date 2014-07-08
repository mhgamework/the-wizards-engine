using System;
using System.Collections.Generic;
using DirectX11;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class IVoxelHandle
    {
        private readonly World world;
        private GameVoxel currentVoxel;

        public IVoxelHandle(World world)
        {
            this.world = world;
            Seeder = new Seeder(0);
        }

        public IVoxelHandle(World world, GameVoxel gameVoxel) : this(world)
        {
            CurrentVoxel = gameVoxel;
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

        public IEnumerable<IVoxelHandle> Get8Connected()
        {
            return world.Get8Connected(CurrentVoxel.Coord).Select(encapsulate);
        }

        public Seeder Seeder { get; private set; }

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

        public IEnumerable<IVoxelHandle> GetRange(int radius)
        {
            return world.GetRange(CurrentVoxel, radius).Select(encapsulate);
        }

        public VoxelData Data { get { return currentVoxel.Data; } }


        public IVoxelHandle GetRelative(Point2 p)
        {
            return encapsulate(world.GetVoxel(CurrentVoxel.Coord + p));
        }

        private IVoxelHandle encapsulate(GameVoxel v)
        {
            var ret = new IVoxelHandle(world); // TODO: use pool, or is this solved by gc?
            ret.CurrentVoxel = v;
            return ret;
        }

        public void ChangeType(GameVoxelType air)
        {
            currentVoxel.ChangeType(air);
        }

        public GameVoxelType Type
        {
            get { return currentVoxel.Type; }
        }
    }
}