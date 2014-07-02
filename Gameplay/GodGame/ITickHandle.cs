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
    }
}