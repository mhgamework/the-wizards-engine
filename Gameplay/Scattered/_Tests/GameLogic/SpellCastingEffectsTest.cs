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
            var effect = new BurstSpellEffect();

            r.SetCameraPosition( new Vector3(10, 0, 0), new Vector3());

            r.SetRepeat(3000, () =>
                {
                    effect.Start();
                    r.SetTimeout(2000, effect.Stop);
                });
        }
    }
}