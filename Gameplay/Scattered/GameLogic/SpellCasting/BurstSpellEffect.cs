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

        public BurstSpellEffect(ParticleEffect effect)
        {
            this.effect = effect;

            emitter = effect.CreateCustomEmitter(1 / 10f, p =>
                {
                    p.Color = new Color4(1, 0, 0);
                    p.Size = 0.1f;
                    p.StartVelocity = new Vector3(0, 0, -1);
                    p.Duration = 0.5f;
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