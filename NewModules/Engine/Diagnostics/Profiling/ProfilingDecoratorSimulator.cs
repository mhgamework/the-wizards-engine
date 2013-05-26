using System;
using MHGameWork.TheWizards.Profiling;

namespace MHGameWork.TheWizards.Engine.Diagnostics.Profiling
{
    /// <summary>
    /// Decorates a simulator to make it show up the the profiler
    /// </summary>
    public class ProfilingDecoratorSimulator : ISimulator
    {
        private readonly ISimulator decorated;
        private ProfilingPoint profilingPoint;

        public ProfilingDecoratorSimulator(ISimulator decorated)
        {
            this.decorated = decorated;
            profilingPoint = Profiler.CreateElement("Simulator: " + decorated.GetType().Name);
        }


        public void Simulate()
        {
            profilingPoint.Begin();
            decorated.Simulate();
            profilingPoint.End();
        }
    }
}