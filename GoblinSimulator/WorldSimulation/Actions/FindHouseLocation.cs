using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.WorldSimulation.Priorities;
using SlimDX;

namespace MHGameWork.TheWizards.WorldSimulation.Actions
{
   public class FindHouseLocation:IAction
    {
       public FindHouseLocation(Creature creature,Vector3 HousingLocation)
       {
          
       }
       public void Apply(float elapsed, Creature creature)
       {
           
       }

       public void End()
       {
           throw new NotImplementedException();
       }

       public bool Fullfilled()
       {
           throw new NotImplementedException();
       }

       public void ForcedEnd()
       {
           //not nececary
       }
    }
}
