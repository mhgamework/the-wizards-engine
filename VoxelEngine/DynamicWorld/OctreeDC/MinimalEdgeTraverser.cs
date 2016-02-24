using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DirectX11;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld.OctreeDC
{
    /// <summary>
    /// Finds minimal edges of an octree as described in the dual contouring paper
    /// </summary>
    public class MinimalEdgeTraverser
    {
        private CellToFace[] cellToFaces;
        private CellToEdge[] cellToEdges;
        private FaceToFace[][] faceToFaces; // Dir, subfaceindex
        private FaceToEdge[][] faceToEdges; // Dir, subEdgeIndex
        private EdgeToEdge[][] edgeToEdges; // Dir, subEdgeIndex
        private EdgeToVertex[][] edgeToVertices; // Dir, edgenodeId
        private Action<SignedOctreeNode[], int> processMinimalEdge;

        public MinimalEdgeTraverser(Action<SignedOctreeNode[], int> processMinimalEdge)
        {
            this.processMinimalEdge = processMinimalEdge;
            setupLookupTables();
        }


        private void setupLookupTables()
        {
            var range2 = Enumerable.Range(0, 2).ToArray();
            var dirs = Enumerable.Range(0, 3).ToArray();
            cellToFaces =
                (
                from dir in dirs
                from a in range2
                from b in range2
                select new CellToFace
                    {
                        dir = dir,
                        Children = new[] { offsetToChildIndex(dir, 0, a, b), offsetToChildIndex(dir, 1, a, b) }
                    }
                ).ToArray();


            cellToEdges =
                (
                from dir in dirs
                from a in range2
                select new CellToEdge
                {
                    dir = dir,
                    Children = new[]
                    {
                        // Note that this must be the same order as the edgestoedges
                        offsetToChildIndex(dir, a, 0, 0), 
                        offsetToChildIndex(dir, a, 0, 1),
                        offsetToChildIndex(dir, a, 1, 1), 
                        offsetToChildIndex(dir, a, 1, 0)
                    }

                }
                ).ToArray();

            faceToFaces = dirs.Select(
                dir => (from a in range2
                        from b in range2
                        select new FaceToFace
                        {
                            ChildForNode = new[] { offsetToChildIndex(dir, 1, a, b), offsetToChildIndex(dir, 0, a, b) } // right side of left node to left side of right node
                        }).ToArray()
                    ).ToArray();

            faceToEdges = dirs.Select(
                dir => (from edgeDir in range2
                        from edgeHalveIndex in range2
                        select new FaceToEdge
                        {
                            // edgeDir indicates whether the dir of the edge is the first or second coordinate after dir
                            Dir = (dir + 1 + edgeDir) % 3,

                            // For this mapping, the (dir+1+edgeDir) coordinate is equal to the edgeHalveIndex
                            // The next two coordinates, in order, should be the opposite of 00 01 11 10 => 11 10 00 01
                            // Now depending on whether edgeDir = 0 or 1, the Node array order should be different

                            // Figured out the order part from the DC code!
                            // The Node array corresponds to the dir coordinate. So to match 00 01 11 10: 
                            Node = edgeDir == 1 ? new[] { 0, 0, 1, 1 } : new[] { 0, 1, 1, 0 },
                            // The 4 edgenodes must be 00 01 11 10. But since we are picking from Node in the dir coordinate,
                            //  So, if the coord is the dir coordinate, invert the 00 01 11 10  
                            Child = new[]
                            {
                                offsetToChildIndex(dir + 1 + edgeDir,edgeHalveIndex,edgeDir == 0 ? 0:1,edgeDir == 1 ? 0:1),
                                offsetToChildIndex(dir + 1 + edgeDir,edgeHalveIndex,edgeDir == 0 ? 0:1,edgeDir == 1 ? 1:0),
                                offsetToChildIndex(dir + 1 + edgeDir,edgeHalveIndex,edgeDir == 0 ? 1:0,edgeDir == 1 ? 1:0),
                                offsetToChildIndex(dir + 1 + edgeDir,edgeHalveIndex,edgeDir == 0 ? 1:0,edgeDir == 1 ? 0:1),
                                //offsetToChildIndex(dir, 1,edgeDir ==0 ? edgeHalveIndex : 0, edgeDir ==1 ? edgeHalveIndex : 0),
                                //offsetToChildIndex(dir, 1,edgeDir ==0 ? edgeHalveIndex : 1, edgeDir ==1 ? edgeHalveIndex : 1),
                                //offsetToChildIndex(dir, 0,edgeDir ==0 ? edgeHalveIndex : 1, edgeDir ==1 ? edgeHalveIndex : 1),
                                //offsetToChildIndex(dir, 0,edgeDir ==0 ? edgeHalveIndex : 0, edgeDir ==1 ? edgeHalveIndex : 0),
                            }

                        }).ToArray()
                ).ToArray();


            edgeToEdges = dirs.Select(
                dir => (
                    from edgeHalveIndex in range2
                    select new EdgeToEdge
                    {
                        // This is the tricky part, as here the 4 input nodes coordinates are unknown
                        // So we must assume their relative coordinates are correctly set, both by the cellrpoc, Faceproc and edgeproc
                        // I decided to have the edgeproc as reference (so the next few lines are the reference order)
                        // Node 1-4: 0 0, 0 1, 1 1, 1 0. This are the values for the coordinates after the 'dir-coordinate' in the node offsets 
                        // For the upperleft node, we need the lowerleft child, so invert these coordinaets for the mapping that follows
                        ChildForNode = new[]
                            {
                                offsetToChildIndex(dir, edgeHalveIndex,1,1),
                                offsetToChildIndex(dir, edgeHalveIndex,1,0),
                                offsetToChildIndex(dir, edgeHalveIndex,0,0),
                                offsetToChildIndex(dir, edgeHalveIndex,0,1),
                            }

                    }).ToArray()
                    ).ToArray();


            // 00 01 11 10
            int[] nc0 = new[] { 0, 0, 1, 1 };
            int[] nc1 = new[] { 0, 1, 1, 0 };

            edgeToVertices = dirs.Select(
                dir => (from nodeIndex in Enumerable.Range(0, 4)
                        select new EdgeToVertex
                        {
                            // Again with the 00 01 11 10, the nodes are at these relative locations
                            // That means the vertices are at the opposite coords
                            VertexIds = new[]
                           {
                              SignedOctreeNode.SignOffsets.IndexOf( createOffset(dir, 0, 1 - nc0[nodeIndex], 1 - nc1[nodeIndex])),
                              SignedOctreeNode.SignOffsets.IndexOf( createOffset(dir, 1, 1 - nc0[nodeIndex], 1 - nc1[nodeIndex]))
                           }
                        }).ToArray()
                ).ToArray();

        }

        /// <summary>
        /// Converts a 3D node offset coordinate to a node child index.
        /// The coordoffset allows rotating the coordinates. 
        /// Coordoffset of 0 means X: c0, y:c1, z:c2
        /// Coordoffset of 1 means Y: c0, z:c1, x:c2
        /// </summary>
        private int offsetToChildIndex(int coordOffset, int c0, int c1, int c2)
        {
            var offset = createOffset(coordOffset, c0, c1, c2);

            var ret = SignedOctreeNode.ChildOffsets.IndexOf(offset);
            //Debug.Assert(ret != -1);
            return ret;
        }

        private static Point3 createOffset(int coordOffset, int c0, int c1, int c2)
        {
            var offset = new Point3();
            offset[coordOffset % 3] = c0;
            offset[(coordOffset + 1) % 3] = c1;
            offset[(coordOffset + 2) % 3] = c2;
            return offset;
        }

        private EdgeToEdge getSubEdgeForEdge(int edgeDir, int subEdgeIndex)
        {
            return edgeToEdges[edgeDir][subEdgeIndex];
        }
        public FaceToEdge getSubEdgeForFace(int faceDir, int subEdgeIndex)
        {
            return faceToEdges[faceDir][subEdgeIndex];
        }

        private FaceToFace getSubFaceForFace(int faceDir, int subFaceIndex)
        {
            return faceToFaces[faceDir][subFaceIndex];
        }

        public int[] getVertIdsForEdge(int dir, int index)
        {
            return edgeToVertices[dir][index].VertexIds;
        }

        private void writeQuadForEdge(SignedOctreeNode[] node, int dir)
        {
            processMinimalEdge(node, dir);
        }




        public void writeQuadsForCell(SignedOctreeNode tree, int maxDepth = int.MaxValue)
        {
            if (isLeaf(tree,maxDepth)) return; // No quads inside leaf node
            for (int i = 0; i < 8; i++)
            {
                var child = tree.Children[i];
                writeQuadsForCell(child, maxDepth);
            }

            var faceNodes = new SignedOctreeNode[2];

            for (int i = 0; i < 12; i++) // For each face between children of this node
            {
                var face = cellToFaces[i];

                //List<SignedOctreeNode> list = new List<SignedOctreeNode>();
                for (int iChild = 0; iChild < 2; iChild++)
                {
                    //var c = face.Children[ iChild ];
                    faceNodes[iChild] = tree.Children[face.Children[iChild]];
                }
                writeQuadsForFace(faceNodes, face.dir, maxDepth);

            }
            var edgeNodes = new SignedOctreeNode[4];

            for (int i = 0; i < 6; i++) // For each edge between children of this node
            {
                var edge = getEdgeForCell(i);
                edgeNodes = new SignedOctreeNode[4];
                for (int iEdgeNode = 0; iEdgeNode < 4; iEdgeNode++)
                {
                    edgeNodes[iEdgeNode] = tree.Children[edge.Children[iEdgeNode]];
                }
                writeQuadsForEdge(edgeNodes, edge.dir, maxDepth);

            }
        }

        public CellToEdge getEdgeForCell(int iEdge)
        {
            return cellToEdges[iEdge];
        }


        public void writeQuadsForFace(SignedOctreeNode[] node, int dir, int maxDepth = int.MaxValue)
        {
            Debug.Assert(node[0].Depth <= maxDepth && node[1].Depth <= maxDepth);
            // If depth differs this check does nothing
            //Debug.Assert(node[0].Size != node[1].Size || Math.Abs(Vector3.Distance(node[0].LowerLeft, node[1].LowerLeft) - node[0].Size) < 0.001f);
            //Debug.Assert(node[0].LowerLeft[dir] < node[1].LowerLeft[dir]);

            if (isLeaf(node[0], maxDepth) && isLeaf(node[1], maxDepth)) return; // There are no children, so this face is not split into subfaces or subedges
            var subFaceNodes = new SignedOctreeNode[2];
            for (int iFace = 0; iFace < 4; iFace++) // For each subface
            {
                var face = getSubFaceForFace(dir, iFace);
                //for (int j = 0; j < 2; j++)
                //    subFaceNodes[j] = getChildOrParent(node[j], face.ChildForNode[j]);

                for (int j = 0; j < 2; j++)
                    subFaceNodes[j] = getChildOrParent(node[j], face.ChildForNode[j], maxDepth);
                writeQuadsForFace(subFaceNodes, dir, maxDepth);
            }
            var subEdgeNodes = new SignedOctreeNode[4];

            for (int iEdge = 0; iEdge < 4; iEdge++) // For each edge between the subfaces (inside the current face)
            {
                var edge = getSubEdgeForFace(dir, iEdge);

                //var newSubEdgeNodes = new SignedOctreeNode[4]; //TODO: I have absolutely no clue why this is needed
                for (int i = 0; i < 4; i++)
                {
                    var signedOctreeNode = getChildOrParent(node[edge.Node[i]], edge.Child[i], maxDepth);
                    subEdgeNodes[i] = signedOctreeNode;
                }
                //var edgeNeighbours = Enumerable.Range(0, 4).Select(i => getChildOrParent(node[edge.Node[i]], edge.Child[i])).ToArray();

                writeQuadsForEdge(subEdgeNodes, edge.Dir, maxDepth); // Edges have different dir than face

            }
        }

        private static bool isLeaf(SignedOctreeNode node, int maxDepth)
        {
            Debug.Assert(node.Depth <= maxDepth);
            return node.Children == null || node.Depth == maxDepth;
        }

        public void writeQuadsForEdge(SignedOctreeNode[] node, int dir, int maxDepth)
        {
            Debug.Assert(node[0].Depth <= maxDepth && node[1].Depth <= maxDepth && node[2].Depth <= maxDepth && node[3].Depth <= maxDepth);
            // If depth differs this check does nothing
            //TODO: ebed check somehow? doesnt work for sparse nodes
            //Debug.Assert( Math.Abs(Vector3.Distance(node[0].LowerLeft, node[1].LowerLeft) - node[0].Size) < 0.001f);
            //Debug.Assert( Math.Abs(Vector3.Distance(node[1].LowerLeft, node[2].LowerLeft) - node[0].Size) < 0.001f);
            //Debug.Assert( Math.Abs(Vector3.Distance(node[2].LowerLeft, node[3].LowerLeft) - node[0].Size) < 0.001f);
            //Debug.Assert( Math.Abs(Vector3.Distance(node[3].LowerLeft, node[0].LowerLeft) - node[0].Size) < 0.001f);

            //Idea: can check that nodes are in the 00 01 11 10 order
            //Debug.Assert(node[0].LowerLeft[dir] <= node[1].LowerLeft[dir]);
            bool allLeafs = true;
            for (int i = 0; i < 4; i++)
            {
                if (isLeaf(node[i], maxDepth)) continue;
                allLeafs = false;
                break;
            }
            if (allLeafs)
            {
                writeQuadForEdge(node, dir);
                return;
            }

            //for (int iSubEdge = 0; iSubEdge < 2; iSubEdge++) // For each half of the edge
            //{
            //    var edge = getSubEdgeForEdge(dir, iSubEdge);
            //    var edgeNeighbours = Enumerable.Range(0, 4).Select(i => getChildOrParent(node[i], edge.ChildForNode[i])).ToArray();
            //    writeQuadsForEdge(edgeNeighbours, dir);
            //}


            var subEdgeNodes = new SignedOctreeNode[4];

            for (int iSubEdge = 0; iSubEdge < 2; iSubEdge++) // For each half of the edge
            {
                var edge = getSubEdgeForEdge(dir, iSubEdge);
                for (int i = 0; i < 4; i++)
                    subEdgeNodes[i] = getChildOrParent(node[i], edge.ChildForNode[i], maxDepth);

                writeQuadsForEdge(subEdgeNodes, dir, maxDepth);
            }
        }



        /// <summary>
        /// Returns the child at given index if exists, otherwise returns the node itself
        /// </summary>
        private SignedOctreeNode getChildOrParent(SignedOctreeNode node, int childIndex, int maxDepth)
        {
            if (isLeaf(node,maxDepth))
                return node;

            return node.Children[childIndex];
        }


        public static string indexToOffsetString(int[] childIndices)
        {
            return string.Join(" | ",
                childIndices.Select(i => SignedOctreeNode.ChildOffsets[i])
                .Select(v => v.X + "," + v.Y + "," + v.Z)
                );
        }

        public struct CellToFace
        {
            public int[] Children;
            public int dir;

            public override string ToString()
            {
                return string.Format("Dir: {0}, Children: {1}", dir, indexToOffsetString(Children));
            }
        }

        public struct CellToEdge
        {
            public int[] Children;
            public int dir;
            public override string ToString()
            {
                return string.Format("Dir: {0}, Children: {1}", dir, indexToOffsetString(Children));
            }
        }

        public struct FaceToFace
        {
            public int[] ChildForNode;
            public override string ToString()
            {
                return string.Format("ChildForNode: {0}", indexToOffsetString(ChildForNode));
            }
        }

        public struct FaceToEdge
        {
            public int Dir;
            public int[] Node;
            public int[] Child;

            public override string ToString()
            {
                return string.Format("Dir: {0}, Node: {1}, Child: {2}",
                    Dir,
                    string.Join(" ", Node.Select(i => i.ToString()).ToArray()),
                   indexToOffsetString(Child));
            }
        }

        public struct EdgeToEdge
        {
            public int[] ChildForNode;
            public override string ToString()
            {
                return string.Format("ChildForNode: {0}", indexToOffsetString(ChildForNode));
            }
        }

        public struct EdgeToVertex
        {
            public int[] VertexIds;
        }


    }
}