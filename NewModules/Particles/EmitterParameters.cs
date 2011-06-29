using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Particles
{
    public class EmitterParameters
    {
        public ITexture texture;
        public Vector3 position;
      
        public float particleWidth, particleHeight;
        public  IParticleCreater particleCreater;
        
        public float MaxLifeTime = 2.5f;
        public int particlesPerSecond=250;
        public float ParticleFrequency { get { return 1f / ParticlesPerSecond; } }
        public int ParticlesPerSecond
        {
            get { return particlesPerSecond; }
            set { if(value*MaxLifeTime<size*size){particlesPerSecond = value;} }
        }
        public int size = 128;

        public float particleWidthEnd = 5;
        public float particleHeightEnd = 1;
        public float darkScale = 0.6f;
        public Color startColor = new Color(new Vector3(1, 0.4f, 0.4f));
        public Color endColor = new Color(new Vector3(0.4f, 0.2f, 0.2f));
        public Boolean Directional = false;
        public Vector2 UvStart = Vector2.Zero;
        public Vector2 UvSize = new Vector2(1, 1);
        public Boolean CreateParticles = true;
        public Boolean Continueous = true;
        public float CreationTime = 1;
        //out constructor
        public String EffectName = "calculateBall";


    }
}
