using System.Linq;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.VoxelEngine.DynamicWorld.OctreeDC;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld.Tests.OctreeDC
{
    public class OctreeDCAlgorithmTest : EngineTestFixture
    {
        private OctreeDCAlgorithm algo;

        [SetUp]
        public void Setup()
        {
            algo = new OctreeDCAlgorithm();
        }

        [Test]
        public void TestSimpleCube_SignsOnly()
        {
            var octree = createSimpleCubeOctree();
            var triangles = algo.GenerateTrianglesForOctree(octree);

            //TODO: check visually and add automated checking

            var b = new MeshBuilder();
            b.AddCustom(triangles.Select(p => p.dx()).ToArray());
            var mesh = b.CreateMesh();

            //TODO: do this betterly :p
            new Entity() { Mesh = mesh };
        }


        private SignedOctreeNode createSimpleCubeOctree()
        {
            var test = new SignedOctreeBuilderTest();
            return test.CreateOctreeSmallCube();
        }
    }
}