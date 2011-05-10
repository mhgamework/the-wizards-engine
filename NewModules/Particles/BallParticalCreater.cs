using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Particles
{
    public class BallParticleCreater:IParticleCreater
    {
        private readonly Emitter emmiter;
        private Seeder seed = new Seeder(123);
        public float Radius=1;// needs to be put in the shader!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

       public void GetNewParticleData(out Vector3 position, out Vector3 velocity)
       {
           double a=seed.NextFloat(0, MathHelper.TwoPi);
           double t=seed.NextFloat(0, MathHelper.TwoPi);
           
           position = new Vector3((float)(Radius*Math.Sin(a)*Math.Cos(t)), (float)(Radius*Math.Sin(t)*Math.Cos(a)), (float)(Radius*Math.Sin(t)));
           velocity = new Vector3((float)(-Radius * Math.Sin(t) * Math.Cos(a)), (float)(-Radius * Math.Sin(a) * Math.Cos(t)), (float)(Radius * Math.Cos(t)));
       }
    }
}
