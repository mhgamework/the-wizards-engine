using System;
using System.Collections.Generic;
using System.IO;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using MHGameWork.TheWizards.VoxelEngine.DualContouring.Generation;
using MHGameWork.TheWizards.VoxelEngine.Environments;
using MHGameWork.TheWizards.VoxelEngine.Persistence;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine
{
    
    
    public class VoxelTerrainGenerationTest : EngineTestFixture
    {

        [SetUp]
        public void Setup()
        {
        }

        private void testDensityFunction(Func<Vector3, float> densityFunction, Point3 dimensions)
        {
            AbstractHermiteGrid grid = null;

            grid = new DensityFunctionHermiteGrid(densityFunction, dimensions);
            var terrGen = PerformanceHelper.Measure(() =>
                {
                    grid = HermiteDataGrid.CopyGrid(grid);
                });



            var testEnv = new DualContouringTestEnvironment();
            testEnv.Grid = grid;

            testEnv.AdditionalText = "Terrain to Hermite: " + terrGen.PrettyPrint();


            testEnv.AddToEngine(EngineFactory.CreateEngine());

        }


        [Test]
        public void TestGeneratePerlinNoise()
        {

            int size = 16 * 4;
            var densityFunction = GenerationUtils.createDensityFunction5Perlin(112, size / 2);

            var dimensions = new Point3(size, size, size);

            testDensityFunction(densityFunction, dimensions);

        }

        [Test]
        public void TestGenerateAndSavePerlinNoise64()
        {
            int size = 16 * 4;
            var densityFunction = GenerationUtils.createDensityFunction5Perlin(112, size / 2);

            var dimensions = new Point3(size, size, size);

            var grid = new DensityFunctionHermiteGrid(densityFunction, dimensions);
            HermiteDataGrid dataGrid = null;
            var terrGen = PerformanceHelper.Measure(() =>
            {
                dataGrid = HermiteDataGrid.CopyGrid(grid);
            });

            Console.WriteLine( terrGen.PrettyPrint() );

            var p = new HermiteDataPersister();
            var fileInfo = TestDirectory.CreateFile( "PerlinNoise5-64.txt" );
            p.Save( dataGrid, fileInfo );

            HermiteDataGrid newGrid = null;

            using ( var fs = fileInfo.OpenRead() )
            {
                newGrid = (HermiteDataGrid)p.Load(fs, dim => HermiteDataGrid.Empty(dim));
            }

            var testEnv = new DualContouringTestEnvironment();
            testEnv.Grid = newGrid;

            testEnv.AddToEngine(EngineFactory.CreateEngine());

            //TODO: doesnt seem to load textures

        }


        [Test]
        public void TestSaveGeneratedTerrain()
        {
            var dens = GenerationUtils.createDensityFunction5Perlin(11, 10);
            var grid = (AbstractHermiteGrid)new DensityFunctionHermiteGrid(dens, new Point3(64, 64, 64));
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
        [Test]
        public void TestDensityBasedLOD()
        {
            TW.Graphics.SpectaterCamera.FarClip = 5000;

            var maxSampleSize = 64;
            var skipLevels = 1;


            var density = GenerationUtils.createDensityFunction5Perlin(19, maxSampleSize / 2);
            var meshBuilder = new DualContouringMeshBuilder();



            var size = maxSampleSize * (int)(Math.Pow(2, skipLevels));

            var minSize = skipLevels;
            var maxSize = Math.Log(size) / Math.Log(2);

            for (int i = minSize; i < maxSize; i++)
            {
                var currI = i;
                var multiplier = (int)(Math.Pow(2, i));
                var resolution = size / multiplier;

                var grid = HermiteDataGrid.CopyGrid(new DensityFunctionHermiteGrid(v => density(v * multiplier), new Point3(resolution, resolution, resolution)));
                var mesh = meshBuilder.buildMesh(grid);

                var el = TW.Graphics.AcquireRenderer().CreateMeshElement(mesh);
                el.WorldMatrix = Matrix.Scaling(new Vector3(multiplier)) * Matrix.Translation((size * 1.1f) * (i - skipLevels), 0, 0);


            }

            EngineFactory.CreateEngine().AddSimulator(new WorldRenderingSimulator());
        }

    }
}