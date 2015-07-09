using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;

namespace MHGameWork.TheWizards.WorldSimulation
{
    public class Eat:IAction
    {
        private float foodlevelStart;
        private float foodlevelEnd;
        private readonly IBellyFillable belly;
        private readonly Resource resource;
        private float neededTime;
        private float totalElapsed;
           public Eat(Creature creature, float _foodlevelEnd,float eatSpeed,IBellyFillable belly,Resource resource)
           {
               foodlevelStart = belly.FoodLevel;
               this.foodlevelEnd = _foodlevelEnd;
               this.belly = belly;
               this.resource = resource;
               this.foodlevelEnd-=resource.DecrementResource(foodlevelEnd - foodlevelStart);
               neededTime = (foodlevelEnd - foodlevelStart) / eatSpeed;
              
           }

        public void Apply(float elapsed, Creature creature)
        {
            totalElapsed += elapsed;
            totalElapsed = MathHelper.Clamp(totalElapsed, 0, neededTime);
           
            belly.FoodLevel = MathHelper.Lerp(foodlevelStart, foodlevelEnd, totalElapsed/neededTime);
            //resource.DecrementResource(((foodlevelEnd - foodlevelStart)*elapsed)/neededTime);
        }

        public void End()
        {

            belly.FoodLevel = foodlevelEnd;
        }

        public bool Fullfilled()// note possible error when not applying before caling fullfilled
        {
            if (totalElapsed >= neededTime)
                return true;
            return false;
        }

        public void ForcedEnd()
        {
            belly.FoodLevel = MathHelper.Lerp(foodlevelStart, foodlevelEnd, totalElapsed / neededTime);
            //note: fix return the not needed food to the resource here
        }

        public bool isValid()
        {
            if (neededTime < 0.001f)
                return false;
            return true;
        }
    }
}
