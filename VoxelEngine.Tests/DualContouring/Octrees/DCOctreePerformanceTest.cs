using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.VoxelEngine.DynamicWorld.OctreeDC;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld.Tests.OctreeDC
{
    public class DCOctreePerformanceTest
    {
        private DCOctreeAlgorithm algo;

        [SetUp]
        public void Setup()
        {
            algo = new DCOctreeAlgorithm();
        }

        [Test]
        public void MeasureSphere16()
        {
            var tree = ExampleGrids.CreateSphereOctree();

            measureExtractSurface( tree, new Point3(16,16,16),3000);

        }

        [Test]
        public void MeasureSphere32()
        {
            var tree = ExampleGrids.CreateSphereOctree(32);

            measureExtractSurface(tree, new Point3(32, 32, 32), 100);

        }
        [Test]
        public void MeasureSphere64()
        {
            var tree = ExampleGrids.CreateSphereOctree(64);

            measureExtractSurface(tree, new Point3(64,64,64), 40);

        }

        private void measureExtractSurface( SignedOctreeNode tree, Point3 dimensions, int times = 40 )
        {
            var vertices = new List<Vector3>(10 * 1000 * 1000);
            var indices = new List<int>(10 * 1000 * 1000);
            var extractor = new DCUniformGridAlgorithm();

            var time = PerformanceHelper.Measure(() =>
            {
                for (int i = 0; i < times; i++)
                {
                    var result = algo.GenerateTrianglesForOctree(tree);
                }   
            });

            Console.WriteLine("Time: " + time.Multiply(1f / times).PrettyPrint());
            Console.WriteLine("{0:#.0}x{0:#.0}x{0:#.0} grid per second", Math.Pow(dimensions.X * dimensions.Y * dimensions.Z / (time.TotalSeconds / times), 1 / 3f));
        }
    }
}