using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.WorldRendering;

namespace MHGameWork.TheWizards.Engine
{
    public class UISimulator :ISimulator
    {
        private TextareaUpdater textareaSimulator;

        public UISimulator()
        {
            textareaSimulator = new TextareaUpdater();

        }

        public void Simulate()
        {

            textareaSimulator.Update();
            textareaSimulator.Render();
        }
    }
}
