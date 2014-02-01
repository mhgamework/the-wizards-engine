using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Rendering;
using MHGameWork.TheWizards.Scattered.Simulation;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using SlimDX;
using System.Linq;

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
            createBridges();

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
            createBridges();

            createIslandConstructions();

            engine.AddSimulator(new ConstructionSimulator(level));

            isl1.Construction.UpdateAction = new CliffsConstructionAction(isl1, airCrystalType);

            visualizeLevel();
        }

        [Test]
        public void TestRenderTravellers()
        {
            createSomeIslands();
            createBridges();

            Traveller t;
            t = level.CreateNewTraveller(isl1, () => null);
            t.BridgePosition = new BridgePosition(isl1, isl3, 0.2f);
            t = level.CreateNewTraveller(isl1, () => null);
            t.BridgePosition = new BridgePosition(isl1, isl3, 0.6f);
            t = level.CreateNewTraveller(isl3, () => null);
            t.BridgePosition = new BridgePosition(isl3, isl2, 0.5f);

            visualizeLevel();
        }

        [Test]
        public void TestInterIslandMovementSimulator()
        {
            createSomeIslands();
            createIslandConstructions();
            createBridges();

            engine.AddSimulator(new InterIslandMovementSimulator(level, createPathFinderMock()));

            //create a traveller which has the crystals form the cliffs and drops them off at the warehouse.
            createCrystalToWarehouseCart();

            visualizeLevel();
        }

        [Test]
        public void TestIslandConnectionProviderIntegration()
        {
            createSomeIslands();
            createIslandConstructions();
            createBridges();

            var finder = new PathFinder2D<Island>();
            finder.ConnectionProvider = new IslandConnectionProvider();
            engine.AddSimulator(new InterIslandMovementSimulator(level, finder));

            //create a traveller which has the crystals form the cliffs and drops them off at the warehouse.


            createCrystalToWarehouseCart();

            visualizeLevel();
        }

        private Traveller createCrystalToWarehouseCart()
        {
            Traveller trav = null;
            trav = level.CreateNewTraveller(isl1, delegate
                                                      {
                                                          if (trav.IsAtIsland(isl2))
                                                          {
                                                              // When at warehouse, drop off goods
                                                              trav.Inventory.TransferItemsTo(isl2.Inventory, airCrystalType,
                                                                                             trav.Inventory.GetAmountOfType(
                                                                                                 airCrystalType));
                                                          }

                                                          if (trav.Inventory.GetAmountOfType(airCrystalType) > 0)
                                                              return isl2; // When has goods, go to warehouse

                                                          if (trav.IsAtIsland(isl1))
                                                          {
                                                              // When home and empty, no more destination!
                                                              return null;
                                                          }

                                                          return isl1; // go home!
                                                      });

            isl1.Inventory.TransferItemsTo(trav.Inventory, airCrystalType, 1);

            return trav;
        }

        private IPathFinder2D<Island> createPathFinderMock()
        {

            //TODO: This should work, but i think there is a problem with castle windsor trying to load the Island class from a previous version of the gameplay dll (due to hotloading)
            //var ret = Substitute.For<IIslandPathFinder>();
            //ret.FindPath(isl1, isl3).Returns(new List<Island>(new[] { isl1, isl3 }));
            //ret.FindPath(isl3, isl1).Returns(new List<Island>(new[] { isl3, isl1 }));
            //ret.FindPath(isl2, isl3).Returns(new List<Island>(new[] { isl2, isl3 }));
            //ret.FindPath(isl3, isl2).Returns(new List<Island>(new[] { isl3, isl2 }));


            //ret.FindPath(isl1, isl2).Returns(new List<Island>(new[] { isl1, isl3, isl2 }));
            //ret.FindPath(isl2, isl1).Returns(new List<Island>(new[] { isl2, isl3, isl1 }));

            var ret = new DummyPathFinder(isl1, isl2, isl3);

            return ret;
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
            //isl2.Inventory.AddNewItems(new ItemType() { Name = "Iron ore" }, 3);
        }

        private void createBridges()
        {
            isl1.AddBridgeTo(isl3);
            isl3.AddBridgeTo(isl2);
        }


        /// <summary>
        ///  Note working, here to demonstrate a problem with nsubstitute and the engine.
        /// </summary>
        public interface IIslandPathFinder : IPathFinder2D<Island>
        {

        }
        internal class DummyPathFinder : IPathFinder2D<Island>
        {
            private List<List<Island>> data;

            public DummyPathFinder(Island isl1, Island isl2, Island isl3)
            {
                data = new List<List<Island>>();
                data.Add((new[] { isl1, isl3, isl1, isl3 }).ToList());
                data.Add((new[] { isl2, isl3, isl2, isl3 }).ToList());
                data.Add((new[] { isl3, isl1, isl3, isl1 }).ToList());
                data.Add((new[] { isl3, isl2, isl3, isl2 }).ToList());

                data.Add((new[] { isl1, isl2, isl1, isl3, isl2 }).ToList());
                data.Add((new[] { isl2, isl1, isl2, isl3, isl1 }).ToList());
            }
            public List<Island> FindPath(Island start, Island goal)
            {
                foreach (var list in data)
                {
                    if (list[0] == start && list[1] == goal)
                        return list.Skip(2).ToList();
                }

                throw new InvalidOperationException("Path not found!");

            }
        }

        private class CliffsConstructionAction : IConstructionAction
        {
            private readonly Island isl1;
            private readonly ItemType airCrystalType;
            float nextCliffTimeout = 0;


            public CliffsConstructionAction(Island isl1, ItemType airCrystalType)
            {
                this.isl1 = isl1;
                this.airCrystalType = airCrystalType;
            }

            public void Update()
            {
                if (nextCliffTimeout > TW.Graphics.TotalRunTime) return;
                if (isl1.Inventory.GetAmountOfType(airCrystalType) >= 4) return;
                nextCliffTimeout = TW.Graphics.TotalRunTime + 5;
                isl1.Inventory.AddNewItems(airCrystalType, 1);
            }
        }
    }


}