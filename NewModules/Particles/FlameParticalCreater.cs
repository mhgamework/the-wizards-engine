using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Particles
{
    public class FlameParticelCreater : IParticleCreater
    {
        private readonly Emitter emmiter;
        private Seeder seed = new Seeder(123);

        public void GetNewParticleData(out Vector3 position, out Vector3 velocity)
        {

            float rad = seed.NextFloat(0, 1);
            position = randomDirection() * seed.NextFloat(0, 1f);
            position.Y = 0;
            //position = new Vector3((float)(Math.Cos(a) * Math.Cos(t)), (float)Math.Sin(t), (float)(Math.Sin(a) * Math.Cos(t)));

            velocity = randomDirection() * 1f;
            velocity.Y = (float) Math.Abs(velocity.Y);
        }

        private Vector3 randomDirection()
        {
            //FROM: http://mathworld.wolfram.com/SpherePointPicking.html

            float u = seed.NextFloat(-1, 1);
            float theta = seed.NextFloat(0, MathHelper.TwoPi);

            float x = (float)(Math.Sqrt(1 - u * u) * Math.Cos(theta));
            float y = (float)(Math.Sqrt(1 - u * u) * Math.Sin(theta));
            float z = u;


            return new Vector3(x, y, z);
        }
    }
}
