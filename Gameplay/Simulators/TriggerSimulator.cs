using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;

namespace MHGameWork.TheWizards.Simulators
{
    public class TriggerSimulator : ISimulator
    {
        private List<Trigger.Trigger> triggers = new List<Trigger.Trigger>();

        public void Simulate()
        {
            foreach(Trigger.Trigger t in triggers)
            {
                t.Update();
            }
        }

        public void AddTrigger(Trigger.Trigger t)
        {
            triggers.Add(t);
        }
    }
}
