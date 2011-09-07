using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.WorldSimulation
{
    public class FillBelly:IPriority
    {
        
        private readonly IBellyFillable belly;

        public FillBelly(IBellyFillable belly)
        {
            this.belly = belly;
        }


        public void Apply(float elapsed, Creature creature, Simulater simulater)
        {
           
        }

        public IAction GetNextAction(Creature creature, Simulater simulater)
        {
            if ((creature.CurrentAction is MoveToResource || creature.CurrentAction is Eat) && creature.CurrentPriority == this )
                return creature.CurrentAction;

            IAction act = null;
            for (int i = 0; i < simulater.Resources.Count; i++)
            {
                if (simulater.Resources[i].Type == ResourceTypes.Food)
                    if (simulater.Resources[i].IsInvinicity(creature.Position) && simulater.Resources[i].ResourceLevel > 0)
                    {
                        return new Eat(creature, 100, 10, belly, simulater.Resources[i]);
                       
                    }
            }

            var mtrAct = new MoveToResource(ResourceTypes.Food, creature, simulater.Resources, 10f,5);
            if (!mtrAct.isValid())
              return null;//note velocity used
            return mtrAct;
        }

        public void SetFoodLevel(float level)
        {
            
        }
    }
}
