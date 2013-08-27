using System;
using Castle.Windsor;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._Windsor;
using NUnit.Framework;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant
{
    [EngineTest]
    [TestFixture]
    public class PrototypeTest
    {
        [Test]
        public void TestPlayPrototype()
        {
            try
            {
                var container = BootstrapContainer();

                var prototype = container.Resolve<PrototypeTest>();
                prototype.Run();
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Init prototype");
            }

        }



        #region Injection
        [NonOptional]
        public TWEngine Engine { get; set; }
        [NonOptional]
        public PrototypeWorldGenerator PrototypeWorldGenerator { get; set; }
        #endregion

        public PrototypeTest()
        {

        }

        private void Run()
        {
            DI.Get<TestSceneBuilder>().Setup = setupTest;
            Engine.AddSimulator(new BasicSimulator(simulate));

            Engine.AddSimulator(new PhysicalSimulator());
            Engine.AddSimulator(new WorldRenderingSimulator());

            Engine.Run();
        }

        private void simulate()
        {
            foreach (var i in TW.Data.Objects.OfType<IslandPart>())
            {
                i.FixPhysical();
            }
        }

        private void setupTest()
        {
            PrototypeWorldGenerator.GenerateWorld(100);
        }


        public IWindsorContainer BootstrapContainer()
        {
            return new WindsorContainer()
                .Install(new PrototypeInstaller());
        }
    }
}