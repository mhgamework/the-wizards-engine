using System;
using System.Collections.Generic;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Model
{
    /// <summary>
    /// Responsible for construction, lifetime and destruction of objects in the model.
    /// </summary>
    public class Level
    {
        private List<Island> islands = new List<Island>();
        private List<Traveller> travellers = new List<Traveller>();
        
        public Level()
        {
            createItemTypes();
        }

        private void createItemTypes()
        {
            AirCrystalType = new ItemType() {Name = "Air crystal"};
            ScrapType = new ItemType() { Name = "Scrap" };
        }

        public ItemType AirCrystalType { get; private set; }
        public ItemType ScrapType { get; private set; }

        public Island CreateNewIsland(Vector3 position)
        {
            var ret = new Island(this) { Position = position };
            islands.Add(ret);
            return ret;
        }
        public Traveller CreateNewTraveller(Island start, Func<Island> destinationAction)
        {
            var ret = new Traveller()
                          {
                              BridgePosition = new BridgePosition(start, start, 0),
                              PlannedPath = new[] { start },
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

        public void RemoveIsland(Island island)
        {
            islands.Remove(island);
        }
    }
}