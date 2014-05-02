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

            r.SetCameraPosition(new Vector3(0, 1, 2), new Vector3(0, 0, -3));

            r.SetRepeat(3, () =>
                {
                    effect.Start();
                    r.SetTimeout(2, effect.Stop);
                });

        }

        public void TestBeamEffect()
        {
            var effect = new BeamSpellEffect(r.CreateParticleEffect());

            r.SetCameraPosition(new Vector3(0, 1, 2), new Vector3(0, 0, -3));

            r.SetRepeat(3, () =>
            {
                effect.Start();
                r.SetTimeout(2, effect.Stop);
            });

        }
    }
}