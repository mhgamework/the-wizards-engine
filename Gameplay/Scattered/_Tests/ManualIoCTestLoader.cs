using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Rendering.Particles;
using MHGameWork.TheWizards.Scattered._Tests.GameLogic;
using MHGameWork.TheWizards.Simulation.ActionScheduling;
using MHGameWork.TheWizards.Testing;
using MHGameWork.TheWizards._Tests._Manual.Rendering;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    /// <summary>
    /// Current manual usage of the IoC test loader. This should be integrated into the construction of testfixtures using IoC.
    /// Manual usage continues until IoC test loading proves usefull
    /// </summary>
    [TestFixture]
    [EngineTest]
    public class ManualIoCTestLoader
    {
        public ManualIoCTestLoader()
        {

        }

        [Test]
        public void Run()
        {
            var r = new IRenderingTester(EngineFactory.CreateEngine(), new IActionScheduler(), new ParticlesBoxRenderer());

            var test = new SpellCastingEffectsTest(r);
            //test.TestBurstEffect();
            test.TestBeamEffect();

            //var test = new ParticleEffectTest(r);
            //test.TestEmitter();
            //test.TestBoxRenderer();
        }
    }
}