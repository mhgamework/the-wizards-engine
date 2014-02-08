using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.Scattered.Model;
using System.Linq;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Simulation.Constructions
{
    public class CrystalCliffsAction : IConstructionAction
    {
        private readonly Island island;
        private readonly DistributionHelper distribution;
        private float nextCliffTimeout;
        private Level level;
        private Traveller cart;
        private const float Interval = 5;

        public CrystalCliffsAction(Island island, DistributionHelper distribution)
        {
            this.island = island;
            this.distribution = distribution;
            this.level = island.Level;
            nextCliffTimeout = TW.Graphics.TotalRunTime + Interval;
        }

        public void Update()
        {
            if (cart != null && cart.Destination == null) cart = null; // Cleanup removed cart
            if (island.Inventory.GetAmountOfType(level.AirCrystalType) >= 4)
            {
                if (cart != null) return; // Cart not back yet
                cart = distribution.CreateSingleTarget(island, isWarehouse);
                if (cart == null) return;
                cart.Inventory.TakeAll(island.Inventory);
                return;
            }
            if (nextCliffTimeout > TW.Graphics.TotalRunTime) return;

            nextCliffTimeout = TW.Graphics.TotalRunTime + Interval;
            island.Inventory.AddNewItems(level.AirCrystalType, 1);
        }

        private bool isWarehouse(Island island1)
        {
            return island1.Construction.Name == "Warehouse"; //TODO
        }
    }
}