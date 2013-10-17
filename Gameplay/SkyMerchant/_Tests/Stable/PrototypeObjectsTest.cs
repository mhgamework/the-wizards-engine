using System;
using Castle.Core;
using Castle.Windsor;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Diagnostics.Tracing;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Installers;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    /// <summary>
    /// Tests the gameplay objects in the prototype
    /// </summary>
    [EngineTest]
    [TestFixture]
    public class PrototypeObjectsTest
    {
        private PrototypeObjectsFactory objectsFactory;

        public PrototypeObjectsTest()
        {
            var engine = EngineFactory.CreateEngine();

            var container = new WindsorContainer();
            container.Install(
                new GameObjectsInstaller(),
                new PrototypeInstaller(),
                new WorldingInstaller(),
                new EngineInstaller()
                );

            container.Kernel.ComponentCreated += delegate(ComponentModel model, object instance)
                {
                    Console.WriteLine("Created {0}", model.Name);
                };

            objectsFactory = container.Resolve<PrototypeObjectsFactory>();

            engine.AddSimulator(new SkyMerchantRenderingSimulator());
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestIsland()
        {
            objectsFactory.CreateIsland();
        }
        [Test]
        public void TestTrader()
        {
            objectsFactory.CreateTrader();
        }

        [Test]
        public void TestCogItem()
        {
            objectsFactory.CreateCog();
        }

        [Test]
        public void TestTree()
        {
            var tree = objectsFactory.CreateTree();
            tree.SimulateGeneration();
        }
        [Test]
        public void TestDrone()
        {
            objectsFactory.CreateDrone();
        }

        [Test]
        public void TestPirate()
        {
            objectsFactory.CreatePirate();
        }

        [Test]
        public void TestMultipleIslands()
        {
            var island = objectsFactory.CreateIsland();
            island.Physical.Position = new Vector3(20, 0, 20);
            island = objectsFactory.CreateIsland();
            island.Physical.Position = new Vector3(0, 0, 20);
            island = objectsFactory.CreateIsland();
            island.Physical.Position = new Vector3(-20, 0, 20);
        }


    }
}