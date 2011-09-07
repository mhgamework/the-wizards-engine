using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.WorldSimulation
{
    public class MoveToResource:IAction
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="type"></param>
       /// <param name="creature"></param>
       /// <param name="resources"></param>
       /// <param name="velocity"></param>
       /// <param name="resourceTreshold">If the resource is less then the treshold he will not go there even if this is the only resource</param>
        public MoveToResource(ResourceTypes type,Creature creature, List<Resource> resources, float velocity,float resourceTreshold)
        {
            
             moveTo = findClosedResource(type,resources, creature.Position,resourceTreshold);
             if (moveTo == null) return; //Invalid
            startPosition = creature.Position;
            neededTime = (moveTo.Position - startPosition).Length()/velocity;
           
        }

        private Resource findClosedResource(ResourceTypes type, List<Resource> resources, Vector3 position,float resourceTreshold)
        {
           Resource value=null;
            for (int i = 0; i < resources.Count; i++)
            {
                if(resources[i].Type==type)
                    if (value == null)
                    {
                        value = resources[i];
                    }
                    else
                    {
                        if ((value.Position - position).LengthSquared() > (resources[i].Position - position).LengthSquared()&&resources[i].ResourceLevel>resourceTreshold)
                        {
                            value =resources[i];
                        }
                    }
            }
            return value;
        }

        private Resource moveTo;
        private Vector3 startPosition;

      

        private float neededTime = 0;
        private float elapsedApplied;
        public void Apply(float elapsed, Creature creature)
        {
            elapsedApplied += elapsed;
            creature.Position = Vector3.Lerp(startPosition, moveTo.Position, elapsedApplied/neededTime);
        }

        public void End()
        {
            
        }
        /// <summary>
        /// note error when not applying first
        /// </summary>
        /// <returns></returns>
        public bool Fullfilled()
        {
            if (elapsedApplied >= neededTime)
                return true;
            return false;
        }

        public bool isValid()
        {
            if (moveTo == null)
                return false;
            return true;
        }
    }
}
