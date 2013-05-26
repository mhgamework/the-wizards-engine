using System;
using MHGameWork.TheWizards.Debugging;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Decorates a simulator to make it show up the the profiler
    /// </summary>
    public class ContextDecoratorSimulator : ISimulator
    {
        private readonly ISimulator decorated;
        private readonly IErrorLogger errorLogger;

        public ContextDecoratorSimulator(ISimulator decorated,IErrorLogger errorLogger)
        {
            this.decorated = decorated;
            this.errorLogger = errorLogger;
        }


        public void Simulate()
        {
            var sim = decorated;

            TW.Data.RunningSimulator = sim;
            try
            {

                sim.Simulate();
            }
            catch (Exception ex)
            {
                errorLogger.Log(ex, "Error in simulator: " + sim.GetType().Name);
            }
            TW.Data.RunningSimulator = null;
        }
    }
}