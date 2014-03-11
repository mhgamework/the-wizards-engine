using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.Scattered.Core;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Rendering;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered.Simulation;
using MHGameWork.TheWizards.Scattered.Simulation.Playmode;
using MHGameWork.TheWizards.Scattered.Simulation.Sandbox;
using MHGameWork.TheWizards.Simulators;
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


            var level = new Level();

            distributionHelper = new DistributionHelper(level, pathfinder);
            roundState = new RoundState();

            var interIslandMovementSimulator = new InterIslandMovementSimulator(level, pathfinder);

            var config = new EditorConfiguration();

            engine.AddSimulator(new LoadLevelSimulator(level));
            engine.AddSimulator(new PlayControllerSimulator(level, config, roundState,interIslandMovementSimulator));
            engine.AddSimulator(new WorldInputtingSimulator(config));

            engine.AddSimulator(interIslandMovementSimulator);
            engine.AddSimulator(new ClusterPhysicsSimulator(level));

            engine.AddSimulator(new ThirdPersonCameraSimulator());
            engine.AddSimulator(new LevelRenderer(level));
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        private static PathFinder2D<Island> createPathfinder()
        {
            var ret = new PathFinder2D<Island>();
            ret.ConnectionProvider = new IslandConnectionProvider();
            return ret;
        }



        [Test]
        public void TestCoreGame()
        {
            Island i;
            var level = new Level();
            var rootNode = new SceneGraphNode();
            var player = new ScatteredPlayer(rootNode.CreateChild());
            player.Position = new Vector3(0, 3, 0);

            i = level.CreateNewIsland(new Vector3(0, 0, 0));
            i.AddAddon(new Bridge(i.Node.CreateChild()));
            i.AddAddon(new FlightEngine(i.Node.CreateChild()));
            

            i = level.CreateNewIsland(new Vector3(10, 0, 0));
            i.AddAddon(new Bridge(i.Node.CreateChild()));

            i = level.CreateNewIsland(new Vector3(20, 0, 0));
            i.AddAddon(new Bridge(i.Node.CreateChild()));
            i.AddAddon(new Enemy(i.Node.CreateChild()));

            i = level.CreateNewIsland(new Vector3(30, 0, 0));
            i.AddAddon(new Bridge(i.Node.CreateChild()));
            i.AddAddon(new Storage(i.Node.CreateChild()));

            i = level.CreateNewIsland(new Vector3(40, 0, 0));
            i.AddAddon(new Bridge(i.Node.CreateChild()));
            i.AddAddon(new Resource(i.Node.CreateChild()));

            i = level.CreateNewIsland(new Vector3(40, 0, 0));
            i.AddAddon(new Bridge(i.Node.CreateChild()));
            i.AddAddon(new Tower(i.Node.CreateChild()));

            engine.AddSimulator(new EnemySpawningSimulator());
            engine.AddSimulator(new PlayerMovementSimulator(player));
            engine.AddSimulator(new PlayerInteractionSimulator(player));
            engine.AddSimulator(new PlayerCameraSimulator(player));
            
            engine.AddSimulator(new WorldRenderingSimulator());
            


        }
    }
}