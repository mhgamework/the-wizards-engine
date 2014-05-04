using System;
using MHGameWork.TheWizards.Engine.Features.Testing;
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

        public SpellCastingEffectsTest(IRenderingTester r,GeometrySampler sampler)
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