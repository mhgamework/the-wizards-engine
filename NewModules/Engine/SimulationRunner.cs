using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Responsible for safely executing a collection of simulators
    /// </summary>
    public class SimulationRunner
    {
        public void simulateStep(IEnumerable<ISimulator> simulators)
        {
            foreach (var sim in simulators)
            {
                //sim.Simulate();
                simulateSave(sim);
            }
        }

        private void simulateSave(ISimulator sim)
        {
            TW.Data.RunningSimulator = sim;
            try
            {

                sim.Simulate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in simulator: {0}", sim.GetType().Name);
                Console.WriteLine(ex.ToString());

            }
            TW.Data.RunningSimulator = null;

        }
    }
}
