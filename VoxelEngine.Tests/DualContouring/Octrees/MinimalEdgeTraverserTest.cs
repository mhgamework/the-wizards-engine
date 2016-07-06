using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.VoxelEngine.DynamicWorld.OctreeDC;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld.Tests.OctreeDC
{
    /// <summary>
    /// Tests my implementation of minimal edge traversal in the DC paper (cellproc,faceproc and edgeproc)
    /// </summary>
    public class MinimalEdgeTraverserTest
    {
        private List<SignedOctreeNode[]> edgeNodes;
        private List<int> edgeDirs;
        private MinimalEdgeTraverser algo;

        [SetUp]
        public void Setup()
        {
            edgeNodes = new List<SignedOctreeNode[]>();
            edgeDirs = new List<int>();

            algo = new MinimalEdgeTraverser((nodes, dir) =>
            {
                edgeNodes.Add(nodes.ToArray());
                edgeDirs.Add(dir);
            });
            //TODO: Write a set of tests that validates the mapping set
            // Do this by validating some properties of the algorithm.
            // On a fully divided tree:
            //  - Each edge visited once: check total edge count + check each edge unique + check count of edges with each dir
            //  - Each leaf node should appear in 12 edges, except for the border nodes
            //  - Edge nodes should only be leafs

            // It might be a good idea to first compare them a bit to the c++ impl mapping
            //  can be done by quickly printing them?
            // Edit: probably not usefull since the octree child order is different


        }

        [Test]
        public void TestMinimalEdgeTraversal_Cell_Depth0()
        {
            var tree = createFullyDividedOctree(0);
            algo.writeQuadsForCell(tree);

            assertEdgeConstraints(0);

        }

        [Test]
        public void TestMinimalEdgeTraversal_Cell_Depth1()
        {
            var tree = createFullyDividedOctree(1);
            algo.writeQuadsForCell(tree);

            assertEdgeConstraints(6);

        }
        [Test]
        public void TestMinimalEdgeTraversal_Cell_Depth2()
        {
            var tree = createFullyDividedOctree(2);
            algo.writeQuadsForCell(tree);

            assertEdgeConstraints(9 * 4 * 3);

        }
        [Test]
        public void TestMinimalEdgeTraversal_Cell_Depth3()
        {
            var tree = createFullyDividedOctree(3);
            algo.writeQuadsForCell(tree);

            assertEdgeConstraints((7 * 7) * 8 * 3);

        }
        /*[Test]
        public void TestMinimalEdgeTraversal_Cell_Depth4()
        {
            var tree = createFullyDividedOctree(4);
            algo.writeQuadsForCell(tree);

            //assertEdgeConstraints((15 * 15) * 16 * 3);

        }*/
        [Test]
        public void TestMinimalEdgeTraversal_Face([Values(0, 1, 2)] int cDir)
        {
            var tree = createFullyDividedOctree(2);
            var p1 = new Point3(0, 0, 0);
            var p2 = p1;
            p2[cDir] = 1;
            algo.writeQuadsForFace(new[] { 
                tree.Children[SignedOctreeNode.ChildOffsets.IndexOf(p1)], 
                tree.Children[SignedOctreeNode.ChildOffsets.IndexOf(p2)] }, cDir);

            var expectedDirCount = new[] { 2, 2, 2 };
            expectedDirCount[cDir] = 0;
            assertEdgeConstraints(4, expectedDirCount);

        }

        private void assertEdgeConstraints(int expectedEdgeCount)
        {
            assertEdgeConstraints(expectedEdgeCount,
                new[] { expectedEdgeCount / 3, expectedEdgeCount / 3, expectedEdgeCount / 3 });
        }
        private void assertEdgeConstraints(int expectedEdgeCount, int[] expectedDirCount)
        {
            // Each edge should be visited once
            Assert.AreEqual(expectedEdgeCount, edgeNodes.Count);

            // Each dir should be visited edgecount/3 times
            var dirCounts = Enumerable.Range(0, 3).Select(dir => edgeDirs.Count(d => d == dir)).ToArray();
            for (int dir = 0; dir < 3; dir++)
                Assert.AreEqual(expectedDirCount[dir], dirCounts[dir]);

            // Each sequential pair of nodes of a minimal edge should be adjacent children
            for (int iEdge = 0; iEdge < edgeNodes.Count; iEdge++)
            {
                var nodes = edgeNodes[iEdge];
                var dir = edgeDirs[iEdge];
                // The coordinate on the dir axis should be the same for each child
                var fixedCoord = nodes[0].LowerLeft[dir];
                for (int i = 0; i < 4; i++)
                {
                    // We are in a fully subdivided tree, so children should be same level
                    var n1 = nodes[i];
                    var n2 = nodes[(i + 1) % 4];
                    Assert.AreEqual(n1.Depth, n2.Depth);

                    // nodes should be a circular sequence!
                    Assert.AreEqual(n1.Size, Vector3.Distance(n1.LowerLeft, n2.LowerLeft), 0.001f,
                        "Edge " + edgeNodes.IndexOf(nodes));

                    // fixed coordinate check
                    Assert.AreEqual(fixedCoord, n1.LowerLeft[dir]);
                }

                // Should not have same edge twice
                foreach (var secondEdge in edgeNodes)
                {
                    if (secondEdge == nodes) continue;
                    CollectionAssert.AreNotEquivalent(nodes, secondEdge);
                }
            }
        }


        [Test]
        public void TestEdgeToVertices([Values(0,1,2,3,4,5)] int cellEdgeId)
        {
            var tree = createFullyDividedOctree(1);
            var edge = MinimalEdgeTraverser.cellToEdges[cellEdgeId];


            var p = new Point3();

            for (int i = 0; i < 4; i++)
            {

                var n = tree.Children[edge.Children[i]];

                var vertIds = MinimalEdgeTraverser.getVertIdsForEdge(edge.dir, i);


                var v0 = SignedOctreeNode.SignOffsets[vertIds[0]];
                var v1 = SignedOctreeNode.SignOffsets[vertIds[1]];


                Assert.AreEqual(0, v0[edge.dir]);
                Assert.AreEqual(1, v1[edge.dir]);
                Assert.AreEqual(v0[(edge.dir + 1) % 3], v1[(edge.dir + 1) % 3]);
                Assert.AreEqual(v0[(edge.dir + 2) % 3], v1[(edge.dir + 2) % 3]);


                var worldV = v0 * n.Size + n.LowerLeft;

                if (i == 0) p = worldV;
                else
                    Assert.AreEqual(p, worldV);



            }

        }

        private SignedOctreeNode createFullyDividedOctree(int depth)
        {
            var helper = new ClipMapsOctree<SignedOctreeNode>();
            return helper.Create(1024, 1024 >> depth);
        }


        private SignedOctreeNode createSimpleCubeOctree()
        {
            var test = new SignedOctreeBuilderTest();
            return test.CreateOctreeSmallCube();
        }
    }
}