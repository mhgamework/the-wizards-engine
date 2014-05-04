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

        public SpellCastingEffectsTest(IRenderingTester r)
        {
            this.r = r;
        }

        public void TestBurstEffect()
        {
            var effect = new BurstSpellEffect(r.CreateParticleEffect());

            loopEffect(effect.Start, effect.Stop);
        }

        

        public void TestBeamEffect()
        {
            var effect = new BeamSpellEffect(r.CreateParticleEffect());

            loopEffect(effect.Start, effect.Stop);


        }

        public void TestImpactEffect()
        {
            var effect = new ImpactSpellEffect(r.CreateParticleEffect());

            loopEffect(effect.Start, effect.Stop);

        }

        private void loopEffect(Action startAction, Action endAction)
        {
            r.SetCameraPosition(new Vector3(0, 1, 2), new Vector3(0, 0, -3));

            r.SetRepeat(3, () =>
            {
                startAction();
                r.SetTimeout(2, endAction);
            });
        }
    }
}