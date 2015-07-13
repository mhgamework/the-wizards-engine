using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.WorldSimulation
{
    public class Explore:IPriority
    {
        public static CreatureProperty<float> TimeExploredProperty = new CreatureProperty<float>("TimeExploredProperty");

       
        public void Apply(float elapsed, Creature creature, Simulater simulater)
        {
           

            creature.SetProperty(TimeExploredProperty, creature.GetProperty(Explore.TimeExploredProperty) + elapsed*10);
            // note velocity used 2 times
        }

        public IAction GetNextAction(Creature creature,Simulater simulater)
        {
             if (!(creature.CurrentAction is MoveToPosition))
            {
               
                Vector3 pos = creature.Seeder.NextVector3(new Vector3(-200, 0, -200).xna(), new Vector3(200, 0, 200).xna()).dx();

               
                return new MoveToPosition(creature, pos, 10f);
            }
            return creature.CurrentAction;
        }
    }
}
