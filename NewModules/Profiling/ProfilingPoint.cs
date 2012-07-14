using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Profiling
{
    /// <summary>
    /// Responsible for representing a code unit that gets profiled.
    /// </summary>
    public class ProfilingPoint
    {
        private readonly Profiler profiler;
        private readonly string name;

        public ProfilingPoint(Profiler profiler, string name)
        {
            this.profiler = profiler;
            this.name = name;
        }

        private bool started = false;
        private TimeSpan start;

        

        public void Begin()
        {
            if (started) throw new InvalidOperationException("This element has already been started!!" );
            started = true;
            start = Configuration.Timer.Elapsed;

        }
        public void End()
        {
            started = false;

            var duration = (Configuration.Timer.Elapsed - start).TotalMilliseconds;
        }


    }
}
