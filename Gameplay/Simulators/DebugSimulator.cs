using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Profiling;
using MHGameWork.TheWizards.WorldRendering;

namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// Responsible for enabling debug functionality in the engine
    /// </summary>
    public class DebugSimulator : ISimulator
    {
        private Textarea profilerText;


        public DebugSimulator()
        {
            profilerText = new Textarea();
            profilerText.Position = new SlimDX.Vector2(20, 20);
            profilerText.Size = new SlimDX.Vector2(700, 500);
        }

        private float time;
        public void Simulate()
        {
            time += TW.Graphics.Elapsed;
            updateProfilerText();
        }

        private void updateProfilerText()
        {
            if (time < 1)
                return;

            time = 0;

            profilerText.Text = Profiler.GetLastResult();
        }
    }
}
