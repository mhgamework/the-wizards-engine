using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Generation
{
    [TestFixture]
    [EngineTest]
    public class VoxelTerrainConvertorTest
    {
        [Test]
        public void TestSimple()
        {
            var engine = EngineFactory.CreateEngine();
            engine.Initialize();

            var gen = new VoxelTerrainConvertor();

            var data = new bool[5,5,5];
            for (int i = 0; i < 5; i++)
            {
                data[i, i, i] = true;
            }

            gen.SetTerrain(data);

            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.Run();
        }
    }
}
