using System;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Particles;
using MHGameWork.TheWizards.Scattered.GameLogic.SpellCasting;
using MHGameWork.TheWizards.Testing;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Tests.GameLogic
{
    [EngineTest]
    [TestFixture]
    public class SpellCastingEffectsTest
    {
        private readonly IRenderingTester r;
        private readonly GeometrySampler sampler;

        public SpellCastingEffectsTest(IRenderingTester r, GeometrySampler sampler)
        {
            this.r = r;
            this.sampler = sampler;
        }

        public void TestBurstEffect()
        {
            r.SetCameraPosition(new Vector3(0, 1, 2), new Vector3(0, 0, -3));

            var effect = new BurstSpellEffect(r.CreateParticleEffect());

            loopEffect(effect.Start, effect.Stop);
        }



        public void TestBeamEffect()
        {
            r.SetCameraPosition(new Vector3(0, 1, 2), new Vector3(0, 0, -3));

            var effect = new BeamSpellEffect(r.CreateParticleEffect(), sampler);

            loopEffect(effect.Start, effect.Stop);


        }

        public void TestImpactEffect()
        {

            r.SetCameraPosition(new Vector3(0, 0, 5), new Vector3(0, 0, 0));

            var effect = new ImpactSpellEffect(r.CreateParticleEffect(), sampler);

            loopEffect(effect.Start, effect.Stop);

        }

        public void TestShieldEffect()
        {

            r.SetCameraPosition(new Vector3(0, 0, 5), new Vector3(0, 0, 0));

            var playbackSpeed = 0.3f;

            var effect = r.CreateParticleEffect();

            var emitter = effect.CreateCustomEmitter(1 / 300f / playbackSpeed, p =>
                {
                    p.StartPosition = new Vector3(sampler.RandomPointOnCircle(), 0) * 0.1f;
                    p.StartVelocity = new Vector3(sampler.RandomPointOnCircle(), 0);
                    p.Size = 0.05f;
                    p.Duration = 1 / 3f / playbackSpeed;
                });

            effect.CalculatePosition = p =>
                {
                    var lifeTime = (effect.CurrentTime - p.SpawnTime) * 3 * playbackSpeed;

                    var magicNumber = 0.4f;

                    var startAngle = p.StartVelocity.X * 1000;
                    var endAngle = p.StartVelocity.Y * 1000 % magicNumber + (1 - magicNumber);

                    var ret = p.StartPosition;

                    ret.X += (float)Math.Pow(lifeTime, 0.8f) * (float)Math.Cos(Math.Sqrt(lifeTime * 10 * endAngle) + startAngle);
                    ret.Y += (float)Math.Pow(lifeTime, 0.8f) * (float)Math.Sin(Math.Sqrt(lifeTime * 10 * endAngle) + startAngle);
                    ret.Z += ret.TakeXY().Length() * 0.5f;

                    return ret;
                };



            loopEffect(() => { emitter.Start(); }, () =>
                {
                    emitter.Stop();
                });

        }

        private void loopEffect(Action startAction, Action endAction)
        {

            r.SetRepeat(3, () =>
            {
                startAction();
                r.SetTimeout(2, endAction);
            });
        }
    }
}