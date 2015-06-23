using System;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    public class LodOctree
    {
        public static Point3[] ChildOffsets = GridHelper.UnitCubeCorners;


        public LodOctreeNode Create(int size, int leafCellSize, int depth = 0, Point3 pos = new Point3())
        {
            var ret = new LodOctreeNode(size, depth, pos);

            if (size <= leafCellSize) return ret; // Finest detail

            Split(ret, true, leafCellSize);

            return ret;

        }

        public void DrawLines(LodOctreeNode node, LineManager3D lm)
        {
            lm.AddBox(new BoundingBox(node.LowerLeft.ToVector3(), node.LowerLeft.ToVector3() + node.size * new Vector3(1)), Color.Black);
            if (node.Children == null) return;
            for (int i = 0; i < 8; i++)
            {
                DrawLines(node.Children[i], lm);
            }
        }

        public void Split(LodOctreeNode ret, bool recurse = false, int minSize = 1)
        {
            if (ret.Children != null) throw new InvalidOperationException();

            var childSize = ret.size / 2;
            if (childSize < minSize) return;

            ret.Children = new LodOctreeNode[8];

            for (int i = 0; i < 8; i++)
            {
                ret.Children[i] = new LodOctreeNode(childSize, ret.depth + 1, ret.LowerLeft + ChildOffsets[i] * childSize);
            }
        }

        public void Merge(LodOctreeNode node)
        {
            node.Children = null;
        }
    }
}