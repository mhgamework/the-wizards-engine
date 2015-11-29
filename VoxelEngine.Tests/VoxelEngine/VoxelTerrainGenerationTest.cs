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
            var densityFunction = createDensityFunction5Perlin(112, size / 2);

            var dimensions = new Point3(size, size, size);

            testDensityFunction(densityFunction, dimensions);

        }

        [Test]
        public void TestGenerateAndSavePerlinNoise64()
        {
            int size = 16 * 4;
            var densityFunction = createDensityFunction5Perlin(112, size / 2);

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

        public static Func<Vector3, float> createDensityFunction5Perlin(int seed, int height)
        {
            Array3D<float> noise = generateNoise(seed);
            /*noise = new Array3D<float>(new Point3(2, 2, 2));
            noise[new Point3(1, 1, 1)] = 3;
            noise[new Point3(0, 0, 0)] = -3;*/
            var seeder = new Seeder(0);
            var sampler = new Array3DSampler<float>();
            Func<Vector3, float> densityFunction = v =>
                {
                    var density = (float)height - v.Y;
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


        public static Array3D<float> generateNoise(int seed)
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
            var dens = createDensityFunction5Perlin(11, 10);
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


            var density = createDensityFunction5Perlin(19, maxSampleSize / 2);
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

        [Test]
        public void TestLodOctree()
        {
            var tree = new LodOctree<LodOctreeNode>();
            var root = tree.Create(32 * 4, 32);

            var engine = EngineFactory.CreateEngine();
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(() =>
                {
                    tree.DrawLines(root, TW.Graphics.LineManager3D);
                }, "LinesOctree");

        }

        [Test]
        public void TestLodEnvironment()
        {
            var env = new TerrainLodEnvironment();
            env.LoadIntoEngine(EngineFactory.CreateEngine());
        }

        [Test]
        public void TestGenerateLodTerrainMeshes()
        {
            var tree = new LodOctree<LodOctreeNode>();
            var size = 128;
            var root = tree.Create(size, size);
            tree.UpdateQuadtreeClipmaps(root, new Vector3(size / 2, size / 2, size / 2), 8);

            var density = new Func<Vector3, float>(v => DensityHermiteGridTest.SineXzDensityFunction(v, 1 / 5f, size / 2, 3));
            var builder = new LodOctreeMeshBuilder();
            var list = new List<LodOctreeNode>();
            builder.ListMeshLessNodes(root, list);
            list.ForEach(n =>
                {
                    if (n.Children != null) return;
                    var mesh = builder.CalculateNodeMesh(n, 8, density);
                    n.Mesh = mesh; // This is flakey

                    builder.CreateRenderElementForNode(n, 8, mesh);
                });

            EngineFactory.CreateEngine().AddSimulator(() => { tree.DrawLines(root, TW.Graphics.LineManager3D); }, "Octreelines");

            VisualTestingEnvironment.Load();
        }
    }
}