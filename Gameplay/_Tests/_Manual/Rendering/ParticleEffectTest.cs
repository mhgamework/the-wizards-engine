using MHGameWork.TheWizards.Rendering.Particles;
using MHGameWork.TheWizards.Testing;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards._Tests._Manual.Rendering
{
    [TestFixture]
    public class ParticleEffectTest
    {
        private readonly IRenderingTester test;

        public ParticleEffectTest(IRenderingTester test)
        {
            this.test = test;
        }

        public void TestEmitter()
        {
            var effect = new ParticleEffect();

            test.ObserveUpdate(effect.Update);

            test.SetCameraPosition(new Vector3(10, 3, 0), new Vector3());

            var emitter = effect.CreateCustomEmitter(1 / 10f, p =>
            {
                p.Color = new Color4(1, 0, 0);
                p.Size = 0.1f;
                p.StartVelocity = new Vector3(0, 0, -5);
                p.Duration = 1;
            });

            emitter.Start();

            var renderer = new ParticlesLineRenderer(test.LineManager3D);

            test.ObserveUpdate(() => renderer.RenderEffect(effect));

        }
    }
}