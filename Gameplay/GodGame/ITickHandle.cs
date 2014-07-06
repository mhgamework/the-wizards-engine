using System;
using System.Collections;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.GodGame
{
    public class ITickHandle
    {
        private readonly World world;

        public ITickHandle(World world)
        {
            this.world = world;
            Seeder = new Seeder(0);
        }

        public IEnumerable<GameVoxel> Get8Connected(GameVoxel v)
        {
            return world.Get8Connected(v.Coord);
        }

        public Seeder Seeder { get; private set; }

        public float Elapsed { get { return TW.Graphics.Elapsed; } }

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
    }
}