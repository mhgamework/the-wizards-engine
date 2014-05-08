using System;
using DirectX11;
using MHGameWork.TheWizards.Rendering.Particles;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.GameLogic.SpellCasting
{
    /// <summary>
    /// Simulates a burst spell effect
    /// Idea, add normal direction for which to erupt particles
    /// </summary>
    public class ImpactSpellEffect
    {
        private readonly ParticleEffect effect;
        private readonly GeometrySampler sampler;
        private CustomEmitter emitter;
        private Seeder seeder = new Seeder(0);

        public ImpactSpellEffect(ParticleEffect effect, GeometrySampler sampler)
        {
            this.effect = effect;
            this.sampler = sampler;

            effect.Gravity = new Vector3(0, -10, 0);

            /*emitter = effect.CreateCustomEmitter(1 / 100f, p =>
                {
                    p.StartPosition = new Vector3(0, 0, 0);
                    p.Color = new Color4(1, 0, 0);
                    p.Size = 0.1f;
                    p.StartVelocity = sampler.RandomPointOnSphere()*5;
                    p.Duration = 3f;
                });*/

        }

        public Vector3 Position { get; set; }

        public void Start()
        {
            //emitter.Start();
            for (int i = 0; i < 50; i++)
            {
                var p = new ColoredParticle();
                p.StartPosition = new Vector3(0, 0, 0);
                p.Color = new Color4(1, 0, 0);
                p.Size = 0.1f;
                p.StartVelocity = (sampler.RandomPointOnSphere() * 6).Alter(v => v.ChangeY((float)Math.Abs(v.Y)));

                p.Duration = 1f;
                p.SpawnTime = effect.CurrentTime;


                effect.AddParticle(p);
            }
        }

        public void Stop()
        {
            //emitter.Stop();
        }
    }
}