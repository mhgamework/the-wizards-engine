using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Scattered.Simulation;
using MHGameWork.TheWizards.Scattered.Simulation.Constructions;
using SlimDX;
using System.Linq;
using Castle.Core.Internal;

namespace MHGameWork.TheWizards.Scattered.Model
{
    /// <summary>
    /// Responsible for construction, lifetime and destruction of objects in the model.
    /// 
    /// TODO: split into gamestate + constructors?
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
            island.RenderData.Dispose();
            island.RenderData = null;
            islands.Remove(island);
        }


        public void ClearAll()
        {
            islands.ToArray().ForEach(RemoveIsland);
            travellers.ToArray().ForEach(RemoveTraveller);
        }


        #region Item types

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

        #endregion

        #region Cart Types

        public TravellerType WorkshopCartType { get; private set; }
        public TravellerType DeliveryCartType { get; private set; }

        #endregion

        #region Construction Types
        // Note that this region is a leak, it uses simulation code. IoC should be used to allow simulation code to provide implementations for these methods.

        public Construction createEmptyConstruction(Island arg)
        {
            return new Construction()
                       {
                           Name = "Empty",
                           UpdateAction = cFactory.CreateConstructionAction<NullConstructionAction>(arg),
                           LevelConstructorMethod = "createEmptyConstruction"
                           // TODO: try get method name using code or use AOP or add ConstructionType
                       };
        }

        public Construction createWarehouseConstruction(Island arg)
        {
            return new Construction()
                       {
                           Name = "Warehouse",
                           UpdateAction = new NullConstructionAction(),
                           LevelConstructorMethod = "createWarehouseConstruction"
                           // TODO: try get method name using code or use AOP
                       };
        }

        public Construction createCrysalCliffsConstruction(Island arg)
        {
            return new Construction()
                       {
                           Name = "Crystal Cliffs",
                           UpdateAction = cFactory.CreateConstructionAction<CrystalCliffsAction>(arg),
                           LevelConstructorMethod = "createCrysalCliffsConstruction"
                           // TODO: try get method name using code or use AOP
                       };
        }

        public Construction createEnergyNodeConstruction(Island arg)
        {
            return new Construction()
                       {
                           Name = "Energy Node",
                           UpdateAction = cFactory.CreateConstructionAction<EnergyNodeAction>(arg),
                           LevelConstructorMethod = "createEnergyNodeConstruction"
                           // TODO: try get method name using code or use AOP
                       };
        }

        public Construction createScrapStationConstruction(Island arg)
        {
            return new Construction()
                       {
                           Name = "Scrap Station",
                           UpdateAction = cFactory.CreateConstructionAction<ScrapStationAction>(arg),
                           LevelConstructorMethod = "createScrapStationConstruction"
                           // TODO: try get method name using code or use AOP
                       };
        }

        public Construction createCampConstruction(Island arg)
        {
            return new Construction()
                       {
                           Name = "Camp",
                           UpdateAction = cFactory.CreateConstructionAction<NullConstructionAction>(arg),
                           LevelConstructorMethod = "createCampConstruction"
                           // TODO: try get method name using code or use AOP
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

        #endregion



    }
}