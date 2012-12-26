using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Generation
{
    [TestFixture]
    [EngineTest]
    public class SimpleCaveGenerationTest
    {
        [Test]
        public void TestSimple()
        {
            var engine = EngineFactory.CreateEngine();
            engine.Initialize();

            var grid = new VoxelGrid<bool>(new Point3(20, 20, 20));

            var gen = new SimpleCaveGenerator();

            gen.GenerateRandom(grid);

            new VoxelTerrainConvertor().SetTerrain(grid.ToArray());


            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.Run();
        }
    }
}
