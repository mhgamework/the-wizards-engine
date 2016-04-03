using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.VoxelEngine;
using MHGameWork.TheWizards.VoxelEngine.DynamicWorld;
using MHGameWork.TheWizards.VoxelEngine.DynamicWorld.OctreeDC;
using NUnit.Framework;

namespace MHGameWork.TheWizards.DynamicWorld
{
    /// <summary>
    /// Test demonstrating dynamic editing of world capabilties (automated)
    /// </summary>
    [TestFixture]
    public class DynamicAddingTest : EngineTestFixture
    {
        private ClipMapsOctree<SignedOctreeNode> helper = new ClipMapsOctree<SignedOctreeNode>();

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestDynamicDc()
        {
            var ent = new Entity() { Mesh = null };

            var amount = 0f;

            engine.AddSimulator(() =>
            {
                amount += TW.Graphics.Elapsed;

                var builder = new SignedOctreeBuilder();
                var size = 16;

                var center = new Vector3(1) * size * 0.5f;

                var octree = builder.ConvertHermiteGridToOctree(new DensityFunctionHermiteGrid(v =>
                {
                    var ret = v.Y - (size * 0.5f);

                    var dist = (v - center).Length();
                    var range = amount;
                    if (dist > range) return ret;

                    ret += range - dist;

                    return ret;
                }, new Point3(size + 1, size + 1, size + 1)));

                //helper.VisitDepthFirst(octree, a => { if (a != null) a.QEF = new Vector3(0.5f); });

                var algo = new DCOctreeAlgorithm();
                var triangles = algo.GenerateTrianglesForOctree(octree);

                //TODO: check visually and add automated checking

                var b = new MeshBuilder();
                b.AddCustom(triangles.Select(p => p.dx()).ToArray());
                ent.Visible = false;
                ent.Mesh = null;
                ent = new Entity();

                ent.Mesh = b.CreateMesh();
            }, "grow");


            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestPlane()
        {

            var builder = new SignedOctreeBuilder();
            var size = 128;
            var octree = builder.ConvertHermiteGridToOctree(new DensityFunctionHermiteGrid(v => v.Y - (size * 0.5f),
                    new Point3(size + 1, size + 1, size + 1)));

            //helper.VisitDepthFirst(octree, a => { if (a != null) a.QEF = new Vector3(0.5f); });

            var algo = new DCOctreeAlgorithm();
            var triangles = algo.GenerateTrianglesForOctree(octree);

            //TODO: check visually and add automated checking

            var b = new MeshBuilder();
            b.AddCustom(triangles.Select(p => p.dx()).ToArray());
            var mesh = b.CreateMesh();

            new Entity() { Mesh = mesh };

            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestDigWithDensity()
        {

            var ent = new Entity() { Mesh = null };

            var amount = 0f;

            engine.AddSimulator(() =>
            {
                amount += TW.Graphics.Elapsed;

                var builder = new SignedOctreeBuilder();
                var size = 16;

                var center = new Vector3(1) * size * 0.5f;

                var octree = builder.ConvertHermiteGridToOctree(new DensityFunctionHermiteGrid(v =>
                {
                    var ret = v.Y - (size * 0.5f);

                    var dist = (v - center).Length();
                    var range = amount;
                    if (dist > range) return ret;

                    ret += range - dist;

                    return ret;
                }, new Point3(size + 1, size + 1, size + 1)));

                //helper.VisitDepthFirst(octree, a => { if (a != null) a.QEF = new Vector3(0.5f); });

                var algo = new DCOctreeAlgorithm();
                var triangles = algo.GenerateTrianglesForOctree(octree);

                //TODO: check visually and add automated checking

                var b = new MeshBuilder();
                b.AddCustom(triangles.Select(p => p.dx()).ToArray());
                ent.Visible = false;
                ent.Mesh = null;
                ent = new Entity();
                
                ent.Mesh = b.CreateMesh();
            }, "grow");


            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestExtend()
        {
            var signedOctree = new SignedOctreeNode();

            helper.Create(128, 8, 1);






        }
    }
}