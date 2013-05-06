using System;
using System.Linq;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.Tests.Gameplay.Core;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;
using SlimDX;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.Tests.Gameplay.RTS
{
    [TestFixture]
    [EngineTest]
    public class TestGoblinWalking
    {
        [Test]
        public void TestRenderWalking()
        {
            var engine = EngineFactory.CreateEngine();
            engine.Initialize();
            if (TW.Data.GetSingleton<VoxelTerrain>().NumChunks == 0)
            {
                VoxelTerrainTest.generateTerrain(1, 1);
                var world = TW.Data.GetSingleton<Engine.WorldRendering.World>();    
            }
            if (TW.Data.Objects.Count(o => o is Goblin) == 0)
            {
                var TestGoblin = new Goblin();
                
            }

            TW.Data.GetSingleton<CameraInfo>().Mode = CameraInfo.CameraMode.FirstPerson;
            engine.AddSimulator(new FirstPersonCameraSimulator());
            engine.AddSimulator(new TerrainEditorSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new GoblinCommanderSimulator());
            engine.AddSimulator(new GoblinMovementSimulator());
            engine.AddSimulator(new RTSRendererSimulator());
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