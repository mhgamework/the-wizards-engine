using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Particles
{
    public class SparkelParticleCreater : IParticleCreater
    {
        private Seeder seed = new Seeder(123);
        private float speed = 0.1f;
        public void GetNewParticleData(out Vector3 position, out Vector3 velocity)
        {
            position = Vector3.Zero;
            velocity =seed.NextVector3(new Vector3(-1, -1, 0), new Vector3(1, 1, 1));
            velocity.Normalize();
            velocity*= speed*50;
        }
    }
}
