using Castle.Windsor;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.GameObjects;
using MHGameWork.TheWizards.SkyMerchant.Installers;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    /// <summary>
    /// Tests the integration of the relative position code in the gameplayinterfaces layer.
    /// (Test code only references the gameplay interfaces.)
    /// </summary>
    [TestFixture]
    [EngineTest]
    public class RelativePositioningIntegrationTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private IGameObjectsRepository repository;
        private RelativePositioningSimulator relativePositioningSimulator;

        public RelativePositioningIntegrationTest()
        {
            var container = new WindsorContainer();
            container.Install(new GameObjectsInstaller(),
                              new SimulatorsInstaller(),
                              new EngineInstaller());
            
            repository = container.Resolve<IGameObjectsRepository>();
            relativePositioningSimulator = container.Resolve<RelativePositioningSimulator>();

            engine.AddSimulator(relativePositioningSimulator);
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestIslandWithTree()
        {
            var island = createTestObject();
            var tree = createTestObject();

            var relIsland = island.GetComponent<IRelativePositionComponent>();
            var relTree = tree.GetComponent<IRelativePositionComponent>();

            relTree.Parent = relIsland;
            relTree.RelativePosition = new Vector3(1, 1, 0);

            engine.AddSimulator(new BasicSimulator(delegate
                {
                    relIsland.Position += new Vector3(0.1f, 0, 0) * TW.Graphics.Elapsed;
                    relIsland.Rotation *= Quaternion.RotationAxis(Vector3.UnitY, TW.Graphics.Elapsed*3);
                }));



        }

        [Test]
        public void TestIslandWithTreeAndCog()
        {
            var island = createTestObject();
            var tree = createTestObject();
            var cog = createTestObject();

            var relIsland = island.GetComponent<IRelativePositionComponent>();
            var relTree = tree.GetComponent<IRelativePositionComponent>();
            var reltCog = cog.GetComponent<IRelativePositionComponent>();

            relTree.Parent = relIsland;
            relTree.RelativePosition = new Vector3(1, 1, 0);
            reltCog.Parent = relTree;
            reltCog.RelativePosition = new Vector3(1, 1, 0);

            engine.AddSimulator(new BasicSimulator(delegate
            {
                relIsland.Position += new Vector3(0.1f, 0, 0) * TW.Graphics.Elapsed;
                relIsland.Rotation *= Quaternion.RotationAxis(Vector3.UnitY, TW.Graphics.Elapsed * 3);

                relTree.Rotation *= Quaternion.RotationAxis(Vector3.UnitY, TW.Graphics.Elapsed * 3);
            }));



        }

        private IGameObject createTestObject()
        {
            var ret = repository.CreateGameObject();
            ret.GetComponent<IMeshRenderComponent>().Mesh = TW.Assets.LoadMesh("Core/Barrel01");
            return ret;
        }

    }
}