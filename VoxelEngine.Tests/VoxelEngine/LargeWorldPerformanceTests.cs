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
    public class LargeWorldPerformanceTests : EngineTestFixture
    {

        /// <summary>
        /// Testing performance of the terrain generation + clipmaps
        /// Goals
        ///  - Asses how fast we can generate a large 16k terrain using gpu
        ///  - Speedup GPU/CPU interaction
        ///  - Find out if we can store the data
        /// </summary>
        [Test]
        public void TestGenerateTerrainSignsForLargeWorld()
        {
            var clipmapsTree = new ClipMapsOctree<SimpleNode>();
            var leafCellSize = 64;
            var root = clipmapsTree.Create(16 * 1024 / 4, leafCellSize);


            var cameraPosition = new Vector3(root.Size / 3f);
            //cameraPosition = new Vector3(1165.446f, 1082.746f, 996.1563f);
            cameraPosition = new Vector3(1238.56f,1035.99f,1134.944f);
            clipmapsTree.UpdateQuadtreeClipmaps(root, cameraPosition, leafCellSize);
            TW.Graphics.SpectaterCamera.FarClip = 16 * 1024;
            engine.AddSimulator(() =>
            {
                //clipmapsTree.DrawLines(root, TW.Graphics.LineManager3D);
            }, "DrawClipmapsLines");

            var leafs = new List<SimpleNode>();
            clipmapsTree.VisitTopDown(root, n =>
            {
                if (n.Children == null)
                    leafs.Add(n);
            });

            var gpu = new GPUHermiteCalculator(TW.Graphics);
            var signsTex = gpu.CreateDensitySignsTexture(leafCellSize + 2);// One bigger to touch the next cell

            var cache = GPUTexture3D.CreateStaging(TW.Graphics, signsTex);

            PerformanceHelper.Measure(() =>
            {
                foreach (var l in leafs)
                {
                    //gpu.WriteHermiteSigns(leafCellSize, l.LowerLeft, new Vector3(1), "", signsTex);
                    gpu.WriteHermiteSigns(leafCellSize + 2, l.LowerLeft.Alter(v => new Vector3(v.X, v.Y, v.Z)), new Vector3(l.Size / leafCellSize), "", signsTex);

                    cache.CopyResourceFrom(signsTex);

                    l.Signs = cache.GetRawData();

                }
            }).PrettyPrint().Print();

            var voxelRender = VoxelCustomRenderer.CreateDefault(TW.Graphics);
            TW.Graphics.AcquireRenderer().AddCustomGBufferRenderer(voxelRender);

            var i = 0;
            engine.AddSimulator(() =>
            {
                if (i >= leafs.Count) return;
                var l = leafs[i];
                var grid = gpu.CreateHermiteGrid(l.Signs, leafCellSize + 2);// One bigger to touch the next cell
                var surf = voxelRender.CreateSurface(grid, Matrix.Scaling(new Vector3(l.Size / leafCellSize)) * Matrix.Translation(l.LowerLeft));
                i++;


            }, "UpdateMeshes");

            engine.AddSimulator(new WorldRenderingSimulator());
        }


        public class SimpleNode : IOctreeNode<SimpleNode>
        {
            public SimpleNode[] Children { get; set; }
            public Point3 LowerLeft { get; set; }
            public int Size { get; set; }
            public int Depth { get; set; }
            public byte[] Signs;
            public void Initialize(SimpleNode parent)
            {
            }

            public void Destroy()
            {
            }
        }
    }
}