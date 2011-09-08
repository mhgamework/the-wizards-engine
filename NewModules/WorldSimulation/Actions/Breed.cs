using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.WorldSimulation.Priorities;

namespace MHGameWork.TheWizards.WorldSimulation.Actions
{
    public class Breed:IAction
    {
        private readonly Simulater sim;
        private Creature baby;
        //private float neededTime = 5f;
        //private float totalElapsed;
        private bool ok;
        public Breed(Building build, Creature creature, Simulater sim)
        {
            this.sim = sim;
            baby =
                new Creature(
                    (int)
                    ((build.Creature.Seeder.Seed + creature.Seeder.Seed)*0.5f + build.Creature.Seeder.NextInt(0, 5) +
                     creature.Seeder.NextInt(0, 5)));
            baby.Behaviour = creature.Behaviour.GetNewBehavior(baby);
            baby.Position = creature.Position;
            creature.SetProperty(ReProduction.TimeSinceLastFornication, 0);
            build.Creature.SetProperty(ReProduction.TimeSinceLastFornication, 0);
        }

        public void Apply(float elapsed, Creature creature)
        {
            //totalElapsed += elapsed;
            //totalElapsed = MathHelper.Clamp(totalElapsed, 0, neededTime);
            sim.AddCreature(baby);
            ok = true;
        }

        public void End()
        {
            
        }

        public bool Fullfilled()
        {
            return ok;
        }

        public void ForcedEnd()
        {
            if(!ok)
                sim.AddCreature(baby);
            ok = true;
        }
    }
}
