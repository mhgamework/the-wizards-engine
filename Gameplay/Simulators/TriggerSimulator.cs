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
            for (int i = 0; i < triggers.Count; i++)
            {
                Trigger.Trigger t = triggers[i];
                t.Update();
            }
        }

        public void AddTrigger(Trigger.Trigger t)
        {
            triggers.Add(t);
        }

        public void RemoveTrigger(Trigger.Trigger t)
        {
            triggers.Remove(t);
        }

        public bool ContainsTrigger(Trigger.Trigger t)
        {
            return triggers.Contains(t);
        }
    }
}
