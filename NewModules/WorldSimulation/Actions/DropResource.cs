using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;

namespace MHGameWork.TheWizards.WorldSimulation.Actions
{
   public class DropResource:IAction
    {
       
        private float resourcelevelStart;
        private readonly Creature creature;
        private float resourcelevelEnd;
        private readonly Resource resource;
        private readonly ResourceTypes type;
        private float neededTime;
        private float totalElapsed;
       /// <summary>
       /// not operational yet note: drop everywhere? only in chests
       /// </summary>
       /// <param name="creature"></param>
       /// <param name="resourcelevelEnd"></param>
       /// <param name="gatherSpeed"></param>
       /// <param name="belly"></param>
       /// <param name="resource"></param>
       /// <param name="type"></param>
        public DropResource(Creature creature, float resourcelevelEnd, float gatherSpeed, IBellyFillable belly, Resource resource,ResourceTypes type)
           {
               resourcelevelStart = belly.FoodLevel;
            this.creature = creature;
            this.resourcelevelEnd = resourcelevelEnd;
               this.resource = resource;
            this.type = type;
            this.resourcelevelEnd-=resource.DecrementResource(this.resourcelevelEnd - resourcelevelStart);
               neededTime = (this.resourcelevelEnd - resourcelevelStart) / gatherSpeed;
              
           }

        public void Apply(float elapsed, Creature creature)
        {
            totalElapsed += elapsed;
            totalElapsed = MathHelper.Clamp(totalElapsed, 0, neededTime);
           
            creature.SetResource(type, MathHelper.Lerp(resourcelevelStart, resourcelevelEnd, totalElapsed/neededTime));
            //resource.DecrementResource(((foodlevelEnd - foodlevelStart)*elapsed)/neededTime);
        }

        public void End()
        {
            
        }

        public bool Fullfilled()// note possible error when not applying before caling fullfilled
        {
            if (totalElapsed >= neededTime)
                return true;
            return false;
        }

       public void ForcedEnd()
       {
           //not nececary here
       }
    }
}
