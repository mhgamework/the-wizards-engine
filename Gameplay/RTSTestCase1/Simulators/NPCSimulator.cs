using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Building;
using MHGameWork.TheWizards.RTSTestCase1.Cannons;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Spawning;
using MHGameWork.TheWizards.RTSTestCase1._Tests;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.Simulators
{
    /// <summary>
    /// Simulates all NPC's (Enemy, friendly, machines)
    /// </summary>
    public class NPCSimulator : ISimulator
    {
        /// <summary>
        /// Dependency
        /// </summary>
        public GoblinAttackSimulator GoblinAttackSimulator { get; set; }
        public CannonSimulator CannonSimulator { get; set; }
        public GoblinSpawner GoblinSpawner { get; set; }
        public SimpleBuilder SimpleBuilder { get; set; }
        public NPCSimulator(GoblinSpawner s)
        {
            GoblinSpawner = s;
        }

        public void Simulate()
        {
            GoblinAttackSimulator.Simulate();
            SimulateSpawn();
            simulateBuilding();
            CannonSimulator.Simulate();
        }

        private void SimulateSpawn()
        {
            var points = TW.Data.Objects.OfType<GoblinSpawnPoint>();
            foreach (var goblinSpawnerPoint in points.ToArray())
            {
                GoblinSpawner.Simulate(TW.Graphics.Elapsed, goblinSpawnerPoint);
            }
        }

        private float time = 0;
        private void simulateBuilding()
        {
            if (time > TW.Graphics.TotalRunTime) return;
            time = TW.Graphics.TotalRunTime + 1;

            SimpleBuilder.BuildRange = 5;
            foreach (var b in TW.Data.Objects.OfType<IBuildable>().Where(o=>o.Buildable.BuildProgress < 1).ToArray())
            {
                SimpleBuilder.BuildSingleResource(b);
            }
        }
    }
}