using System;
using MHGameWork.TheWizards.Scattered.Model;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Simulation.Constructions
{
    public class CrystalCliffsAction : IConstructionAction
    {
        private readonly Island island;
        private float nextCliffTimeout;
        private Level level;
        private const float Interval = 5;

        public CrystalCliffsAction(Island island)
        {
            this.island = island;
            this.level = island.Level;
            nextCliffTimeout = TW.Graphics.TotalRunTime + Interval;
        }

        public void Update()
        {
            if (island.Inventory.GetAmountOfType(level.AirCrystalType) >= 4)
            {
                createCrystalToWarehouseCart();
                return;
            }
            if (nextCliffTimeout > TW.Graphics.TotalRunTime) return;

            nextCliffTimeout = TW.Graphics.TotalRunTime + Interval;
            island.Inventory.AddNewItems(level.AirCrystalType, 1);
        }

        private Traveller createCrystalToWarehouseCart()
        {
            Traveller trav = null;
            var airCrystalType = level.AirCrystalType;
            trav = level.CreateNewTraveller(island, delegate
            {
                if (isWarehouse(trav.Island))
                {
                    // When at warehouse, drop off goods
                    trav.Inventory.TransferItemsTo(trav.Island.Inventory, airCrystalType,
                                                   trav.Inventory.GetAmountOfType(
                                                       airCrystalType));
                }

                if (trav.Inventory.GetAmountOfType(airCrystalType) > 0)
                    return findNearestWarehouse(); // When has goods, go to warehouse

                if (trav.IsAtIsland(island))
                {
                    // When home and empty, no more destination!
                    return null;
                }

                return island; // go home!
            });

            island.Inventory.TransferItemsTo(trav.Inventory, airCrystalType, island.Inventory.GetAmountOfType(airCrystalType));

            return trav;

        }

        private Island findNearestWarehouse()
        {
            return level.Islands.First(isWarehouse); //TODO
        }

        private bool isWarehouse(Island island1)
        {
            return island1.Construction.Name == "Warehouse"; //TODO
        }
    }
}