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
    
    public class LodTest : EngineTestFixture
    {

        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Gen same terrain area at lower resolution
        /// </summary>
        [Test]
        public void TestGenerateTerrainDifferentResolutions()
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

        /// <summary>
        /// Very basic octree drawing test?
        /// </summary>
        [Test]
        public void TestLodOctree()
        {
            var tree = new ClipMapsOctree<LodOctreeNode>();
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
            var env = new MultithreadedTerrainLodDemo();
            env.LoadIntoEngine(EngineFactory.CreateEngine());
        }

        [Test]
        public void TestGenerateLodTerrainMeshes()
        {
            var tree = new ClipMapsOctree<LodOctreeNode>();
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