using System;
using System.Diagnostics;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Utility functions for measuring performance.
    /// </summary>
    public static class PerformanceHelper
    {
        public static Stopwatch w = new Stopwatch();

        public static TimeSpan Measure(Action act)
        {
            w.Reset();
            w.Start();
            act();
            w.Stop();
            return w.Elapsed;
        }

        public static TimeSpan Multiply(this TimeSpan t, double amount)
        {
            return new TimeSpan((long)(t.Ticks * amount));

        }

    }
}