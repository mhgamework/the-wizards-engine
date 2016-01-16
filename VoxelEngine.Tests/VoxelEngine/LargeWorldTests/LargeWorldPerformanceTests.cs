using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.GPU;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.VoxelEngine.EngineServices;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine
{
    /// <summary>
    /// Testing performance of the terrain generation + clipmaps
    /// Goals
    ///  - Asses how fast we can generate a large 16k terrain using gpu
    ///  - Speedup GPU/CPU interaction
    ///  - Find out if we can store the data
    ///  - Benchmark CPU based octree-gen speed
    /// </summary>
    public class LargeWorldPerformanceTests : EngineTestFixture
    {
        private int minNodeSize = 16;
        private ClipMapsOctree<SimpleNode> clipmapsTree;
        private SimpleNode root;
        private VoxelCustomRenderer voxelRender;


        [SetUp]
        public void Setup()
        {
            clipmapsTree = new ClipMapsOctree<SimpleNode>();
            root = clipmapsTree.Create(16 * 1024 / 4, minNodeSize);

            var cameraPosition = new Vector3(root.Size / 3f);
            //cameraPosition = new Vector3(1165.446f, 1082.746f, 996.1563f);
            cameraPosition = new Vector3(1238.56f, 1035.99f, 1134.944f);
            clipmapsTree.UpdateQuadtreeClipmaps(root, cameraPosition, minNodeSize);
            TW.Graphics.SpectaterCamera.FarClip = 16 * 1024;
            engine.AddSimulator(() =>
            {
                //clipmapsTree.DrawLines(root, TW.Graphics.LineManager3D);
            }, "DrawClipmapsLines");

            voxelRender = VoxelCustomRenderer.CreateDefault(TW.Graphics);
            TW.Graphics.AcquireRenderer().AddCustomGBufferRenderer(voxelRender);
            engine.AddSimulator(new WorldRenderingSimulator());
        }


        /// <summary>
        /// Generate signs on gpu and generate surfaces using regular grid dc
        /// </summary>
        [Test]
        public void TestGenerateTerrainSignsForLargeWorld()
        {
            var leafs = getAllLeafs();

            var gpu = generateTerrainSigns(leafs);

            addSimulator_GenerateDensityToSurfaceOnePerFrame(leafs, gpu);
        }

        [Test]
        public void TestBuildOctree()
        {
            throw new NotImplementedException(); // Replaced by SignedOctreeBuilderTest
            /*var signsSize = 4;
            var signs = new byte[signsSize * signsSize * signsSize * 4];

            var builder = new SignedOctreeBuilder();

            var root = builder.BuildTree(new Point3(0, 0, 0), signsSize, signs, signsSize);

            Assert.AreEqual(0, root.signs);

            signs = signs.Select(_ => (byte)255).ToArray();


            root = builder.BuildTree(new Point3(0, 0, 0), signsSize, signs, signsSize);

            Assert.AreEqual(255, root.signs);*/

        }

        [Test]
        public void TestBuildOctreeEmptySpeed()
        {
            EngineEnabled = false;
            var signsSize = 128;
            var signs = new byte[signsSize * signsSize * signsSize * 4];

            var builder = new SignedOctreeBuilder();

            var times = 3;

            var time = PerformanceHelper.Measure(() =>
            {
                for (int i = 0; i < times; i++)
                {
                    throw new NotImplementedException();
                    //var root = builder.BuildTree(new Point3(0, 0, 0), signsSize, signs, signsSize);

                }

            });


            printGridSpeed("BuildOctreeEmptySpeed", time, times, signsSize);


        }

        [Test]
        public void TestBuildOctreeRandomSpeed()
        {
            EngineEnabled = false;
            var signsSize = 128;
            var signs = new byte[signsSize * signsSize * signsSize * 4];
            var r = new Random();
            r.NextBytes( signs );
            var builder = new SignedOctreeBuilder();

            var times = 3;

            var time = PerformanceHelper.Measure(() =>
            {
                for (int i = 0; i < times; i++)
                {
                    throw new NotImplementedException();
                    //var root = builder.BuildTree(new Point3(0, 0, 0), signsSize, signs, signsSize);

                }

            });


            printGridSpeed("BuildOctreeEmptySpeed", time, times, signsSize);


        }
        [Test]
        public void TestBuildOctreeBottomUpScanning()
        {
            EngineEnabled = false;
            var signsSize = 128;
            var signs = new byte[signsSize * signsSize * signsSize * 4];

            var builder = new SignedOctreeBuilder();

            var times = 1;

            var time = PerformanceHelper.Measure(() =>
            {
                for (int i = 0; i < times; i++)
                {
                    throw new NotImplementedException();
                    //var root = builder.BuildTreeBottomUpScanning(new Point3(0, 0, 0), signsSize, signs, signsSize);

                }

            });


            printGridSpeed("BottomUpScanning", time, times, signsSize);
        }

        private void printGridSpeed(string name, TimeSpan time, int times, int signsSize)
        {
            Console.WriteLine(name + ": {0}", secPerGridToGridPerSec(time.TotalSeconds / times, signsSize));
            Console.WriteLine(name + ": {0} sec per voxel {1}^3", time.Multiply(1.0 / times).PrettyPrint(), signsSize);
        }


        /// <summary>
        /// Generate signs on GPU and convert to octrees
        /// </summary>
        [Test]
        public void TestBuildOctrees()
        {
            EngineEnabled = false;
            var leafs = getAllLeafs();

            var gpu = generateTerrainSigns(leafs);

            var builder = new SignedOctreeBuilder();
            PerformanceHelper.Measure(() =>
            {
                foreach (var l in leafs)
                {
                    //TODO: scale for downsampled nodes?
                    //builder.BuildTree(l.LowerLeft, l.Size, l.Signs, 16);
                    throw new NotImplementedException();
                    //builder.BuildTree(l.LowerLeft, minNodeSize, l.Signs, minNodeSize);

                }
            });

        }

        [Test]
        public void TestOrAllSigns()
        {
            EngineEnabled = false;

            var size = 128;
            var signs = new byte[size * size * size];
            int acc = 0;
            var times = 10;
            var time = PerformanceHelper.Measure(() =>
            {
                for (int j = 0; j < times; j++)
                {
                    for (int i = 0; i < signs.Length; i++)
                    {
                        //if ( signs[ i ] == 0 ) acc++;
                        acc = (byte)(acc | signs[i]);
                    }
                }

            });

            Console.WriteLine("Speed: {0}", secPerGridToGridPerSec(time.TotalSeconds / times, size));

        }

        private object secPerGridToGridPerSec(double totalSeconds, int size)
        {
            return String.Format("{0:#.0}x{0:#.0}x{0:#.0} grid per second", Math.Pow(size * size * size / totalSeconds, 1 / 3f));
        }


        private void addSimulator_GenerateDensityToSurfaceOnePerFrame(List<SimpleNode> leafs, GPUHermiteCalculator gpu)
        {
            var i = 0;
            engine.AddSimulator(() =>
            {
                if (i >= leafs.Count) return;
                var l = leafs[i];
                var grid = gpu.CreateHermiteGrid(l.Signs, minNodeSize + 2); // One bigger to touch the next cell
                var surf = voxelRender.CreateSurface(grid,
                    Matrix.Scaling(new Vector3(l.Size / minNodeSize)) * Matrix.Translation(l.LowerLeft));
                i++;
            }, "UpdateMeshes");
        }

        private GPUHermiteCalculator generateTerrainSigns(List<SimpleNode> leafs)
        {
            var gpu = new GPUHermiteCalculator(TW.Graphics, "getDensityTerrain");
            //var gpu = new GPUHermiteCalculator( TW.Graphics, "getDensityCaves" );
            var signsTex = gpu.CreateDensitySignsTexture(minNodeSize + 2); // One bigger to touch the next cell

            var cache = GPUTexture3D.CreateStaging(TW.Graphics, signsTex);


            PerformanceHelper.Measure(() =>
            {
                foreach (var l in leafs)
                {
                    //gpu.WriteHermiteSigns(leafCellSize, l.LowerLeft, new Vector3(1), "", signsTex);
                    gpu.WriteHermiteSigns(minNodeSize + 2, l.LowerLeft.Alter(v => new Vector3(v.X, v.Y, v.Z)),
                        new Vector3(l.Size / minNodeSize), "", signsTex);

                    cache.CopyResourceFrom(signsTex);

                    l.Signs = cache.GetRawData();
                }
            }).PrettyPrint().Print("Time to calculate density signs on GPU: {0}");
            return gpu;
        }

        private List<SimpleNode> getAllLeafs()
        {
            var leafs = new List<SimpleNode>();
            clipmapsTree.VisitTopDown(root, n =>
            {
                if (n.Children == null)
                    leafs.Add(n);
            });
            return leafs;
        }


        public class SimpleNode : IOctreeNode<SimpleNode>
        {
            //public DCNode DCTree;
            /// <summary>
            /// This is x,y,z *4, with the first of 4 either 0 or 255
            /// </summary>
            public byte[] Signs;





            public SimpleNode[] Children { get; set; }
            public Point3 LowerLeft { get; set; }
            public int Size { get; set; }
            public int Depth { get; set; }

            public void Initialize(SimpleNode parent)
            {
            }

            public void Destroy()
            {
            }
        }


    }
}