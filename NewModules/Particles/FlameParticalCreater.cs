using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Particles
{
   public class FlameParticelCreater:IParticleCreater
    {
        private readonly Emitter emmiter;
        private Seeder seed = new Seeder(123);

       public void GetNewParticleData(out Vector3 position, out Vector3 velocity)
       {
           double a = seed.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
           double t = seed.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);

           position = seed.NextVector3(new Vector3(-2, 0, -2), new Vector3(2, 0, 2));
           velocity = new Vector3((float)Math.Cos(a), 1, (float)Math.Sin(t))*5f;
       }
    
    }
}
