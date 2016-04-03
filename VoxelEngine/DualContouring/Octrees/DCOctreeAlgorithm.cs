using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DirectX11;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld.OctreeDC
{
    /// <summary>
    /// Finds minimal edges of an octree as described in the dual contouring paper
    /// Optimization ideas: http://www.dotnetperls.com/optimization
    ///  - make all static (no vtables)
    ///  - remove nested lookup table arrays => halves lookup memory accesses
    ///  - minimize parameter passing between methods somehow?
    /// </summary>
    public class DCOctreeAlgorithm
    {
        private MinimalEdgeTraverser edgeTraverser;

        private List<Vector3> points = new List<Vector3>();
        private int[] quadVertices = new[] { 0, 1, 2, 2, 3, 0 };

        public DCOctreeAlgorithm()
        {
            edgeTraverser = new MinimalEdgeTraverser(processEdge);
        }

        private void processEdge(SignedOctreeNode[] nodes, int dir)
        {
            //implementation of Octree::processEdgeWrite here, check if crossing edge etc.


            // Find minimal(deepest) cell 
            var minimalCell = nodes[0];
            var minimalCellIndex = 0;
            for (int i = 1; i < 4; i++)
            {
                var n = nodes[i];
                if (minimalCell.Depth <= n.Depth) continue;
                minimalCell = n;
                minimalCellIndex = i;
            }

            // Figure out which verts this edge belongs to
            var vertIds = edgeTraverser.getVertIdsForEdge(dir, minimalCellIndex);

            var s0 = minimalCell.Signs[vertIds[0]];
            var s1 = minimalCell.Signs[vertIds[1]];

            if (s0 == s1) return; // not crossing

            if (s0 == false) // TODO: check flip direction
            {
                //Flip
                var swap = nodes[1];
                nodes[1] = nodes[3];
                nodes[3] = swap;

            }

            for (int index = 0; index < quadVertices.Length; index++)
            {
                var i = quadVertices[index];
                var n = nodes[i];
                points.Add(n.LowerLeft + n.QEF * n.Size);
            }
        }


        public Vector3[] GenerateTrianglesForOctree(SignedOctreeNode tree, int maxDepth = int.MaxValue)
        {
            points.Clear();
            edgeTraverser.writeQuadsForCell(tree, maxDepth);

            return points.ToArray();
        }


    }
}