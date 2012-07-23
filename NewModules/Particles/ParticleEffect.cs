using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;


namespace MHGameWork.TheWizards.Particles
{
    public class ParticleEffect
    {

        public Vector3 Position;
        public Vector3 Normal;
        private List<EmitterParameters> emitterParameters = new List<EmitterParameters>();
        private List<Emitter> emitters = new List<Emitter>();
        private DX11Game game;
        //private VertexDeclarationPool pool;
        private readonly TexturePool texPool;
        private bool initialized = false;
        bool running = false;
        private float timeSinceStarted;
        public float playtime = 2f;
        public ParticleEffect(DX11Game game, TexturePool texPool)
        {
            this.game = game;
            //this.pool = pool;
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
                emitters[i].Reset();
            }
            timeSinceStarted = 0;
        }
        public void Update()
        {
            if (timeSinceStarted > playtime)
            {
                running = false;
            }

            if (!running) return;

            timeSinceStarted += game.Elapsed;
            for (int i = 0; i < emitters.Count; i++)
            {
                emitters[i].Update();
            }
        }
        public void Render(Matrix viewProjection, Matrix viewInverse)
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
            Emitter emit = new Emitter(texPool, game, emitterParameters,800,600);//note: screenspace
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
