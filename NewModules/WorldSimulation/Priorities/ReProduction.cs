using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.WorldSimulation.Priorities
{
    public class ReProduction:IPriority
    {
        public ReProduction()
        {
            
        }
        public void Apply(float elapsed, Creature creature, Simulater simulater)
        {
            throw new NotImplementedException();
        }

        public IAction GetNextAction(Creature creature, Simulater simulater)
        {
            if ((creature.CurrentAction is MoveToPosition ) && creature.CurrentPriority == this)
                return creature.CurrentAction;
            for (int i = 0; i < simulater.Buildings.Count; i++)
            {
                
            }

        }
    }
}
