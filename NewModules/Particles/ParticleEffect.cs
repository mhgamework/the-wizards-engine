using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Particles
{
    public class ParticleEffect
    {

        public Vector3 Position;
        public Vector3 Normal;
        private List<EmitterParameters> emitterParameters = new List<EmitterParameters>();
        private List<Emitter> emitters = new List<Emitter>();
        private IXNAGame game;
        private VertexDeclarationPool pool;
        private readonly TexturePool texPool;
        private bool initialized=false;
        bool running = false;
        private float timeSinceStarted;
        public float playtime = 2f;
        public ParticleEffect(IXNAGame game,VertexDeclarationPool pool,TexturePool texPool)
        {
            this.game= game;
            this.pool = pool;
            this.texPool = texPool;
        }
        public void Initialize()
       {
           initialized = true;
           for (int i = 0; i < emitters.Count; i++)
           {
               emitters[i].Initialize();
               emitters[i].InitializeRender();
               emitters[i].CreateRenderData();
               emitters[i].SetRenderData();
           }
       }
        public void Trigger()
        {
            running = true;
            for (int i = 0; i < emitters.Count; i++)
            {
                emitters[i].TimeSinceStart = 0;
                emitters[i].Reset();
            }
            timeSinceStarted = 0;
        }
        public void Update()
        {
            
            if(timeSinceStarted>playtime)
            {
                running = false;
               
            }
            if (running)
            {
                timeSinceStarted += game.Elapsed;
                for (int i = 0; i < emitters.Count; i++)
                {
                    emitters[i].Update();
                }
            }
            
        }
        public void Render(Matrix viewProjection,Matrix viewInverse)
        {
            if (running)
            {
                for (int i = 0; i < emitters.Count; i++)
                {
                    emitters[i].Render(viewProjection, viewInverse);
                }
            }
        }
        public void AddEmitter(EmitterParameters emitterParameters)
        {
            this.emitterParameters.Add(emitterParameters);
            Emitter emit = new Emitter(texPool, pool, game, emitterParameters);
            if (initialized)
            {
                emit.Initialize();
                emit.InitializeRender();
                emit.CreateRenderData();
                emit.SetRenderData();
            }

            emitters.Add(emit);

        }
    }
}
