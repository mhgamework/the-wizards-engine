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
        private readonly TWEngine engine;

        public DebugWrapper(TWEngine engine)
        {
            this.engine = engine;
            MainProfilingPoint = Profiler.CreateElement("[THE WIZARDS ENGINE]");
        }

        public ProfilingPoint MainProfilingPoint { get; private set; }
        public bool NeedsReload { get; set; }

        public Exception LastException { get { return engine.EngineErrorLogger.LastException; } }
        public string LastExceptionExtra { get { return engine.EngineErrorLogger.LastExtra; } }
    }
}
