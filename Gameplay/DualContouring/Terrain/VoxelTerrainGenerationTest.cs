using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
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
    public class VoxelTerrainGenerationTest
    {

        private float gridWorldSize;
        private int subdivision;
        private float cellSize;
        private LineManager3DLines lines;

        [SetUp]
        public void Setup()
        {
            gridWorldSize = 10f;
            subdivision = 20;
            cellSize = gridWorldSize / subdivision;
            lines = new LineManager3DLines(TW.Graphics.Device);
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
            this.lines.SetMaxLines(1000000);

            DualContouringTest.addHermiteVertices(grid, cellSize, this.lines);
            DualContouringTest.addHermiteNormals(grid, cellSize, this.lines);
            DualContouringTest.addFaceNormals(grid, cellSize, this.lines);

            var lines = this.lines;


            //foreach (var v in vertices) lines.AddCenteredBox(v * cellSize, cellSize * 0.2f, Color.OrangeRed.dx());

            var mesh = DualContouringTest.buildMesh(grid);
            DualContouringTest.addQEFPoints(mesh, cellSize, this.lines);
            var el = TW.Graphics.AcquireRenderer().CreateMeshElement(mesh);
            el.WorldMatrix = Matrix.Scaling(new Vector3(cellSize));

            var engine = EngineFactory.CreateEngine();


            engine.AddSimulator(new WorldRenderingSimulator());

            DualContouringTest.addLinesSimulator(engine, lines);
            DualContouringTest.addCameraLightSimulator(engine);

            TW.Graphics.AcquireRenderer().CullMode = CullMode.Front;
        }

        public static AbstractHermiteGrid createGridFromDensityFunction(Func<Vector3, float> densityFunction, Point3 dimensions)
        {
            AbstractHermiteGrid grid = null;
            grid = new DelegateHermiteGrid(
                p => densityFunction(p) > 0,
                (p, i) =>
                {
                    var startPos = grid.GetEdgeOffsets(i)[0] + p;
                    var endPos = grid.GetEdgeOffsets(i)[1] + p;
                    // search intersection point
                    var zeroFactor = FindZeroOnLineSubdivide(densityFunction, startPos, endPos, 8);
                    var point = Vector3.Lerp(startPos, endPos, zeroFactor);
                    //var zeroPos = FindZeroOnLineLinearApprox(densityFunction, startPos, endPos);

                    //TODO: calculate normal
                    //Note: upvoid uses analytical differentiation, im going approx style
                    var normal = new Vector3();
                    float stepSize = 0.01f;
                    normal.X = densityFunction(point + Vector3.UnitX * stepSize) - densityFunction(point - Vector3.UnitX * stepSize);
                    normal.Y = densityFunction(point + Vector3.UnitY * stepSize) - densityFunction(point - Vector3.UnitY * stepSize);
                    normal.Z = densityFunction(point + Vector3.UnitZ * stepSize) - densityFunction(point - Vector3.UnitZ * stepSize);

                    normal.Normalize();
                    normal = -normal; // This is since we want it to point to the negative side of the density field, eg air
                    return new Vector4(normal, zeroFactor);

                }, dimensions);
            return grid;
        }

        /// <summary>
        /// Divide line between startpos and endpos into 'divide' pieces, and linearly approx the zeros in each piece, returning the first zero found
        /// TODO: Possible problems: multiple roots can give strange results, as a the first root is always picked. 
        ///         Also, the linear approx can be very much off in the 1/divide subpiece, so we have somewhat of a limited 1/8 precision, even for simple functions
        /// TODO: probably use a better root finder, since when i have a simple function which is not linear it provides only results up to 1/8th precision.
        /// </summary>
        /// <param name="densityFunction"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="divide"></param>
        /// <returns></returns>
        public static float FindZeroOnLineSubdivide(Func<Vector3, float> densityFunction, Point3 startPos, Point3 endPos, int divide)
        {
            var factorStep = 1f / divide;
            var diff = (endPos - startPos).ToVector3();


            for (int i = 0; i < divide; i++)
            {
                var x1 = i * factorStep;
                // If this is not exactly 1 in the end we might miss skip the exact endpoint in all calculation, but this is probably not a problem since its
                //    already an approximation
                var x2 = (i + 1) * factorStep;
                var y1 = densityFunction(startPos + x1 * diff);
                var y2 = densityFunction(startPos + x2 * diff);
                if (y1 * y2 > 0) continue; // no linear zero

                // return first zero
                return FindZeroLinear(x1, x2, y1, y2);
            }
            throw new InvalidOperationException("No zero! Should not be possible since this function should only be called if start and end have a sign difference");
        }

        /// <summary>
        /// Find the x for which y = zero assuming function is linear between p1 and p2
        /// </summary>
        /// <param name="function"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static float FindZeroLinear(float x1, float x2, float y1, float y2)
        {
            // Assume linear
            var slope = (y2 - y1) / (x2 - x1);
            if (slope < 0.001) return (x1 + x2) / 2; // Return middle when slope is almost zero
            var zero = -y1 / slope + x1;
            return zero;
        }


        /// <summary>
        /// Untested
        /// </summary>
        /// <param name="densityFunction"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        private static void FindZeroOnLineLinearApprox(Func<Vector3, float> densityFunction, Point3 startPos, Point3 endPos)
        {
            var searchStart = 0f;
            var searchEnd = 1f;
            var startDens = densityFunction(Vector3.Lerp(startPos, endPos, searchStart));
            var endDens = densityFunction(Vector3.Lerp(startPos, endPos, searchEnd));
            if (startDens * endDens > 0)
                throw new InvalidOperationException("No intersection OR linear estimation doesnt hold");
            //Idea maybe take center


            for (int j = 0; j < 5; j++)
            {
                //TODO: test interstection

                // Assume linear
                var slope = (endDens - startDens) / (searchEnd - searchStart);
                // startDens + (zeroEstimate - searchStart) * slope = 0
                var zero = -startDens / slope + searchStart;
                var zeroDens = densityFunction(Vector3.Lerp(startPos, endPos, zero));

                if (startDens * zeroDens < 0)
                {
                    // Sign difference, so take start to zero
                    searchEnd = zero;
                    endDens = zeroDens;
                }
                else if (endDens * zeroDens < 0)
                {
                    searchStart = zero;
                    startDens = zeroDens;
                }
                else
                {
                    // No sign difference??
                    throw new InvalidOperationException("No intersection OR linear estimation doesnt hold");
                }
            }
        }

        [Test]
        public void TestGenerateSinglePerlinNoise()
        {
            var densityFunction = createDensityFunction5Perlin(12);

            int size = 16*4;
            var dimensions = new Point3(size, size, size);

            testDensityFunction(densityFunction, dimensions);

        }

        public static Func<Vector3, float> createDensityFunction5Perlin(int seed)
        {
            Array3D<float> noise = generateNoise(seed);
            /*noise = new Array3D<float>(new Point3(2, 2, 2));
            noise[new Point3(1, 1, 1)] = 3;
            noise[new Point3(0, 0, 0)] = -3;*/
            var seeder = new Seeder(0);
            Func<Vector3, float> densityFunction = v =>
                {
                    var density = (float)10 - v.Y;
                    v *= 1 / 8f;
                    //v *= (1/8f);
                    density += sampleTrilinear(noise, v * 4.03f) * 0.25f;
                    density += sampleTrilinear(noise, v * 1.96f) * 0.5f;
                    density += sampleTrilinear(noise, v * 1.01f) * 1;
                    density += sampleTrilinear(noise, v * 0.55f) * 10;
                    density += sampleTrilinear(noise, v * 0.21f) * 30;
                    //density += noise.GetTiled(v.ToFloored());
                    return density;
                };
            return densityFunction;
        }

        public static float sampleTrilinear(Array3D<float> noise, Vector3 pos)
        {

            var min = pos.ToFloored();
            var f = pos - min;

            var q000 = noise.GetTiled(new Point3(0, 0, 0) + min);
            var q100 = noise.GetTiled(new Point3(1, 0, 0) + min);
            var q010 = noise.GetTiled(new Point3(0, 1, 0) + min);
            var q001 = noise.GetTiled(new Point3(0, 0, 1) + min);
            var q110 = noise.GetTiled(new Point3(1, 1, 0) + min);
            var q011 = noise.GetTiled(new Point3(0, 1, 1) + min);
            var q101 = noise.GetTiled(new Point3(1, 0, 1) + min);
            var q111 = noise.GetTiled(new Point3(1, 1, 1) + min);


            var z = triLerp(f, q000, q100, q001, q101, q010, q110, q011, q111);
            return z;


            /*return triLerp(pos.X, pos.Y, pos.Z, q000, q001, q010, q011, q100, q101, q110, q111, min.X, min.X + 1, min.Y,
                           min.Y + 1, min.Z, min.Z + 1);*/
        }

        public static float triLerp(Vector3 f, float q000, float q100, float q001, float q101, float q010, float q110,
                                     float q011, float q111)
        {
            var x00 = MathHelper.Lerp(q000, q100, f.X);
            var x01 = MathHelper.Lerp(q001, q101, f.X);
            var x10 = MathHelper.Lerp(q010, q110, f.X);
            var x11 = MathHelper.Lerp(q011, q111, f.X);

            var y0 = MathHelper.Lerp(x00, x10, f.Y);
            var y1 = MathHelper.Lerp(x01, x11, f.Y);


            var z = MathHelper.Lerp(y0, y1, f.Z);
            return z;
        }

        /*public static float lerp(float x, float x1, float x2, float q00, float q01)
        {
            return ((x2 - x) / (x2 - x1)) * q00 + ((x - x1) / (x2 - x1)) * q01;
        }

        public static float biLerp(float x, float y, float q11, float q12, float q21, float q22, float x1, float x2, float y1, float y2)
        {
            float r1 = lerp(x, x1, x2, q11, q21);
            float r2 = lerp(x, x1, x2, q12, q22);

            return lerp(y, y1, y2, r1, r2);
        }

        public static float triLerp(float x, float y, float z, float q000, float q001, float q010, float q011, float q100, float q101, float q110, float q111, float x1, float x2, float y1, float y2, float z1, float z2)
        {
            float x00 = lerp(x, x1, x2, q000, q100);
            float x10 = lerp(x, x1, x2, q010, q110);
            float x01 = lerp(x, x1, x2, q001, q101);
            float x11 = lerp(x, x1, x2, q011, q111);
            float r0 = lerp(y, y1, y2, x00, x01);
            float r1 = lerp(y, y1, y2, x10, x11);

            return lerp(z, z1, z2, r0, r1);
        }*/


        private static Array3D<float> generateNoise(int seed)
        {
            var ret = new Array3D<float>(new Point3(16, 16, 16));

            var r = new Seeder(seed);

            ret.ForEach((val, p) =>
                {
                    ret[p] = r.NextFloat(-1, 1);
                });

            return ret;
        }

        [Test]
        public void TestSaveGeneratedTerrain()
        {
            var dens = VoxelTerrainGenerationTest.createDensityFunction5Perlin(11);
            var grid = VoxelTerrainGenerationTest.createGridFromDensityFunction(dens, new Point3(64, 64, 64));
            var dataGrid = HermiteDataGrid.CopyGrid(grid);
            dataGrid.Save(TWDir.Test.CreateSubdirectory("DualContouring").CreateFile("TerrainHermite64.txt"));

        }

        [Test]
        public void TestLoadGeneratedTerrain()
        {
            var grid =
                HermiteDataGrid.LoadFromFile(
                    TWDir.Test.CreateSubdirectory("DualContouring").CreateFile("TerrainHermite64.txt"));

            showGrid(grid);
            
        }
    }
}