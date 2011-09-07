using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.WorldSimulation
{
    public  class Resource
    {
        public Vector3 Position = Vector3.Zero;
       

        private float decrementSpeed = 20f;
        private float incrementSpeed = 10;
        private float resourceLevel = 100;
        public ResourceTypes Type;//note attention for using resourcenames

        public float ResourceLevel
        {
            get { return resourceLevel; }
        }

        public float Radius;
        public bool IsInvinicity(Vector3 position)
        {
            if ((Position - position).Length() < Radius)
                return true;
            return false;
        }

        public float DecrementResource(float decrement)
        {
            resourceLevel -= decrement;
            if(resourceLevel<0)
            {
                float val = resourceLevel;
                resourceLevel = 0;
                return -val;
            }
            return 0;
        }

        
        public void IncrementResource(float elapsed)
        {
            resourceLevel += incrementSpeed * elapsed;
        }

        public static  Resource InResourceVicinity(Vector3 position, List<Resource> resources, ResourceTypes type)
        {
            for (int i = 0; i < resources.Count; i++)
            {
                if (resources[i].Type == type)
                    if (resources[i].IsInvinicity(position) && resources[i].ResourceLevel > 0)
                    {
                        return resources[i];

                    }
            }
            return null;
        }
    }
}
