using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MHGameWork.TheWizards.Rendering;
using SlimDX;


namespace MHGameWork.TheWizards.Particles
{
    public class EmitterParameters
    {
        public ITexture texture;
        public Vector3 position;
      
        public float particleWidth, particleHeight;
        public  IParticleCreater particleCreater;
        
        public float MaxLifeTime = 1.5f;//2.5
        private int particlesPerSecond=250;//250
        public float ParticleFrequency { get { return 1f / ParticlesPerSecond; } }
        public int ParticlesPerSecond
        {
            get { return particlesPerSecond; }
            set { if(value*MaxLifeTime<size*size){particlesPerSecond = value;} }
        }
        public int size = 64;

        public float particleWidthEnd = 2;
        public float particleHeightEnd = 2;
        public float darkScale = 0.6f;
        public Color4 startColor = new Color4(new Vector3(0,1, 0));
        public Color4 endColor = new Color4(new Vector3(1,0, 0));
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
