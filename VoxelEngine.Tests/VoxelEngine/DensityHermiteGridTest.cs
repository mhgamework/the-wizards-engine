using System;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.VoxelEngine.Environments;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine
{


    [EngineTest]

    public class DensityHermiteGridTest : EngineTestFixture
    {
        public DensityHermiteGridTest()
        {

        }

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
            Func<Vector3, float> densityFunction = p => PlaneDensityFunction(p, 9.5f);

            var dimensions = new Point3(20, 20, 20);
            testDensityFunction(densityFunction, dimensions);


        }

        public static float PlaneDensityFunction(Vector3 p, float height)
        {
            return height - p.Y;
        }

        [Test]
        public void TestSlopeDensityFunction()
        {
            Func<Vector3, float> densityFunction = SlopeDensityFunction;

            var dimensions = new Point3(20, 20, 20);
            testDensityFunction(densityFunction, dimensions);
        }

        public static float SlopeDensityFunction(Vector3 p)
        {
            return 2.5f - p.Y + p.X;
        }

        /// <summary>
        /// Should test point and normal calculation for flat, since the plane is at 9.5 in between the 9th and 10th point
        /// </summary>
        [Test]
        public void TestSineDensityFunction()
        {
            Func<Vector3, float> densityFunction = p => SineXzDensityFunction(p, 0.5f, 9.5f, 1);

            var dimensions = new Point3(20, 20, 20);

            testDensityFunction(densityFunction, dimensions);
        }

        public static float SineXzDensityFunction(Vector3 p, float waveLengthMultiplier, float height, float amplitude)
        {
            var dens = height - p.Y;
            dens += amplitude * (float)Math.Sin((p.X + p.Z) * waveLengthMultiplier);
            return dens;
        }


        [Test]
        public void TestSphereDensityFunction()
        {
            Func<Vector3, float> densityFunction = p => SphereDensityFunction(p, 5, new Vector3(10, 10, 10));

            var dimensions = new Point3(20, 20, 20);

            testDensityFunction(densityFunction, dimensions);
        }

        public static float SphereDensityFunction(Vector3 p, int radius, Vector3 center)
        {
            return -Vector3.Distance(p, center) + radius;
        }

        [Test]
        public void TestDiamondDensityFunction()
        {
            Func<Vector3, float> densityFunction = p => DiamondDensityFunction(p, new Vector3(10, 10, 10), 8);

            var dimensions = new Point3(20, 20, 20);

            testDensityFunction(densityFunction, dimensions);
        }

        public static float DiamondDensityFunction(Vector3 p, Vector3 center, int radius)
        {
            var diff = p - center;
            var l = Math.Abs(diff.X) + Math.Abs(diff.Y) + Math.Abs(diff.Z);

            return radius - l;
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
            engine.AddSimulator(() => TW.Graphics.LineManager3D.AddBox(new BoundingBox(new Vector3(0), grid.Dimensions.ToVector3() * testEnv.CellSize), Color.Black), "BoundingBox");

            testEnv.AddToEngine(engine);

        }

        public static AbstractHermiteGrid createGridFromDensityFunction(Func<Vector3, float> densityFunction, Point3 dimensions)
        {
            return new DensityFunctionHermiteGrid(densityFunction, dimensions);
        }

    }
}