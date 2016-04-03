using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using MHGameWork.TheWizards.VoxelEngine.DynamicWorld.OctreeDC;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld.Tests.OctreeDC
{
    public class DCOctreeAlgorithmTest : EngineTestFixture
    {
        private DCOctreeAlgorithm algo;

        [SetUp]
        public void Setup()
        {
            algo = new DCOctreeAlgorithm();
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

            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestComplex_SignsOnly()
        {
            var octree = createComplexOctree(17);
            var triangles = algo.GenerateTrianglesForOctree(octree);

            //TODO: check visually and add automated checking

            var b = new MeshBuilder();
            b.AddCustom(triangles.Select(p => p.dx()).ToArray());
            var mesh = b.CreateMesh();

            //TODO: do this betterly :p
            new Entity() { Mesh = mesh };

            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestComplex_SignsOnly_BIG()
        {
            var octree = createComplexOctree(64 + 1);

            Vector3[] triangles = null;

            PerformanceHelper.Measure(() =>
            {
                triangles = algo.GenerateTrianglesForOctree(octree);
            }).PrettyPrint().Print();

            //TODO: check visually and add automated checking

            var b = new MeshBuilder();
            b.AddCustom(triangles.Select(p => p.dx()).ToArray());
            var mesh = b.CreateMesh();

            //TODO: do this betterly :p
            new Entity() { Mesh = mesh };

            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestComplex_QEF()
        {
            var octree = createComplexOctree_withQEF(64 + 1);

            Vector3[] triangles = null;

            PerformanceHelper.Measure(() =>
            {
                triangles = algo.GenerateTrianglesForOctree(octree);
            }).PrettyPrint().Print();

            //TODO: check visually and add automated checking

            var b = new MeshBuilder();
            b.AddCustom(triangles.Select(p => p.dx()).ToArray());
            var mesh = b.CreateMesh();

            //TODO: do this betterly :p
            new Entity() { Mesh = mesh };

            engine.AddSimulator(new WorldRenderingSimulator());
        }



        private SignedOctreeNode createComplexOctree(int size)
        {
            var builder = new SignedOctreeBuilder();
            var signs = new Array3D<bool>(new global::DirectX11.Point3(size, size, size));
            signs.ForEach((b, p) => signs[p] = Math.Sin(p.X) + Math.Sin(p.Z) + 10 - p.Y < 0);
            var tree = builder.GenerateCompactedTreeFromSigns(signs);
            return tree;
        }

        private SignedOctreeNode createComplexOctree_withQEF(int size)
        {
            var hermiteData =
                HermiteDataGrid.CopyGrid(
                    new DensityFunctionHermiteGrid(p => (float)Math.Sin(p.X) + (float)Math.Sin(p.Z) + 10 - p.Y,
                        new global::DirectX11.Point3(size, size, size)));


            var tree = new SignedOctreeBuilder().ConvertHermiteGridToOctree( hermiteData);
            return tree;
        }

        private SignedOctreeNode createSimpleCubeOctree()
        {
            var test = new SignedOctreeBuilderTest();
            test.Setup();
            return test.CreateOctreeSmallCube();
        }
    }
}