using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;

namespace MHGameWork.TheWizards.WorldSimulation.Actions
{
    public class GatherResource:IAction
    {
        private float resourcelevelStart;
        private readonly Creature creature;
        private float resourcelevelEnd;
        private readonly Resource resource;
        private readonly ResourceTypes type;
        private float neededTime;
        private float totalElapsed;
        public GatherResource(Creature creature, float resourcelevelEnd, float gatherSpeed, IBellyFillable belly, Resource resource,ResourceTypes type)
           {
               if (type == ResourceTypes.Food)
                   resourcelevelStart = belly.FoodLevel;
               else
                   resourcelevelStart = creature.GetResource(type);
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
    }
}
