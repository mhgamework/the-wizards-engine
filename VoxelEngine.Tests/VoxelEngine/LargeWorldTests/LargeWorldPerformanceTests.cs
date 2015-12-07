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
            var signsSize = 4;
            var signs = new byte[signsSize * signsSize * signsSize * 4];

            var builder = new DCOctreeBuilder();

            var root = builder.BuildTree(new Point3(0, 0, 0), signsSize, signs, signsSize);

            Assert.AreEqual(0, root.signs);

            signs = signs.Select(_ => (byte)255).ToArray();


            root = builder.BuildTree(new Point3(0, 0, 0), signsSize, signs, signsSize);

            Assert.AreEqual(255, root.signs);

        }

        [Test]
        public void TestBuildOctreeEmptySpeed()
        {
            EngineEnabled = false;
            var signsSize = 128;
            var signs = new byte[signsSize * signsSize * signsSize * 4];

            var builder = new DCOctreeBuilder();

            var times = 10;

            var time = PerformanceHelper.Measure( () =>
            {
                for ( int i = 0; i < times; i++ )
                {
                    var root = builder.BuildTree(new Point3(0, 0, 0), signsSize, signs, signsSize);
                    
                }

            } );
            
            
            Console.WriteLine("{0:#.0}x{0:#.0}x{0:#.0} grid to octree per second", Math.Pow(signsSize*signsSize*signsSize / (time.TotalSeconds / times), 1 / 3f));
            


        }


        /// <summary>
        /// Generate signs on GPU and convert to octrees
        /// </summary>
        [Test]
        public void TestBuildOctrees()
        {
            var leafs = getAllLeafs();

            var gpu = generateTerrainSigns(leafs);

            var builder = new DCOctreeBuilder();
            PerformanceHelper.Measure(() =>
            {
                foreach (var l in leafs)
                {
                    //TODO: scale for downsampled nodes?
                    builder.BuildTree(l.LowerLeft, l.Size, l.Signs, 16);
                }
            });

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
            public DCNode DCTree;
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