using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.WorldSimulation.Actions
{
   public class Die:IAction
    {
       
       private readonly Simulater sim;
       bool ok = false;
       public Die( Simulater sim)
       {
           this.sim = sim;
           
       }
       
       public void Apply(float elapsed, Creature creature)
       {
           creature.Alive = false;
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
           //not nececary here
       }
    }
}
