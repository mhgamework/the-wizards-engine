using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered
{
    public class Level
    {
        private List<Island> islands = new List<Island>();
        public Island CreateNewIsland(Vector3 position)
        {
            var ret=  new Island(){Position = position};
            islands.Add(ret);
            return ret;
        }

        public IEnumerable<Island> Islands { get { return islands; } }


    }
}