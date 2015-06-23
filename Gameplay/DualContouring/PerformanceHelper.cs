using System;
using System.Diagnostics;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Utility functions for measuring performance.
    /// </summary>
    public class PerformanceHelper
    {
        public static Stopwatch w = new Stopwatch();

        public static TimeSpan Measure(Action act)
        {
            w.Start();
            act();
            w.Stop();
            return w.Elapsed;
        }
    }
}