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

        public Stack<ProfilingPoint> pointStack = new Stack<ProfilingPoint>();
        public string lastResult;

        public Profiler()
        {
            
        }

        public static string GetLastResult()
        {
            return instance.lastResult;
        }

        public static ProfilingPoint CreateElement(string name)
        {
            var el = new ProfilingPoint(instance, name);
            
            return el;
        }

        

       
    }
}
