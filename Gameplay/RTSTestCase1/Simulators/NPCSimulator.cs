using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1._Tests;

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
        public NPCSimulator()
        {
            
        }

        public void Simulate()
        {
            GoblinAttackSimulator.Simulate();
        }
    }
}