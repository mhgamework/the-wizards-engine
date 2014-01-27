using System.Diagnostics.Contracts;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Rendering;
using MHGameWork.TheWizards.Scattered.Simulation;
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
        private ItemType airCrystalType;
        private float nextCliffTimeout = 0;

        [SetUp]
        public void Setup()
        {
            level = new Level();
            airCrystalType = new ItemType() { Name = "Air Crystal" };

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

        [Test]
        public void TestIslandUpdateConstructions()
        {
            createSomeIslands();
            createIslandConstructions();

            engine.AddSimulator(new ConstructionSimulator(level));

            isl1.Construction.UpdateAction = delegate
            {
                if (nextCliffTimeout > TW.Graphics.TotalRunTime) return;
                if (isl1.Inventory.GetAmountOfType(airCrystalType) >= 4) return;
                nextCliffTimeout = TW.Graphics.TotalRunTime + 5;
                isl1.Inventory.AddNewItems(airCrystalType, 1);
            };

            visualizeLevel();
        }

        [Test]
        public void TestInterIslandMovement()
        {
            createSomeIslands();
            createIslandConstructions();

            engine.AddSimulator(new InterIslandMovementSimulator(level));

            isl1.Construction.UpdateAction = delegate
            {
                if (nextCliffTimeout > TW.Graphics.TotalRunTime) return;
                if (isl1.Inventory.GetAmountOfType(airCrystalType) >= 4) return;
                nextCliffTimeout = TW.Graphics.TotalRunTime + 5;
                isl1.Inventory.AddNewItems(airCrystalType, 1);
            };

            visualizeLevel();
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

        private void createIslandConstructions()
        {
            isl1.Construction.Name = "Crystal cliffs";
            isl1.Inventory.AddNewItems(airCrystalType, 1);

            isl2.Construction.Name = "Warehouse";
            isl2.Inventory.AddNewItems(new ItemType() { Name = "Scrap" }, 12);
            isl2.Inventory.AddNewItems(new ItemType() { Name = "Iron ore" }, 3);
        }



    }
}