using System.Diagnostics.Contracts;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Scattered.Rendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    /// <summary>
    /// Tests for creating and managing an Island graph
    /// </summary>
    [EngineTest]
    [TestFixture]
    [ContractVerification(true)]
    public class TestIslandGraph
    {
        private Level level;
        private Island isl1;
        private Island isl2;
        private Island isl3;
        private TWEngine engine = EngineFactory.CreateEngine();

        [SetUp]
        public void Setup()
        {
            level = new Level();
        }

        [Test]
        public void TestSomeIslands()
        {
            createSomeIslands();

            visualizeLevel();
        }

        [Test]
        public void TestSomeBridges()
        {
            createSomeIslands();
            isl1.AddBridgeTo(isl3);
            isl3.AddBridgeTo(isl2);

            visualizeLevel();

            CollectionAssert.Contains(isl3.ConnectedIslands, isl1);
            CollectionAssert.Contains(isl3.ConnectedIslands, isl2);
            CollectionAssert.Contains(isl1.ConnectedIslands, isl3);
            CollectionAssert.Contains(isl2.ConnectedIslands, isl3);
        }


        public void visualizeLevel()
        {
            var levelRenderer = new LevelRenderer(level);

            engine.AddSimulator(levelRenderer);
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        private void createSomeIslands()
        {
            isl1 = level.CreateNewIsland(new Vector3(20, 10, 0));
            isl2 = level.CreateNewIsland(new Vector3(-50, 10, 0));
            isl3 = level.CreateNewIsland(new Vector3(0, 20, 40));
        }
    }
}