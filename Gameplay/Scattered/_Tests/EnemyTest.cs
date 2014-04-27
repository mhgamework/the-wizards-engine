﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.GameLogic.Objects;
using MHGameWork.TheWizards.Scattered.GameLogic.Services;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.ProcBuilder;
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
            var desc = new WorldGenerationService.IslandDescriptor();
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
            isle.SpaceAllocator.BuildAreaMeshes = buildmesh;
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
            var gen = new WorldGenerationService(level, new Random(0),null);

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
            throw new NotImplementedException();
            //engine.AddSimulator(new EnemySpawningService(level, 0.1f));
            //engine.AddSimulator(new PlayerMovementSimulator(level, player));
            //engine.AddSimulator(new PlayerInteractionService(level, player));
            //engine.AddSimulator(new GameSimulationService(level));
            //engine.AddSimulator(new ClusterPhysicsService(level));
            //engine.AddSimulator(new PlayerCameraSimulator(player));

            //engine.AddSimulator(new ScatteredRenderingSimulator(level, () => level.EntityNodes,
            //                                                    () => level.Islands.SelectMany(c => c.Addons)));
            //engine.AddSimulator(new WorldRenderingSimulator());
            //engine.AddSimulator(new AudioSimulator());

        }


        [Test]
        public void TestHealing()
        {
            var level = new Level();
            var player = level.LocalPlayer;
            player.TakeDamage(0.9f);
            addPlaySimulators(level, player);

            var islePos = new Vector3(30, 0, 30);
            Island island;
            createIsland(level, islePos, out island);

            island.AddAddon(new Resource(level, island.Node.CreateChild(), level.FriesType).Alter(r => r.Amount = 3).Alter(r => r.Node.Position = new Vector3(10, 0, 0)));
            island.AddAddon(new Resource(level, island.Node.CreateChild(), level.FriesType).Alter(r => r.Amount = 1).Alter(r => r.Node.Position = new Vector3(3, 0, 8)));
            island.AddAddon(new Resource(level, island.Node.CreateChild(), level.FriesType).Alter(r => r.Amount = 5).Alter(r => r.Node.Position = new Vector3(18, 0, 1)));

            TW.Graphics.SpectaterCamera.FarClip = 2000;
        }
    }
}
