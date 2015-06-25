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

        public void DrawLines(LodOctreeNode node, LineManager3D lm, Func<LodOctreeNode, bool> isVisible, Func<LodOctreeNode, Color> getColor)
        {
            if (isVisible(node))
                DrawSingleNode(node, lm, getColor(node));

            if (node.Children == null) return;
            for (int i = 0; i < 8; i++)
            {
                DrawLines(node.Children[i], lm, isVisible, getColor);
            }
        }
        public void DrawLines(LodOctreeNode node, LineManager3D lm)
        {
            DrawSingleNode(node, lm, Color.Black);
            if (node.Children == null) return;
            for (int i = 0; i < 8; i++)
            {
                DrawLines(node.Children[i], lm);
            }
        }

        public void DrawSingleNode(LodOctreeNode node, LineManager3D lm, Color col)
        {
            lm.AddBox(new BoundingBox(node.LowerLeft.ToVector3(), node.LowerLeft.ToVector3() + node.size * new Vector3(1)),
                      col);
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
            if (node.Children == null) return;
            for (int i = 0; i < 8; i++) node.Children[i].Destroy();
            node.Children = null;
        }

        public void UpdateQuadtreeClipmaps(LodOctreeNode node, Vector3 cameraPosition, int minNodeSize)
        {
            var center = node.LowerLeft.ToVector3() + new Vector3(1) * node.size * 0.5f;
            var dist = Vector3.Distance(cameraPosition, center);

            // Should take into account the fact that if minNodeSize changes, the quality of far away nodes changes so the threshold maybe should change too
            if (dist > node.size * 1.2f)
            {
                // This is a valid node size at this distance, so remove all children
                Merge(node);
            }
            else
            {
                if (node.Children == null)
                    Split(node, false, minNodeSize);

                if (node.Children == null) return; // Minlevel

                for (int i = 0; i < 8; i++)
                {
                    UpdateQuadtreeClipmaps(node.Children[i], cameraPosition, minNodeSize);
                }
            }
        }

        public void VisitDepthFirst(LodOctreeNode rootNode, Action<LodOctreeNode> action)
        {
            action(rootNode);
            if (rootNode.Children == null) return;
            for (int i = 0; i < 8; i++)
            {
                VisitDepthFirst(rootNode.Children[i], action);
            }
        }
    }
}