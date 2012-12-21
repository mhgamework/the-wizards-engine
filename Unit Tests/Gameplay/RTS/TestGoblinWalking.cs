using System;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.Tests.Gameplay.Core;
using MHGameWork.TheWizards.VoxelTerraining;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Gameplay.RTS
{
    [TestFixture]
    public class TestGoblinWalking
    {
        [Test]
        public void TestRenderWalking()
        {
            var engine = new TWEngine { DontLoadPlugin = true };
            engine.Initialize();
            VoxelTerrainTest.generateTerrain(1, 1);
            var world = TW.Data.GetSingleton<WorldRendering.World>();
            var TestGoblin = new Goblin();
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new GoblinCommanderSimulator());
            engine.AddSimulator(new GoblinRendererSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.Run();
        }
        private static VoxelTerrainChunk createTerrain()
        {
            var terr = new VoxelTerrainChunk();
            terr.Size = new Point3(10, 10, 10);
            terr.Create();

            var random = new Random();
            for (int i = 0; i < 30; i++)
            {
                terr.GetVoxelInternal(new Point3(random.Next(9), random.Next(9), random.Next(9))).Filled = true;
            }

            terr.GetVoxelInternal(new Point3(1, 1, 1)).Filled = true;
            return terr;
        }

    }
}