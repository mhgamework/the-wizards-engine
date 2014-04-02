using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Core;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.ProcBuilder;
using MHGameWork.TheWizards.Scattered.Simulation.Playmode;
using MHGameWork.TheWizards.Scattered._Engine;
using NUnit.Framework;
using ProceduralBuilder.Building;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    [TestFixture]
    [EngineTest]
    public class EnemyTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestEnemyBehaviour()
        {
            var level = new Level();
            var player = level.LocalPlayer;
            addPlaySimulators(level, player);

            var islePos = new Vector3(30, 0, 30);
            Island island;
            createIsland(level, islePos, out island);

            var enemyRelativeStartPos = new Vector3(-30, 0, 5);
            var enemy = new Enemy(level, island.Node.CreateChild(), enemyRelativeStartPos);
            island.AddAddon(enemy);

            enemy.Activate();

            TW.Graphics.SpectaterCamera.FarClip = 2000;
        }

        private void createIsland(Level level, Vector3 islePos, out Island isle)
        {
            isle = level.CreateNewIsland(islePos);
            var desc = new WorldGenerator.IslandDescriptor();
            desc.seed = 0;
            isle.Descriptor = desc;

            var islandGenerator = new CachedIslandGenerator(new IslandGenerator(), new OBJExporter());
            var realtimeIslandGenerator = new IslandGenerator();
            desc.BaseElements = islandGenerator.GetIslandBase(isle.Descriptor.seed);
            IMesh temp;
            List<IBuildingElement> navMesh;
            List<IBuildingElement> buildmesh;
            List<IBuildingElement> bordermesh;
            realtimeIslandGenerator.GetIslandParts(desc.BaseElements, desc.seed, false, out temp, out navMesh, out buildmesh,
                                                   out bordermesh);
            isle.SpaceManager.BuildAreaMeshes = buildmesh;
            desc.BuildMesh = buildmesh;
            desc.NavMesh = navMesh;
            isle.Descriptor = desc;
            var mesh = islandGenerator.GetIslandMesh(desc.BaseElements, desc.seed);
            isle.Mesh = mesh;
            isle.Descriptor = desc;
        }

        [Test]
        public void TestEnemyInWorld()
        {
            var level = new Level();
            var player = level.LocalPlayer;
            addPlaySimulators(level, player);
            var gen = new WorldGenerator(level, new Random(0));

            gen.Generate();

            var nbEnemies = (int)Math.Floor(level.Islands.Count() * 0.5f);
            var rnd = new Random(0);
            for (int j = 1; j < nbEnemies; j++)
            {
                var index = rnd.Next(0, level.Islands.Count());

                var island = level.Islands.ElementAt(index);
                var enemy = new Enemy(level, island.Node.CreateChild(), new Vector3());
                island.AddAddon(enemy);
                enemy.Activate();
            }

            TW.Graphics.SpectaterCamera.FarClip = 2000;
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
    }
}
