using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.VSIntegration;

namespace MHGameWork.TheWizards.Engine.Debugging
{
    public class AttachDebuggerOnStartupSimulator : ISimulator
    {
        private bool started = false;
        public void Simulate()
        {
            if (started) return;
            started = true;

            if (Debugger.IsAttached) return;
            

            var a = new VSDebugAttacher();
            a.AttachToVisualStudio();
        }
    }
}
