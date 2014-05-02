using System;

namespace MHGameWork.TheWizards.Rendering.Particles
{
    /// <summary>
    /// TODO: this is quite loose concerning timings of emitting particles and their state
    /// </summary>
    public class CustomEmitter
    {
        private readonly ParticleEffect effect;

        public CustomEmitter(ParticleEffect effect)
        {
            this.effect = effect;
        }

        public float EmitInterval { get; set; }

        public Action<ColoredParticle> InitializeParticle { get; set; }

        private bool emitting = false;


        private float elapsedLeft = 0;
        public void Update(float elapsed)
        {
            if (!emitting) return;

            elapsedLeft += elapsed;

            while (elapsedLeft > EmitInterval)
            {
                elapsedLeft-= EmitInterval;
                //TODO: when emitting multiple correct for timing?
                var p = new ColoredParticle();
                InitializeParticle(p);
                p.SpawnTime = effect.CurrentTime;
                effect.AddParticle(p);
            }

        }

        public void Start()
        {
            emitting = true;
        }

        public void Stop()
        {
            emitting = false;
        }
    }
}