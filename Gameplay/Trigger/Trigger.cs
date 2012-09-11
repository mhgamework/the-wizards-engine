using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Trigger
{
    public class Trigger
    {
        public List<ICondition> Conditions = new List<ICondition>();
        public List<IAction> Actions = new List<IAction>();

        private bool hasTriggered;

        public void Update()
        {
            if (hasTriggered) 
                return;

            foreach(ICondition c in Conditions)
            {
                if(!c.IsSatisfied())
                {
                    return;
                }
            }

            foreach(IAction a in Actions)
            {
                a.Activate();
            }

            hasTriggered = true;
        }
    }
}
