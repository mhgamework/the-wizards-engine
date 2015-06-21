﻿using System;
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
    public class VoxelTerrainGenerationTest
    {

        [SetUp]
        public void Setup()
        {
        }

        private void testDensityFunction(Func<Vector3, float> densityFunction, Point3 dimensions)
        {
            AbstractHermiteGrid grid = null;

            grid = new DensityFunctionHermiteGrid(densityFunction, dimensions);
            grid = HermiteDataGrid.CopyGrid(grid);

            var testEnv = new DualContouringTestEnvironment();
            testEnv.Grid = grid;
            testEnv.AddToEngine(EngineFactory.CreateEngine());
        }


        [Test]
        public void TestGenerateSinglePerlinNoise()
        {
            var densityFunction = createDensityFunction5Perlin(12);

            int size = 16 * 4;
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
            var sampler = new Array3DSampler<float>();
            Func<Vector3, float> densityFunction = v =>
                {
                    var density = (float)10 - v.Y;
                    v *= 1 / 8f;
                    //v *= (1/8f);
                    density += sampler.sampleTrilinear(noise, v * 4.03f) * 0.25f;
                    density += sampler.sampleTrilinear(noise, v * 1.96f) * 0.5f;
                    density += sampler.sampleTrilinear(noise, v * 1.01f) * 1;
                    density += sampler.sampleTrilinear(noise, v * 0.55f) * 10;
                    density += sampler.sampleTrilinear(noise, v * 0.21f) * 30;
                    //density += noise.GetTiled(v.ToFloored());
                    return density;
                };
            return densityFunction;
        }


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
            var dens = createDensityFunction5Perlin(11);
            var grid = (AbstractHermiteGrid) new DensityFunctionHermiteGrid(dens, new Point3(64, 64, 64));
            var dataGrid = HermiteDataGrid.CopyGrid(grid);
            dataGrid.Save(TWDir.Test.CreateSubdirectory("DualContouring").CreateFile("TerrainHermite64.txt"));

        }

        [Test]
        public void TestLoadGeneratedTerrain()
        {
            var grid =
                HermiteDataGrid.LoadFromFile(
                    TWDir.Test.CreateSubdirectory("DualContouring").CreateFile("TerrainHermite64.txt"));

            var testEnv = new DualContouringTestEnvironment();
            testEnv.Grid = grid;
            testEnv.AddToEngine(EngineFactory.CreateEngine());
        }
    }
}