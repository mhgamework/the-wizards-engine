using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DualContouring._Test;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using NUnit.Framework;
using SlimDX;
using SlimDX.Direct3D11;
using TreeGenerator.NoiseGenerater;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    [EngineTest]
    [TestFixture]
    public class DensityHermiteGridTest
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Should test point and normal generation from density function, since the plane is at 9.5 in between the 9th and 10th point
        /// </summary>
        [Test]
        public void TestFlatDensityFunction()
        {
            Func<Vector3, float> densityFunction = p =>
                {
                    return 9.5f - p.Y;
                };

            var dimensions = new Point3(20, 20, 20);
            testDensityFunction(densityFunction, dimensions);
        }
        [Test]
        public void TestSlopeDensityFunction()
        {
            Func<Vector3, float> densityFunction = p =>
            {
                return 2.5f - p.Y + p.X;
            };

            var dimensions = new Point3(20, 20, 20);
            testDensityFunction(densityFunction, dimensions);
        }

        /// <summary>
        /// Should test point and normal calculation for flat, since the plane is at 9.5 in between the 9th and 10th point
        /// </summary>
        [Test]
        public void TestSineDensityFunction()
        {
            Func<Vector3, float> densityFunction = p =>
                {
                    var dens = 9.5f - p.Y;
                    dens += (float)Math.Sin((p.X + p.Z) * 0.5f);
                    return dens;
                };

            var dimensions = new Point3(20, 20, 20);

            testDensityFunction(densityFunction, dimensions);
        }


        [Test]
        public void TestSphereDensityFunction()
        {
            Func<Vector3, float> densityFunction = p =>
                {
                    return -Vector3.Distance(p, new Vector3(10, 10, 10)) + 5;
                };

            var dimensions = new Point3(20, 20, 20);

            testDensityFunction(densityFunction, dimensions);
        }

        [Test]
        public void TestDiamondDensityFunction()
        {
            Func<Vector3, float> densityFunction = p =>
                {
                    var diff = p - new Vector3(10, 10, 10);
                    var l = Math.Abs(diff.X) + Math.Abs(diff.Y) + Math.Abs(diff.Z);

                    return 8 - l;

                };

            var dimensions = new Point3(20, 20, 20);

            testDensityFunction(densityFunction, dimensions);
        }

        private void testDensityFunction(Func<Vector3, float> densityFunction, Point3 dimensions)
        {
            AbstractHermiteGrid grid = null;

            grid = createGridFromDensityFunction(densityFunction, dimensions);
            grid = HermiteDataGrid.CopyGrid(grid);

            showGrid(grid);
        }

        private void showGrid(AbstractHermiteGrid grid)
        {
            var testEnv = new DualContouringTestEnvironment();
            testEnv.Grid = grid;
            testEnv.AddToEngine(EngineFactory.CreateEngine());

        }

        public static AbstractHermiteGrid createGridFromDensityFunction(Func<Vector3, float> densityFunction, Point3 dimensions)
        {
            return new DensityFunctionHermiteGrid(densityFunction, dimensions);
        }

    }
}