using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Rendering;
using MHGameWork.TheWizards.Scattered.Simulation;
using MHGameWork.TheWizards.Scattered.Simulation.Sandbox;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    [TestFixture]
    [EngineTest]
    public class SandboxTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        /// <summary>
        /// Runs the sandbox for the game. Game mechanics should be like in the real game, but the user can build anything (sandbox mode)
        /// </summary>
        [Test]
        public void TestSandbox()
        {
            var level = new Level();
            level.CreateNewIsland(new Vector3(20, 0, 30));
            var config = new EditorConfiguration();

            engine.AddSimulator(new SandboxControllerSimulator(level, config));
            engine.AddSimulator(new WorldInputtingSimulator(config));

            engine.AddSimulator(new ConstructionSimulator(level));
            engine.AddSimulator(new InterIslandMovementSimulator(level,createPathfinder()));

            engine.AddSimulator(new LevelRenderer(level));
            engine.AddSimulator(new WorldRenderingSimulator());
        }




        /// <summary>
        /// Create island with crystal cliffs and check if crystals grow
        /// TODO: This test is unwritten due to the fact that it was tested using the Sandbox. 
        /// Epic testing facilities or helpers can be created to record this manual testing, or speedup
        /// </summary>
        [Test]
        public void TestCrystalCliffsGrow()
        {
            // Thinks that should be checked: inventory of cliff
            throw new NotImplementedException();
        }

        /// <summary>
        /// A cliff with a path to a warehouse via several islands, should delivers goods to the warehouse
        /// </summary>
        [Test]
        public void TestCrystalCliffsDeliverToWarehouse()
        {
            // Thinks that should be checked: inventory of cliff and warehouse
            throw new NotImplementedException();
        }

        /// <summary>
        /// A cliff with a path to a 2 warehouse via several islands, should deliver to closest
        /// </summary>
        [Test]
        public void TestCrystalCliffsDeliverToClosestWarehouse()
        {
            // Thinks that should be checked: inventory of cliff and warehouse
            throw new NotImplementedException();
        }

        private static PathFinder2D<Island> createPathfinder()
        {
            var ret = new PathFinder2D<Island>();
            ret.ConnectionProvider = new IslandConnectionProvider();
            return ret;
        }
    }
}