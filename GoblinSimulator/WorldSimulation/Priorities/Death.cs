using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.WorldSimulation.Actions;

namespace MHGameWork.TheWizards.WorldSimulation.Priorities
{
    public class Death:IPriority
    {
        public void Apply(float elapsed, Creature creature, Simulater simulater)
        {
            
        }

        public IAction GetNextAction(Creature creature,  Simulater simulater)
        {
            return new Die(simulater);
        }
    }
}
