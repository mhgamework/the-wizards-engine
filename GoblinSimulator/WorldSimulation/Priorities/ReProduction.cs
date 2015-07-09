using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.WorldSimulation.Actions;
using SlimDX;

namespace MHGameWork.TheWizards.WorldSimulation.Priorities
{
    public class ReProduction:IPriority
    {
        public static CreatureProperty<float> TimeSinceLastFornication = new CreatureProperty<float>("TimeSinceLastFornication");

        public void Apply(float elapsed, Creature creature, Simulater simulater)
        {
            //nothing needs to be applied
        }

        public IAction GetNextAction(Creature creature, Simulater simulater)
        {
            if ((creature.CurrentAction is MoveToPosition || creature.CurrentAction is Breed ) && creature.CurrentPriority == this)
                return creature.CurrentAction;
            if (simulater.Buildings.Count < 0)
                return null;
            Building build=simulater.Buildings[0];
            

            for (int i = 0; i < simulater.Buildings.Count; i++)
            {
                var building = simulater.Buildings[i];
                if (building.Home || building.Creature == creature)
                    if ((building.Position - creature.Position).LengthSquared() < (build.Position - creature.Position).LengthSquared())
                        build = building;
            }
            if ((build.Position - creature.Position).Length() < 5)//note building size or some kind of closeness
            {  if(build.Home && build.Creature!=creature)
                {
                    return new Breed(build, creature, simulater);                
                }
                else
                {
                    return new AtHome(creature, build,50);
                }}
            return new MoveToPosition(creature, build.Position, 10f);//note: velocity used again
        }
    }
}
