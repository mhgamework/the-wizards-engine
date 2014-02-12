using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Scattered.Simulation;
using MHGameWork.TheWizards.Scattered.Simulation.Constructions;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Model
{
    /// <summary>
    /// Responsible for construction, lifetime and destruction of objects in the model.
    /// </summary>
    public class Level
    {
        private readonly ConstructionFactory cFactory;
        private List<Island> islands = new List<Island>();
        private List<Traveller> travellers = new List<Traveller>();

        public Level(ConstructionFactory cFactory)
        {
            this.cFactory = cFactory;
            createItemTypes();
        }

        private void createItemTypes()
        {
            UnitTier1Type = new ItemType() { Name = "Air Unit Tier 1" };
            AirCrystalType = new ItemType() { Name = "Air crystal" };
            AirEnergyType = new ItemType() { Name = "Air energy" };
            ScrapType = new ItemType() { Name = "Scrap" };

            WorkshopCartType = new TravellerType() { IsEnemy = false, Name = "Workshop Cart" };
            DeliveryCartType = new TravellerType() { IsEnemy = false, Name = "Delivery Cart" };
        }

        public ItemType UnitTier1Type { get; private set; }
        public ItemType AirCrystalType { get; private set; }
        public ItemType AirEnergyType { get; private set; }
        public ItemType ScrapType { get; private set; }

        public TravellerType WorkshopCartType { get; private set; }
        public TravellerType DeliveryCartType { get; private set; }

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





        public Construction createEmptyConstruction(Island arg)
        {
            return new Construction()
            {
                Name = "Empty",
                UpdateAction = cFactory.CreateConstructionAction<NullConstructionAction>(arg),
                LevelConstructorMethod = "createEmptyConstruction" // TODO: try get method name using code or use AOP or add ConstructionType
            };
        }

        public Construction createWarehouseConstruction(Island arg)
        {
            return new Construction()
            {
                Name = "Warehouse",
                UpdateAction = new NullConstructionAction(),
                LevelConstructorMethod = "createWarehouseConstruction" // TODO: try get method name using code or use AOP
            };
        }

        public Construction createCrysalCliffsConstruction(Island arg)
        {
            return new Construction()
            {
                Name = "Crystal Cliffs",
                UpdateAction = cFactory.CreateConstructionAction<CrystalCliffsAction>(arg),
                LevelConstructorMethod = "createCrysalCliffsConstruction" // TODO: try get method name using code or use AOP
            };
        }

        public Construction createEnergyNodeConstruction(Island arg)
        {
            return new Construction()
            {
                Name = "Energy Node",
                UpdateAction = cFactory.CreateConstructionAction<EnergyNodeAction>(arg),
                LevelConstructorMethod = "createEnergyNodeConstruction" // TODO: try get method name using code or use AOP
            };
        }

        public Construction createScrapStationConstruction(Island arg)
        {
            return new Construction()
            {
                Name = "Scrap Station",
                UpdateAction = cFactory.CreateConstructionAction<ScrapStationAction>(arg),
                LevelConstructorMethod = "createScrapStationConstruction" // TODO: try get method name using code or use AOP
            };
        }

        public Construction createCampConstruction(Island arg)
        {
            return new Construction()
            {
                Name = "Camp",
                UpdateAction = cFactory.CreateConstructionAction<NullConstructionAction>(arg),
                LevelConstructorMethod = "createCampConstruction" // TODO: try get method name using code or use AOP
            };
        }

        public Construction createWorkshop(Island arg)
        {
            return new Construction()
            {
                Name = "Workshop",
                UpdateAction = cFactory.CreateConstructionAction<WorkshopAction>(arg),
                LevelConstructorMethod = "createWorkshop" // TODO: try get method name using code or use AOP
            };
        }


    }
}