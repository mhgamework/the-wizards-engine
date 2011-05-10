using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Particles
{
    public class SimpleParticleCreater:IParticleCreater
    {
        private Seeder seed = new Seeder(123);
        public float Radius = 1;

        public void GetNewParticleData(out Vector3 position, out Vector3 velocity)
        {
            

            position = seed.NextVector3(new Vector3(0,0,0),new Vector3(50,50,50));
            velocity = seed.NextVector3(new Vector3(0, 0, 0), new Vector3(2, 2, 2)); 
        }
    }
}
