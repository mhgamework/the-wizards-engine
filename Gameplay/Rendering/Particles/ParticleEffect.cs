using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Particles;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Particles
{
    public class ParticleEffect
    {
        private List<CustomEmitter> emitters = new List<CustomEmitter>();
        private List<ColoredParticle> particles = new List<ColoredParticle>();

        public float CurrentTime { get; private set; }

        /// <summary>
        /// TODO: maybe use description?
        /// </summary>
        /// <param name="emitInterval"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public CustomEmitter CreateCustomEmitter(float emitInterval, Action<ColoredParticle> action)
        {
            var ret = new CustomEmitter(this);
            ret.EmitInterval = emitInterval;
            ret.InitializeParticle = action;
            emitters.Add(ret);
            return ret;
        }


        public void AddParticle(ColoredParticle particle)
        {
            particles.Add(particle);
        }

        public void Update(float elapsed)
        {
            CurrentTime += elapsed;
            emitters.ForEach(e => e.Update(elapsed));

            particles.RemoveAll(p => p.SpawnTime + p.Duration < CurrentTime);

        }

        public Vector3 CalculatePosition(ColoredParticle particle)
        {
            return particle.StartPosition + particle.StartVelocity * (CurrentTime - particle.SpawnTime);
        }

        public IEnumerable<ColoredParticle> Particles { get { return particles; } }

    }
}