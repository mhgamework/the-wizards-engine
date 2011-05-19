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
           double a = seed.NextFloat(-MathHelper.Pi , MathHelper.Pi);
           double t = seed.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
           float rad = seed.NextFloat(0, 1);
           position = seed.NextVector3(new Vector3(-2, 0, -2), new Vector3(2,0, 2));
           position = new Vector3((float)Math.Cos(a), 0, (float)Math.Sin(a)) * seed.NextFloat(0, 0.5f);
           velocity = Vector3.Normalize(new Vector3((float)Math.Cos(a) * rad, rad, (float)Math.Sin(a)) * rad) * 1f;
       }
    
    }
}
