using System;

namespace MHGameWork.TheWizards.Scattered.Simulation
{
    /// <summary>
    /// Note: Convert to an interface??
    /// </summary>
    public class RoundState
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