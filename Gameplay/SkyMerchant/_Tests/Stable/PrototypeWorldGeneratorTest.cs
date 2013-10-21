using Castle.Windsor;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Installers;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using NUnit.Framework;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Stable
{
    [EngineTest]
    [TestFixture]
    public class PrototypeWorldGeneratorTest
    {
        private TWEngine engine;
        private PrototypeWorldGenerator generator;

        public PrototypeWorldGeneratorTest()
        {
            engine = EngineFactory.CreateEngine();

            var container = BootstrapContainer();
            engine.AddSimulator(new SkyMerchantRenderingSimulator());
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());


            generator = container.Resolve<PrototypeWorldGenerator>();
        }
        [Test]
        public void TestWorldGenerator()
        {
            generator.GenerateWorld(150);
           
        }


        public IWindsorContainer BootstrapContainer()
        {
            return new WindsorContainer()
                .Install(new GameObjectsInstaller())
                .Install(new PrototypeInstaller())
                .Install(new EngineInstaller())
                .Install(new WorldingInstaller())
                ;
        }
    }
}