using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using MHGameWork.TheWizards.Diagnostics.Profiling;
using MHGameWork.TheWizards.Profiling;

namespace MHGameWork.TheWizards.Engine
{
    public class EngineDebugTools
    {
        public ProfilerComponent Profiler { get; private set; }
        private Window window;

        public EngineDebugTools(ProfilingPoint rootPoint)
        {
            //TODO: use diagnosticscomponent
            Profiler = new ProfilerComponent();
            Profiler.SetRootPoint(rootPoint);
            window = new Window() { Content = Profiler.GetView() };
            window.Width = 400;
            window.Height = 300;
        }

        public void Show()
        {
            window.Show();
        }
    }
}
