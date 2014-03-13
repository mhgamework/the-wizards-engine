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
using MHGameWork.TheWizards.Scattered._Engine;

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
            engine.AddSimulator(new PlayControllerSimulator(level, config, roundState, interIslandMovementSimulator));
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


            var level = new Level();
            Action<Island> addBridge = il => il.AddAddon(new Bridge(level, il.Node.CreateChild()).Alter(b => b.Node.Relative = Matrix.Translation(0, 0, 8)));

            Island i;

            var player = new ScatteredPlayer(level, level.Node.CreateChild());
            player.Position = new Vector3(0, 3, 0);

            i = level.CreateNewIsland(new Vector3(0, 0, 0));
            addBridge(i);
            i.AddAddon(new FlightEngine(level, i.Node.CreateChild()));


            i = level.CreateNewIsland(new Vector3(10, 0, 0));
            addBridge(i);

            i = level.CreateNewIsland(new Vector3(20, 0, 0));
            addBridge(i);
            i.AddAddon(new Enemy(level, i.Node.CreateChild()).Alter(e => e.Node.Relative = Matrix.Translation(Vector3.UnitY * 3)));

            i = level.CreateNewIsland(new Vector3(30, 0, 0));
            addBridge(i);
            i.AddAddon(new Storage(level, i.Node.CreateChild()));

            i = level.CreateNewIsland(new Vector3(40, 0, 0));
            addBridge(i);
            i.AddAddon(new Resource(level, i.Node.CreateChild()));

            i = level.CreateNewIsland(new Vector3(40, 0, 0));
            addBridge(i);
            i.AddAddon(new Tower(level, i.Node.CreateChild()));

            engine.AddSimulator(new EnemySpawningSimulator());
            engine.AddSimulator(new PlayerMovementSimulator(player));
            engine.AddSimulator(new PlayerInteractionSimulator(player));
            engine.AddSimulator(new PlayerCameraSimulator(player));

            engine.AddSimulator(new SceneGraphRenderingSimulator(() => level.EntityNodes));
            engine.AddSimulator(new WorldRenderingSimulator());



        }
    }
}