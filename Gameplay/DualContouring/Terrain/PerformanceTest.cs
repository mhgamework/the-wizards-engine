using System;
using System.Collections.Generic;
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
        /// 45ms
        /// </summary>
        [Test]
        public void TestExtractSurfaceAllEdges()
        {
            int size = 16;
            var srcGrid = new DelegateHermiteGrid(p => p.X % 2 == 0 || p.Y % 2 == 0 || p.Z % 2 == 0, (p, i) => new Vector4(), new Point3(size, size, size));
            testExtractSurface(srcGrid);
        }
        /// <summary>
        /// 2 ms
        /// </summary>
        [Test]
        public void TestExtractSurfaceNoEdges()
        {
            //TODO: serious optimization idea: store all signs for each cube 
            int size = 16;
            var srcGrid = new DelegateHermiteGrid(p => true, (p, i) => new Vector4(), new Point3(size, size, size));
            testExtractSurface(srcGrid);
        }

        private static void testExtractSurface(DelegateHermiteGrid srcGrid)
        {
            var grid = HermiteDataGrid.CopyGrid(srcGrid);
            int times = 40;

            var vertices = new List<Vector3>(10 * 1000 * 1000);
            var indices = new List<int>(10 * 1000 * 1000);
            var extractor = new DualContouringAlgorithm();

            var time = PerformanceHelper.Measure(() =>
                {
                    for (int i = 0; i < times; i++)
                    {
                        vertices.Clear();
                        indices.Clear();
                        extractor.GenerateSurface(vertices, indices, srcGrid);
                    }
                });

            Console.WriteLine("Time: " + time.Multiply(1f / times).PrettyPrint());
            Console.WriteLine("{0:#.0}x{0:#.0}x{0:#.0} grid per second", Math.Pow(srcGrid.Dimensions.X * srcGrid.Dimensions.Y * srcGrid.Dimensions.Z / (time.TotalSeconds / times), 1 / 3f));
        }

        [Test]
        public void TestHermiteCopyingNoEdges()
        {
            int size = 32;
            var srcGrid = new DelegateHermiteGrid(p => true, (p, i) => new Vector4(), new Point3(size, size, size));
            int times = 50;

            Console.WriteLine(PerformanceHelper.Measure(() =>
                {
                    for (int i = 0; i < times; i++)
                    {
                        HermiteDataGrid.CopyGrid(srcGrid);
                    }
                }).Multiply(1f / times).PrettyPrint());
        }
        [Test]
        public void TestHermiteCopyingAllEdges()
        {
            int size = 32;
            var srcGrid = new DelegateHermiteGrid(p => p.X % 2 == 0 || p.Y % 2 == 0 || p.Z % 2 == 0, (p, i) => new Vector4(), new Point3(size, size, size));
            int times = 100;

            Console.WriteLine(PerformanceHelper.Measure(() =>
            {
                for (int i = 0; i < times; i++)
                {
                    HermiteDataGrid.CopyGrid(srcGrid);
                }
            }).Multiply(1f / times).PrettyPrint());
        }

        /// <summary>
        /// Time to build a hermite grid (to data representation) from a density function
        /// </summary>
        [Test]
        public void TestGenerateTerrain([Values(16, 32, 64)] int size)
        {
            var dens = VoxelTerrainGenerationTest.createDensityFunction5Perlin(11, 10);
            var densityGrid = (AbstractHermiteGrid)new DensityFunctionHermiteGrid(dens, new Point3(size, size, size));

            var times = 10;
            var s = new Stopwatch();
            s.Start();

            for (int i = 0; i < times; i++)
            {
                var grid = HermiteDataGrid.CopyGrid(densityGrid);
            }
            s.Stop();
            Console.WriteLine("Time per copy:" + s.Elapsed.Multiply(1f / times).PrettyPrint());
            Console.WriteLine("Time per cell: " + s.Elapsed.Multiply(1f / times / (size * size * size)).PrettyPrint());


        }

        /// <summary>
        /// Counts number of density lookups, and calls the density function that number of times for position 1,1,1. Outputs duration
        /// </summary>
        [Test]
        public void TestCountDensLookups20()
        {
            var dens = VoxelTerrainGenerationTest.createDensityFunction5Perlin(11, 10);


            var numLookups = 0;
            var newDens = new Func<Vector3, float>(v =>
                {
                    numLookups++;
                    return dens(v);
                });

            int size = 20;
            var grid = (AbstractHermiteGrid)new DensityFunctionHermiteGrid(newDens, new Point3(size, size, size));


            grid = HermiteDataGrid.CopyGrid(grid);

            Console.WriteLine("Density lookups: " + numLookups);
            Console.WriteLine("Lookups per cell: {0:#0.00}", (float)numLookups / (size * size * size));
        }

        /// <summary>
        /// Counts number of density lookups, and calls the density function that number of times for position 1,1,1. Outputs duration
        /// </summary>
        [Test]
        public void TestDensityCalcPerformance()
        {
            var dens = VoxelTerrainGenerationTest.createDensityFunction5Perlin(11, 10);

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
            Console.WriteLine("Trilerp : {0:###}% - {1}ms", trilerp / densityEval * 100, trilerp * 1000);



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
            var getTiled = s.Elapsed.TotalSeconds / times;
            Console.WriteLine("GetTiled : {0:###}% - {1}ms", getTiled / densityEval * 100, getTiled * 1000);

        }


        /// <summary>
        /// I estimated (by running the lodenv) that on average you need 500 meshes of 16x16x16 to render a terrain of size 32*1024
        ///    So lets measure and extrapolate!
        /// This is somewhat of the minimum we can achieve using CPU, so use it to see the necessity of going GPU
        /// </summary>
        [Test]
        public void EstimateLodDensitySamplingPerformance()
        {
            var env = new TerrainLodEnvironment();


            // At the moment of writing we averaged 3 densities per cell for 16x16x16, but lets assume its 1 (when alot of single material chunks exist)

            var numMeshes = 500;
            var numIts = 32;

            var extraSimulate = 10; // Do the test 'extraSimulate' times to make it more accurate, but this is again subtracted from total later
            estimateSamplingPerformance(extraSimulate, numIts, env, numMeshes);
        }

        private static void estimateSamplingPerformance(int extraSimulate, int numIts, TerrainLodEnvironment env, int numMeshes)
        {
            var perf = PerformanceHelper.Measure(() =>
                {
                    for (int j = 0; j < extraSimulate; j++)
                    {
                        for (int x = 0; x < numIts; x++)
                            for (int y = 0; y < numIts; y++)
                                for (int z = 0; z < numIts; z++)
                                {
                                    var value = env.densityFunction(new Vector3(x, y, z));
                                }
                    }
                });


            Console.WriteLine("Time for {0}x{0}x{0}: {1}", numIts, perf.Multiply(1f / extraSimulate).PrettyPrint());
            Console.WriteLine("Time for 32*1024 with lod: " + perf.Multiply((float)numMeshes / extraSimulate).PrettyPrint());
        }
    }
}