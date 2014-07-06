using System;
using System.Collections;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.GodGame
{
    public class ITickHandle
    {
        private readonly World world;
        private GameVoxel currentVoxel;

        public ITickHandle(World world)
        {
            this.world = world;
            Seeder = new Seeder(0);
        }
        /// <summary>
        /// WARNING: setter only for internal use!
        /// </summary>
        public GameVoxel CurrentVoxel
        {
            get
            {
                if (currentVoxel == null) throw new InvalidOperationException("Tickhandle has no voxel set!"); return currentVoxel; }
            set { currentVoxel = value; }
        }

        public IEnumerable<GameVoxel> Get8Connected()
        {
            return world.Get8Connected(CurrentVoxel.Coord);
        }

        public Seeder Seeder { get; private set; }

        public float Elapsed { get { return TW.Graphics.Elapsed; } }

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
            Seeder.EachRandomInterval(averageInterval, action, Elapsed);
        }

        public IEnumerable<GameVoxel> GetRange(int radius)
        {
            return world.GetRange(CurrentVoxel, radius);
        }
    }
}