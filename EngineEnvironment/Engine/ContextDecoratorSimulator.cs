using System;
using MHGameWork.TheWizards.Debugging;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Decorates a simulator to make it run in the W context, and catches all exceptions
    /// </summary>
    public class ContextDecoratorSimulator : ISimulator
    {
        private readonly ISimulator decorated;
        private readonly IErrorLogger errorLogger;
        private readonly string name;

        public ContextDecoratorSimulator(ISimulator decorated,IErrorLogger errorLogger, string name)
        {
            this.decorated = decorated;
            this.errorLogger = errorLogger;
            this.name = name;
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
                errorLogger.Log(ex, "Error in simulator: " + name);
            }
            TW.Data.RunningSimulator = null;
        }
    }
}