using System;
using System.Collections.Generic;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Model
{
    public class Level
    {
        private List<Island> islands = new List<Island>();
        private List<Traveller> travellers = new List<Traveller>();
        public Island CreateNewIsland(Vector3 position)
        {
            var ret=  new Island(){Position = position};
            islands.Add(ret);
            return ret;
        }
        public Traveller CreateNewTraveller(Island start, Func<Island> destinationAction )
        {
            var ret = new Traveller()
                          {
                              BridgePosition =  new BridgePosition(start,start,0),
                              PlannedPath = new []{start},
                              DetermineDestinationAction = destinationAction
                          };
            travellers.Add(ret);

            return ret;

        }

        public IEnumerable<Island> Islands { get { return islands; } }

        public IEnumerable<Traveller> Travellers { get { return travellers; } }

        public void RemoveTraveller(Traveller traveller)
        {
            travellers.Remove(traveller);
        }
    }
}