using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld
{
    /// <summary>
    /// Octree node for a octree with grid signs.
    /// </summary>
    public class SignedOctreeNode : IOctreeNode<SignedOctreeNode>
    {
        public static List<Point3> SignOffsets { get; private set; }
        public static List<Point3> ChildOffsets { get; private set; }
        //private readonly int[][] edgeToVertices;
        //private readonly List<Point3[]> cubeEdges;


        static SignedOctreeNode()
        {
            SignOffsets = (from x in Enumerable.Range(0, 2)
                           from y in Enumerable.Range(0, 2)
                           from z in Enumerable.Range(0, 2)
                           select new Point3(x, y, z)).ToList();
            ChildOffsets = ClipMapsOctree<SignedOctreeNode>.ChildOffsets.ToList();
        }


        public SignedOctreeNode[] Children { get; set; }
        public Point3 LowerLeft { get; set; }
        public int Size { get; set; }
        public int Depth { get; set; }

        /// <summary>
        /// Idea: maybe store signs as a flagged byte, and make this signs array into a materialid array
        /// </summary>
        public bool[] Signs { get; set; }

        public Vector3 QEF = new Vector3(0.5f,0.5f,0.5f);


        public void Initialize(SignedOctreeNode parent)
        {
        }

        public void Destroy()
        {
        }

        public override string ToString()
        {
            return string.Format("Depth: {3}, LowerLeft: {1}, Size: {2}, Children: {0}", Children, LowerLeft, Size, Depth);
        }
    }
}