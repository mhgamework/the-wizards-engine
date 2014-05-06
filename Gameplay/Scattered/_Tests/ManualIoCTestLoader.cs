using System;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Rendering.Particles;
using MHGameWork.TheWizards.Scattered._Tests.GameLogic;
using MHGameWork.TheWizards.Simulation.ActionScheduling;
using MHGameWork.TheWizards.Testing;
using MHGameWork.TheWizards._Tests._Manual.Math;
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
        private IActionScheduler iActionScheduler = new IActionScheduler();
        private Random random = new Random();
        private Seeder seeder = new Seeder(0);
        private GeometrySampler sampler;
        private IRenderingTester renderingTester;

        public ManualIoCTestLoader()
        {
            sampler = new GeometrySampler(seeder);
            renderingTester = new IRenderingTester(EngineFactory.CreateEngine(), new IActionScheduler(), new ParticlesBoxRenderer());
        }

        [Test]
        public void Run()
        {

            var test = new SpellCastingEffectsTest(renderingTester, sampler);
            //test.TestBurstEffect();
            //test.TestBeamEffect();
            //test.TestImpactEffect();
            test.TestShieldEffect();

            //var test = new ParticleEffectTest(r);
            //test.TestEmitter();
            //test.TestBoxRenderer();

            //var test = new GeometrySamplerTest(renderingTester, seeder);
            //test.TestCircle();
            //test.TestSphere();
        }
    }
}