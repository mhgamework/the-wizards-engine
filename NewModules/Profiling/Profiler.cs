using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Profiling
{
    /// <summary>
    /// Responsible for managing profiling points throughout the application and calculating elapsed time.
    /// </summary>
    public class Profiler
    {
        private static Profiler instance = new Profiler();

        public static ProfilingPoint CreateElement(string name)
        {
            var el = new ProfilingPoint(instance, name);

            return el;
        }
    }
}
