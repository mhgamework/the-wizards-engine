using System;

namespace MHGameWork.TheWizards.Scattered.Simulation.Constructions
{
    /// <summary>
    /// Note: this is the sandbox version, should split into an interface and a version that uses time for in game use
    /// </summary>
    public class RoundSimulator
    {
        public bool CombatPhase { get; set; }

        public void ExecuteDuringCombatPhase(Action action)
        {
            if (!CombatPhase) return;
            action();
        }

        public void ExecuteDuringBuildPhase(Action action)
        {
            if (CombatPhase) return;
            action();
        }
    }
}