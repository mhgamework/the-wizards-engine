using System;
using MHGameWork.TheWizards.Rendering.Particles;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.GameLogic.SpellCasting
{
    /// <summary>
    /// Simulates a burst spell effect
    /// </summary>
    public class BurstSpellEffect
    {
        private readonly ParticleEffect effect;
        private CustomEmitter emitter;
        private Seeder seeder = new Seeder(0);
        private GeometrySampler sampler;

        public BurstSpellEffect(ParticleEffect effect)
        {
            this.effect = effect;

            sampler = new GeometrySampler(seeder);

            emitter = effect.CreateCustomEmitter(1 / 100f, p =>
                {
                    p.Color = new Color4(1, 0, 0);
                    p.Size = 0.1f;
                    p.StartVelocity = new Vector3(seeder.NextFloat(-1, 1) * 2, seeder.NextFloat(-1, 1) * 0.7f, seeder.NextFloat(-4, -5));
                    p.Duration = 1f;
                });

        }

        public void Start()
        {
            emitter.Start();
        }

        public void Stop()
        {
            emitter.Stop();
        }
    }
}