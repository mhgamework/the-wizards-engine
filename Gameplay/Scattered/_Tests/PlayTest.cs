using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.Scattered.Core;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered.Simulation;
using MHGameWork.TheWizards.Scattered.Simulation.Playmode;
using MHGameWork.TheWizards.Simulators;
using NUnit.Framework;
using SlimDX;
using MHGameWork.TheWizards.Scattered._Engine;
using System.Linq;
using Module = Autofac.Module;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    [TestFixture]
    [EngineTest]
    public class PlayTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestCoreGame()
        {
            var level = new Level();
            var player = level.LocalPlayer;
            addPlaySimulators(level, player);

            Action<Island> addBridge = il => il.AddAddon(new Bridge(level, il.Node.CreateChild()).Alter(b => b.Node.Relative = Matrix.Translation(0, 0, 8)));

            Island i;

            player.Position = new Vector3(0, 3, 0);

            i = level.CreateNewIsland(new Vector3(0, 0, 0));
            addBridge(i);
            i.AddAddon(new FlightEngine(level, i.Node.CreateChild(), level.CoalType));


            i = level.CreateNewIsland(new Vector3(10, 0, 0));
            addBridge(i);

            i = level.CreateNewIsland(new Vector3(20, 0, 0));
            addBridge(i);
            i.AddAddon(new Enemy(level, i.Node.CreateChild(), new Vector3()).Alter(e => e.Node.Relative = Matrix.Translation(Vector3.UnitY * 3)));

            i = level.CreateNewIsland(new Vector3(30, 0, 0));
            addBridge(i);
            var crystalItem = new ItemType() { Mesh = TW.Assets.LoadMesh("Scattered\\Models\\Items\\CrystalItem"), Name = "Crystal" };
            i.AddAddon(new Storage(level, i.Node.CreateChild())
                .Alter(s => s.Inventory.AddNewItems(crystalItem, 4)));

            i = level.CreateNewIsland(new Vector3(40, 0, 0));
            addBridge(i);
            i.AddAddon(new ResourceGenerator(level, i.Node.CreateChild(), crystalItem));

            i = level.CreateNewIsland(new Vector3(50, 0, 0));
            addBridge(i);
            i.AddAddon(new Tower(level, i.Node.CreateChild()));



            //engine.AddSimulator(new AudioSimulator());
        }

        private void addPlaySimulators(Level level, ScatteredPlayer player)
        {
            engine.AddSimulator(new EnemySpawningSimulator(level, 0.1f));
            engine.AddSimulator(new PlayerMovementSimulator(level, player));
            engine.AddSimulator(new PlayerInteractionSimulator(level, player));
            engine.AddSimulator(new GameplaySimulator(level));
            engine.AddSimulator(new ClusterPhysicsSimulator(level));
            engine.AddSimulator(new PlayerCameraSimulator(player));

            engine.AddSimulator(new ScatteredRenderingSimulator(level, () => level.EntityNodes,
                                                                () => level.Islands.SelectMany(c => c.Addons)));
            engine.AddSimulator(new WorldRenderingSimulator());
            //engine.AddSimulator(new AudioSimulator());

        }

        [Test]
        public void TestWorldGeneration()
        {
            var level = new Level();
            engine.AddSimulator(new ScatteredRenderingSimulator(level, () => level.EntityNodes, () => level.Islands.SelectMany(c => c.Addons)));
            engine.AddSimulator(new WorldRenderingSimulator());

            var gen = new WorldGenerator(level, new Random());

            gen.Generate();

            TW.Graphics.SpectaterCamera.FarClip = 2000;

        }

        [Test]
        public void TestProceduralGame()
        {
            var level = new Level();
            var player = level.LocalPlayer;
            addPlaySimulators(level, player);
            var gen = new WorldGenerator(level, new Random(0));

            gen.Generate();

            TW.Graphics.SpectaterCamera.FarClip = 2000;
        }


        [Test]
        public void PlayGame()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(t => t.Name.EndsWith("Simulator")).SingleInstance();
            builder.RegisterType<ScatteredGame>();

            builder.Register(c => new EnemySpawningSimulator(c.Resolve<Level>(), 100)).SingleInstance();
            builder.Register(c =>
                {
                    var lvl = c.Resolve<Level>();
                    return new ScatteredRenderingSimulator(lvl, () => lvl.EntityNodes,
                                                    () => lvl.Islands.SelectMany(d => d.Addons));

                });

            builder.Register(c => new Level()).SingleInstance();
            builder.Register(c => c.Resolve<Level>().LocalPlayer).SingleInstance();

            builder.Register(c => new Random(0));
            builder.RegisterType<WorldGenerator>().SingleInstance();

            builder.RegisterModule<LogRequestsModule>();


            var cont = builder.Build();

            cont.Resolve<ScatteredGame>().LoadIntoEngine(engine);
        }





    }

    public class LogRequestsModule : Module
    {
        protected override void AttachToComponentRegistration(
          IComponentRegistry componentRegistry,
          IComponentRegistration registration)
        {
            // Use the event args to log detailed info
            registration.Preparing += (sender, args) =>
                                      Console.WriteLine(
                                          "Resolving concrete type {0}",
                                          args.Component.Activator.LimitType);
        }
    }
    public class ScatteredGame
    {
        private EnemySpawningSimulator EnemySpawningSimulator;
        private PlayerMovementSimulator PlayerMovementSimulator;
        private PlayerInteractionSimulator PlayerInteractionSimulator;
        private GameplaySimulator GameplaySimulator;
        private ClusterPhysicsSimulator ClusterPhysicsSimulator;
        private PlayerCameraSimulator PlayerCameraSimulator;

        private ScatteredRenderingSimulator ScatteredRenderingSimulator;
        private WorldRenderingSimulator WorldRenderingSimulator;

        private WorldGenerator gen;

        public ScatteredGame(EnemySpawningSimulator enemySpawningSimulator, PlayerMovementSimulator playerMovementSimulator, PlayerInteractionSimulator playerInteractionSimulator, GameplaySimulator gameplaySimulator, ClusterPhysicsSimulator clusterPhysicsSimulator, PlayerCameraSimulator playerCameraSimulator, ScatteredRenderingSimulator scatteredRenderingSimulator, WorldRenderingSimulator worldRenderingSimulator, Level level, WorldGenerator gen)
        {
            EnemySpawningSimulator = enemySpawningSimulator;
            PlayerMovementSimulator = playerMovementSimulator;
            PlayerInteractionSimulator = playerInteractionSimulator;
            GameplaySimulator = gameplaySimulator;
            ClusterPhysicsSimulator = clusterPhysicsSimulator;
            PlayerCameraSimulator = playerCameraSimulator;
            ScatteredRenderingSimulator = scatteredRenderingSimulator;
            WorldRenderingSimulator = worldRenderingSimulator;
            this.gen = gen;
        }

        public void LoadIntoEngine(TWEngine engine)
        {
            engine.AddSimulator(EnemySpawningSimulator);
            engine.AddSimulator(PlayerMovementSimulator);
            engine.AddSimulator(PlayerInteractionSimulator);
            engine.AddSimulator(GameplaySimulator);
            engine.AddSimulator(ClusterPhysicsSimulator);
            engine.AddSimulator(PlayerCameraSimulator);

            engine.AddSimulator(ScatteredRenderingSimulator);
            engine.AddSimulator(WorldRenderingSimulator);
            //engine.AddSimulator(new AudioSimulator());

            gen.Generate();

            TW.Graphics.SpectaterCamera.FarClip = 2000;
        }
    }
}
