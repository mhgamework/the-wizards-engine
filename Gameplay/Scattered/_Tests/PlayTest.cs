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
using MHGameWork.TheWizards.Scattered.Simulation.Constructions;
using MHGameWork.TheWizards.Scattered.Simulation.Sandbox;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    [TestFixture]
    [EngineTest]
    public class PlayTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        /// <summary>
        /// Runs the sandbox for the game. Game mechanics should be like in the real game, but the user can build anything (sandbox mode)
        /// </summary>
        [Test]
        public void TestSandbox()
        {
            DistributionHelper distributionHelper = null;
            RoundState roundState = null;

            var pathfinder = createPathfinder();

            var constructionFactory = new ConstructionFactory(new Lazy<DistributionHelper>(() => distributionHelper), new Lazy<RoundState>(() => roundState));

            var level = new Level(constructionFactory);

            distributionHelper = new DistributionHelper(level, pathfinder);
            roundState = new RoundState();


            var config = new EditorConfiguration();

            engine.AddSimulator(new LoadLevelSimulator(level));
            engine.AddSimulator(new PlayControllerSimulator(level, config, roundState));
            engine.AddSimulator(new WorldInputtingSimulator(config));

            engine.AddSimulator(new ConstructionSimulator(level));
            engine.AddSimulator(new InterIslandMovementSimulator(level, pathfinder));

            engine.AddSimulator(new LevelRenderer(level));
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        private static PathFinder2D<Island> createPathfinder()
        {
            var ret = new PathFinder2D<Island>();
            ret.ConnectionProvider = new IslandConnectionProvider();
            return ret;
        }
    }
}