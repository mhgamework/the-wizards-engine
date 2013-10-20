using Castle.Windsor;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant.Installers;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    [EngineTest]
    public class RelativePositioningIntegrationTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private PrototypeObjectsFactory factory;
        private IGameObjectsRepository repository;

        public RelativePositioningIntegrationTest()
        {
            var container = new WindsorContainer();
            container.Install(new GameObjectsInstaller(),
                              new PrototypeInstaller(),
                              new EngineInstaller());
            factory = container.Resolve<PrototypeObjectsFactory>();
            repository = container.Resolve<IGameObjectsRepository>();

        }

        [Test]
        public void TestIslandWithTree()
        {
            var island = factory.CreateIsland();
            var tree = factory.CreateTree();

            var relIsland = repository.GetGameObject(island).GetComponent<IRelativePositionComponent>();
            var relTree = repository.GetGameObject(tree).GetComponent<IRelativePositionComponent>();

            relTree.Parent = relIsland;

            engine.AddSimulator(new BasicSimulator(delegate
                {
                    island.Physical.Position += new Vector3(1,0,0) * TW.Graphics.Elapsed;
                }));



        }

        [Test]
        public void TestIslandWithTreeAndCog()
        {
            var island = factory.CreateIsland();
            var tree = factory.CreateTree();
            var cog = factory.CreateCog();

            var relIsland = repository.GetGameObject(island).GetComponent<IRelativePositionComponent>();
            var relTree = repository.GetGameObject(tree).GetComponent<IRelativePositionComponent>();
            var reltCog = repository.GetGameObject(cog).GetComponent<IRelativePositionComponent>();

            relTree.Parent = relIsland;

            reltCog.Parent = relTree;

            engine.AddSimulator(new BasicSimulator(delegate
            {
                island.Physical.Position += new Vector3(1, 0, 0) * TW.Graphics.Elapsed;
            }));



        }

    }
}