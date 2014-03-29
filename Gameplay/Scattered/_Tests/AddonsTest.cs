﻿using System;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.Scattered.Core;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.ProcBuilder;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered.Simulation;
using MHGameWork.TheWizards.Scattered.Simulation.Playmode;
using MHGameWork.TheWizards.Scattered.Simulation.Sandbox;
using MHGameWork.TheWizards.Simulators;
using NUnit.Framework;
using ProceduralBuilder.Building;
using ProceduralBuilder.Shapes;
using SlimDX;
using MHGameWork.TheWizards.Scattered._Engine;
using System.Linq;
using Castle.Core.Internal;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    [TestFixture]
    [EngineTest]
    public class AddonsTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestResource()
        {
            var level = new Level();
            var player = level.LocalPlayer;
            addPlaySimulators(level, player);

            var i = createTestIsland(level);

            var type1 = new ItemType { TexturePath = "Scattered\\Items\\coal.jpg", Name = "Coal" };
            var type2 = new ItemType { TexturePath = "Scattered\\Items\\gem1.jpg", Name = "Gems" };

            i.AddAddon(new Resource(level, i.Node.CreateChild(), type1)
                .Alter(k => k.Amount = 7));

            i.AddAddon(new Resource(level, i.Node.CreateChild(), type2)
                .Alter(k => k.Amount = 2)
                .Alter(k => k.Node.Relative = Matrix.Translation(1, 0, 0)));

            engine.AddSimulator(new BasicSimulator(() =>
                {
                    drawIslandFaces(i);
                }));
        }

        private void drawIslandFaces(Island island)
        {
            island.Descriptor.BaseElements.OfType<Face>().ForEach(f =>
                {
                    TW.Graphics.LineManager3D.WorldMatrix = f.GetWorldMatrix();
                    TW.Graphics.LineManager3D.AddBox(new BoundingBox(new Vector3(0, 0, 0), new Vector3(f.Size.X, f.Size.Y, 0)), new Color4(0, 0, 0));
                });
        }

        private Island createTestIsland(Level level)
        {
            var i = level.CreateNewIsland(new Vector3());
            var desc = new WorldGenerator.IslandDescriptor();
            desc.BaseElements = new[] { new Face("banana", Matrix.RotationX(-MathHelper.PiOver2), new Vector2(5, 5)) }.OfType<IBuildingElement>().ToList();
            desc.seed = 0;
            i.Descriptor = desc;
            return i;

        }

        private void addPlaySimulators(Level level, ScatteredPlayer player)
        {
            engine.AddSimulator(new PlayerMovementSimulator(level, player).Alter(k => k.NoclipMode = true));
            engine.AddSimulator(new PlayerInteractionSimulator(level, player));
            engine.AddSimulator(new GameplaySimulator(level));
            engine.AddSimulator(new ClusterPhysicsSimulator(level));
            engine.AddSimulator(new PlayerCameraSimulator(player));

            engine.AddSimulator(new ScatteredRenderingSimulator(level, () => level.EntityNodes,
                                                                () => level.Islands.SelectMany(c => c.Addons)));
            engine.AddSimulator(new WorldRenderingSimulator());

        }
    }
}