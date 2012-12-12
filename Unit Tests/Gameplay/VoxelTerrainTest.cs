using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.VoxelTerraining;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    [TestFixture]
    public class VoxelTerrainTest
    {
        [Test]
        public void TestSimpleVoxelTerrain()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            var terr = new VoxelTerrain();
            terr.Size = new Point3(10, 10, 10);
            terr.Create();

            var random = new Random();

            for (int i = 0; i < 30; i++)
            {
                terr.GetVoxel(new Point3(random.Next(9), random.Next(9), random.Next(9))).Filled = true;
                
            }

            terr.GetVoxel(new Point3(1, 1, 1)).Filled = true;

            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();
        }

        [Test]
        public void TestBlob()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            var terr = new VoxelTerrain();
            terr.Size = new Point3(100, 100, 100);
            terr.Create();

            for (int x = 1; x < 10; x++)
            {
                for (int y = 1; y < 10; y++)
                {
                    for (int z = 1; z < 10; z++)
                    {
                        terr.GetVoxel(new Point3(x, y, z)).Filled = true;   
                    }
                }
            }

            engine.AddSimulator(new VoxelTerrainSimulator());
            //engine.AddSimulator(new EntityBatcherSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();
        }

        [Test]
        public void TestPlanetSurface()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            var terr = new VoxelTerrain();
            int i = 300;
            terr.Size = new Point3(i, 30, i);
            terr.Create();

            for (int x = 1; x <i; x++)
            {
                for (int y = 1; y < 5; y++)
                {
                    for (int z = 1; z < i; z++)
                    {
                        terr.GetVoxel(new Point3(x, y, z)).Filled = true;
                    }
                }
            }

            engine.AddSimulator(new VoxelTerrainSimulator());
            //engine.AddSimulator(new EntityBatcherSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();
        }
    }
}