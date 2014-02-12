using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.Scattered.Model;
using System.Linq;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Simulation.Constructions
{
    /// <summary>
    /// Allows magic to be collected by cliffs and generates enemies.
    /// </summary>
    public class EnergyNodeAction : IConstructionAction
    {
        private readonly Island island;
        private readonly DistributionHelper distribution;
        private readonly RoundState roundState;
        private Level level;


        public const int MaxEnergyInventory = 100;
        public const int EnemySpawnInterval = 1;

        private float nextEnemySpawn;


        public EnergyNodeAction(Island island, DistributionHelper distribution, RoundState roundState)
        {
            this.island = island;
            this.distribution = distribution;
            this.roundState = roundState;
            this.level = island.Level;

            island.Inventory.AddNewItems(level.AirEnergyType, MaxEnergyInventory - island.Inventory[level.AirEnergyType]);

        }

        public void Update()
        {
            roundState.ExecuteDuringCombatPhase(spawnEnemies);
            roundState.ExecuteDuringBuildPhase(refillEnergyInventory);


        }

        /// <summary>
        /// Cliffs take 'energy' from this island. For each energy taken, enemies accumulate here.
        /// Only executing during build phase
        /// </summary>
        private void refillEnergyInventory()
        {
        }

        private void spawnEnemies()
        {
            if (island.Inventory[level.AirEnergyType] >= MaxEnergyInventory) return;
            if (nextEnemySpawn > TW.Graphics.TotalRunTime) return;
            nextEnemySpawn = TW.Graphics.TotalRunTime + EnemySpawnInterval;

            spawnNewEnemy();
            island.Inventory.AddNewItems(level.AirEnergyType, 1);
        }

        private void spawnNewEnemy()
        {
            Traveller trav = null;
            Func<Island, bool> isDestroyableCliff = i => isCrystalCliffs(i, level.AirEnergyType) && i.Inventory[level.AirCrystalType] > 0;
            trav = level.CreateNewTraveller(island, () =>
                {
                    if (isDestroyableCliff(trav.Island))
                    {
                        // reached cliffs
                        trav.Island.Inventory.DestroyItems(level.AirCrystalType, trav.Island.Inventory[level.AirCrystalType]);
                        return null;
                    }

                    return distribution.FindNearestIsland(trav.Island, isDestroyableCliff);
                });

            trav.Type = new TravellerType() { IsEnemy = true, Name = "ENEMEMEME" };

            trav.OnReachIsland = i =>
                    {
                        if (!isCamp(i)) return;

                        if (i.Inventory[level.UnitTier1Type] == 0) return;
                        i.Inventory.DestroyItems(level.UnitTier1Type, 1);

                        trav.Destroy();
                    };

        }

        private bool isCrystalCliffs(Island isl, ItemType itemType)
        {
            return isl.Construction.Name == "Crystal Cliffs"; // TODO
        }

        private bool isCamp(Island isl)
        {
            return isl.Construction.Name == "Camp"; // TODO
        }
    }
}