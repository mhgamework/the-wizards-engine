using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.WorldSimulation
{
    public class MoveToPosition:IAction
    {
        
        private Vector3 startPos;
        private Vector3 endPos;
        private float totalElapsed;
        private float neededTime;
        
        public  MoveToPosition(Creature creature, Vector3 position,float velocity)
        {
            startPos = creature.Position;
            endPos = position; 
            neededTime = (endPos - startPos).Length() / velocity;
        }

        public void Apply(float elapsed, Creature creature)
        {
            totalElapsed += elapsed;
            creature.Position = Vector3.Lerp(startPos, endPos, totalElapsed / neededTime);
        }

        public void End()
        {
            
        }

        public bool Fullfilled()//note error when not applied first
        {
            if (totalElapsed >= neededTime)
                return true;
            else
                return false;
        }
    }
}
