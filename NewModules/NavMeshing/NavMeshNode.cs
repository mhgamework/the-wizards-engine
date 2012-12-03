using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.NavMeshing
{
    public class NavMeshNode
    {

        public List<Edge> Edges = new List<Edge>();
        public int NumPoints { get { return Edges.Count; } }


        public NavMeshNode(Vector3[] polygon)
        {
            foreach(var p in polygon)
            {
                Edges.Add(new Edge {Point = p});
            }

        }
        public NavMeshNode()
        {
            
        }

        public bool IsPointOnEdge(int edgeNum, SlimDX.Vector3 point)
        {
            throw new NotImplementedException();
        }
    
        /*public void Connect(int i, NavMeshNode left)
        {
            if (Connected[i] != null)
            {
                var other = Connected[i];
                other.Connected[other.FindConnectionEdge(this)] = null;
            }
            Connected[i] = left;

        }*/
    }
    /// <summary>
    ///    L|R   = edge
    ///     P    = point
    /// </summary>
    public struct Edge
    {
        public Vector3 Point;
        public NavMeshNode LeftNode;
        public NavMeshNode RightNode;
    }
}
