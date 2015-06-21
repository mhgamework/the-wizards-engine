using System;
using System.Diagnostics;
using DirectX11;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using NUnit.Framework;
using SlimDX;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    [TestFixture]
    [EngineTest]
    public class PerformanceTest
    {
        /// <summary>
        /// Time to build a hermite grid (to data representation) from a density function 20x20x20
        /// </summary>
        [Test]
        public void TestGenerateTerrain20()
        {
            var dens = VoxelTerrainGenerationTest.createDensityFunction5Perlin(11);
            var grid = (AbstractHermiteGrid) new DensityFunctionHermiteGrid(dens, new Point3(20, 20, 20));

            var times = 10;
            var s = new Stopwatch();
            s.Start();

            for (int i = 0; i < 10; i++)
            {
                grid = HermiteDataGrid.CopyGrid(grid);
            }
            s.Stop();
            Console.WriteLine("Time per copy:" + s.Elapsed.TotalSeconds / times);


        }
        /// <summary>
        /// Time to build a hermite grid (to data representation) from a density function 40x40x40
        /// </summary>
        [Test]
        public void TestGenerateTerrain80()
        {
            var dens = VoxelTerrainGenerationTest.createDensityFunction5Perlin(11);
            var grid = (AbstractHermiteGrid) new DensityFunctionHermiteGrid(dens, new Point3(80, 80, 80));

            var times = 10;
            var s = new Stopwatch();
            s.Start();

            for (int i = 0; i < 10; i++)
            {
                grid = HermiteDataGrid.CopyGrid(grid);
            }
            s.Stop();
            Console.WriteLine("Time per copy:" + s.Elapsed.TotalSeconds / times);


        }

        /// <summary>
        /// Counts number of density lookups, and calls the density function that number of times for position 1,1,1. Outputs duration
        /// </summary>
        [Test]
        public void TestCountDensLookups20()
        {
            var dens = VoxelTerrainGenerationTest.createDensityFunction5Perlin(11);


            var numLookups = 0;
            var newDens = new Func<Vector3, float>(v =>
                {
                    numLookups++;
                    return dens(v);
                });

            int size = 20;
            var grid = (AbstractHermiteGrid) new DensityFunctionHermiteGrid(newDens, new Point3(size, size, size));


            grid = HermiteDataGrid.CopyGrid(grid);

            Console.WriteLine("Density lookups: " + numLookups);
            Console.WriteLine("Lookups per cell: {0:#0.00}" , (float)numLookups / (size * size * size));
        }

        /// <summary>
        /// Counts number of density lookups, and calls the density function that number of times for position 1,1,1. Outputs duration
        /// </summary>
        [Test]
        public void TestDensityCalcPerformance()
        {
            var dens = VoxelTerrainGenerationTest.createDensityFunction5Perlin(11);

            var times = 10000;
            var s = new Stopwatch();
            s.Start();

            for (int i = 0; i < times; i++)
            {
                float a = dens(new Vector3(1));
            }
            s.Stop();
            var densityEval = s.Elapsed.TotalSeconds / times;
            Console.WriteLine("Density evaluation: 100% - {0} ms", +densityEval * 1000);


            s.Reset();
            s.Start();

            for (int i = 0; i < times * 5; i++)// 5 trilerps per density lookup
            {
                
                float a = TWMath.triLerp(new Vector3(0.5f), 4, 7, 8, 6, 4, 8, 9, 7);
            }
            s.Stop();
            var trilerp = s.Elapsed.TotalSeconds / times;
            Console.WriteLine("Trilerp : {0:###}% - {1}ms", trilerp / densityEval*100, trilerp * 1000);



            var ar = new Array3D<float>(new Point3(16, 16, 16));
            s.Reset();
            s.Start();

            for (int i = 0; i < times * 5 * 8; i++)// 8 getTiled per trilerp and 5 trilerps per density lookup
            {
                /*var pos = new Point3(5, 5, 5);
                pos.X = TWMath.nfmod(pos.X, ar.Size.X);
                pos.Y = TWMath.nfmod(pos.Y, ar.Size.Y);
                pos.Z = TWMath.nfmod(pos.Z, ar.Size.Z);
                float a = ar.arr[pos.X, pos.Y, pos.Z];*/
                float a = ar.GetTiled(new Point3(5, 5, 5));
            }
            s.Stop();
            var getTiled = s.Elapsed.TotalSeconds/times;
            Console.WriteLine("GetTiled : {0:###}% - {1}ms", getTiled / densityEval*100, getTiled * 1000);

        }

       

    }
}