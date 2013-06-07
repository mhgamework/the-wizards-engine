using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Pickupping;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    /// <summary>
    /// The in the wiki explained 'bigtest'. This is a sandbox version of the game test!
    /// </summary>
    [EngineTest]
    [TestFixture]
    public class RTSTestCase1Test
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        /// <summary>
        /// This allows displaying errors when testing, due to bug in the NUnitTestRunner not using the iErrorLogger
        /// </summary>
        [Test]
        public void TestSandboxCatch()
        {
            try
            {
                TestSandbox();
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex,"Test Setup");
            }   
        }

        [Test]
        public void TestSandbox()
        {
            var c = new WindsorContainer();
            // Register all simulators first: these should be used as primary dependencies!?
            c.Register(
                Classes.FromThisAssembly()
                // doesnt work for some reason? .InNamespace("MHGamework.TheWizards.RTSTestCase1")
                        .Where(o => typeof(ISimulator).IsAssignableFrom(o))
                        .WithServiceAllInterfaces().WithServiceSelf()
                        );
            c.Register(
                Classes.FromThisAssembly()
                       .Pick()
                       .WithServiceDefaultInterfaces());

            engine.AddSimulator(c.Resolve<UserPlayerSimulator>());
            engine.AddSimulator(c.Resolve<PlayerCameraSimulator>());
            engine.AddSimulator(c.Resolve<PhysicalSimulator>());
            engine.AddSimulator(c.Resolve<WorldRenderingSimulator>());
        }

    }
}