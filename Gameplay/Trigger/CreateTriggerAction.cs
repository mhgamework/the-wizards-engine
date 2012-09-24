using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Simulators;

namespace MHGameWork.TheWizards.Trigger
{
    public class CreateTriggerAction : IAction
    {
        private TriggerSimulator sim;
        private Trigger trigger;

        public void Setup(ICondition condition, IAction action, bool orTrigger, bool invertTirgger, int triggerType, TriggerSimulator sim)
        {
            List<ICondition> conds = new List<ICondition>();
            List<IAction> actions = new List<IAction>();

            conds.Add(condition);
            actions.Add(action);

            Setup(conds, actions, orTrigger, invertTirgger, triggerType, sim);
        }

        public void Setup(List<ICondition> conditions, List<IAction> actions, bool orTrigger, bool invertTrigger, int triggerType, TriggerSimulator sim)
        {
            if (actions.Contains(this))
                throw new Exception("Infinite action-loop created!");

            this.sim = sim;
            trigger = new Trigger();
            trigger.Conditions = conditions;
            trigger.Actions = actions;
            trigger.SetAndOr(orTrigger);
            trigger.SetType(triggerType);

            if (invertTrigger)
                trigger.Invert();
        }

        public void Activate()
        {
            if (!sim.ContainsTrigger(trigger))
                sim.AddTrigger(trigger);
        }

        public void Reset()
        {
            if (sim.ContainsTrigger(trigger))
                sim.RemoveTrigger(trigger);
        }
    }
}
