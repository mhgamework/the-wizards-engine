using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.WorldRendering;
using Castle.Core.Internal;

namespace MHGameWork.TheWizards.Engine
{
    public class UISimulator :ISimulator
    {
        /// <summary>
        /// Allows adding additional ui renderers
        /// </summary>
        public List<ISimulator> SubSimulators = new List<ISimulator>();

        public UISimulator()
        {
            SubSimulators.Add(new TextareaUpdater());
        }

        public void Simulate()
        {
            SubSimulators.ForEach(s => s.Simulate());
        }
    }
}
