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

        public ContextDecoratorSimulator(ISimulator decorated,IErrorLogger errorLogger)
        {
            this.decorated = decorated;
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
                Console.WriteLine("Error in simulator: {0}", sim.GetType().Name);
                Console.WriteLine(ex.ToString());

            }
            TW.Data.RunningSimulator = null;
        }
    }
}