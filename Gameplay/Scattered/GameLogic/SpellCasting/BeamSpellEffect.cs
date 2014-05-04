using System;
using DirectX11;
using MHGameWork.TheWizards.Rendering.Particles;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.GameLogic.SpellCasting
{
    /// <summary>
    /// Simulates a burst spell effect
    /// </summary>
    public class BeamSpellEffect
    {
        private readonly ParticleEffect effect;
        private readonly GeometrySampler sampler;
        private CustomEmitter emitter;
        private Seeder seeder = new Seeder(0);

        public BeamSpellEffect(ParticleEffect effect,GeometrySampler sampler)
        {
            this.effect = effect;
            this.sampler = sampler;

            emitter = effect.CreateCustomEmitter(1 / 100f, p =>
                {
                    p.StartPosition = seeder.NextFloat(0, 0.3f) * new Vector3(sampler.RandomPointOnCircle(), 0);
                    p.Color = new Color4(1, 0, 0);
                    p.Size = 0.1f;
                    p.StartVelocity = new Vector3(0, 0, seeder.NextFloat(-1, -1)) * 50;
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