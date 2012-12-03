using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.NavMeshing
{
    public class NavMeshBuilder
    {


        /// <summary>
        /// The original node contains the edge start vertex, the newNode the 2nd edge vertex
        /// Adding the point should preserve convexity!!! should be on edge
        /// </summary>
        /// <param name="node"></param>
        /// <param name="edgeNum"></param>
        /// <param name="point"></param>
        public void SplitNode(Edge edge, Vector3 point)
        {
            Edge newEdge = new Edge { Point = point, LeftNode = edge.LeftNode, RightNode = edge.RightNode };

            if (edge.LeftNode != null)
                edge.LeftNode.Edges.Insert( edge.LeftNode.Edges.IndexOf(edge) + 1, newEdge);

            if (edge.RightNode != null)
                edge.RightNode.Edges.Insert(edge.RightNode.Edges.IndexOf(edge) + 1, newEdge);


            //var startVertexIndex = node.GetEdgeStartVertexIndex(edgeNum);

            //// Split into 2 polygons
            //// First one goes from secondVertexIndex to startVertexIndex + the new point
            //// Second one from the new point to startVertexIndex + 1 to secondVertexIndex

            //// determine the second split point, take random point for now
            //var secondVertexIndex = startVertexIndex - 1;

            //var polygon1 = new Vector3[startVertexIndex - secondVertexIndex + 2];
            //var polygon2 = new Vector3[node.NumPoints - polygon1.Length + 3];

            //var connections1 = new NavMeshNode[polygon1.Length];
            //var connections2 = new NavMeshNode[polygon2.Length];


            //for (int i = 0; i < polygon1.Length - 1; i++)
            //{
            //    int index = (secondVertexIndex + i + node.NumPoints) % node.NumPoints;
            //    polygon1[i] = node.Polygon[i];
            //    connections1[i] = node.Connected[index];
            //}
            //polygon1[polygon1.Length - 1] = point;


            //polygon2[0] = point;
            //for (int i = 1; i < polygon2.Length; i++)
            //{
            //    int index = (startVertexIndex + i + node.NumPoints) % node.NumPoints;
            //    polygon2[i] = node.Polygon[index];
            //    connections2[i] = node.Connected[index];
            //}


            //newNode = new NavMeshNode(polygon2, connections2);

            //connections1[polygon1.Length - 1] = newNode;
            //connections2[polygon2.Length - 1] = node;

            //node.Connected = connections1;
            //node.Polygon = polygon1;


            //newNode = null;

        }

        public NavMesh CreateFromMesh(IMesh mesh)
        {
            throw new NotImplementedException();
        }

        public void AttachToNavMesh(NavMesh bigMesh, NavMesh attacheableMesh)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the bidirectional dependencies are correct
        /// </summary>
        /// <param name="mesh"></param>
        public void CheckMeshConsistency(NavMesh mesh)
        {

        }
    }
}


