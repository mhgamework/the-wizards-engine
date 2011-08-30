using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;


namespace MHGameWork.TheWizards.Particles
{
    public class SimpleParticleCreater:IParticleCreater
    {
        private Seeder seed = new Seeder(123);
        public float Radius = 1;

        public void GetNewParticleData(out Vector3 position, out Vector3 velocity)
        {


            position = seed.NextVector3(new Vector3(0, 0, 0).xna(), new Vector3(5, 5, 5).xna()).dx();
            velocity = seed.NextVector3(new Vector3(0, 0, 0).xna(), new Vector3(2, 2, 2).xna()).dx(); 
        }
    }
}
