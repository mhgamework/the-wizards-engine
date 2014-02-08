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
        private readonly RoundSimulator roundSimulator;
        private Level level;
        private Traveller cart;

        private bool harvestedEnergyNode;

        public CrystalCliffsAction(Island island, DistributionHelper distribution,RoundSimulator roundSimulator)
        {
            this.island = island;
            this.distribution = distribution;
            this.roundSimulator = roundSimulator;
            this.level = island.Level;
        }

        public void Update()
        {
            if (tryDeliverCrystals()) return; // Cart not back yet

            roundSimulator.ExecuteDuringBuildPhase(() =>
                {
                    if (harvestedEnergyNode) return;

                    var closestNode = distribution.FindNearestIsland(island, isEnergyNode);
                    if (closestNode == null) return;

                    closestNode.Inventory.DestroyItems(level.AirEnergyType, 1);

                    island.Inventory.AddNewItems(level.AirCrystalType, 1);
                    harvestedEnergyNode = true;


                });
            roundSimulator.ExecuteDuringCombatPhase(() =>
                {
                    harvestedEnergyNode = false;
                });



        }

        private bool isEnergyNode(Island arg)
        {
            return arg.Construction.Name == "Energy Node";
        }

        private bool tryDeliverCrystals()
        {
            if (cart != null && cart.Destination == null) cart = null; // Cleanup removed cart
            if (island.Inventory.GetAmountOfType(level.AirCrystalType) >= 4)
            {
                if (cart != null) return true;
                cart = distribution.CreateDeliveryCart(island, isWarehouse);
                if (cart == null) return true;
                cart.Inventory.TakeAll(island.Inventory);
                return true;
            }
            return false;
        }

        private bool isWarehouse(Island island1)
        {
            return island1.Construction.Name == "Warehouse"; //TODO
        }
    }
}