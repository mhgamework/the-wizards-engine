using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Profiling;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Responsible for providing access to the engine's debug information (providing access and managing?)
    /// </summary>
    public class DebugWrapper
    {
        public DebugWrapper()
        {
            MainProfilingPoint = Profiler.CreateElement("[THE WIZARDS ENGINE]");
        }

        public ProfilingPoint MainProfilingPoint { get; private set; }
        public bool NeedsReload { get; set; }
    }
}
