using System;
using System.Drawing;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    public class LodOctree<T> where T : IOctreeNode<T>, new()
    {
        public static Point3[] ChildOffsets = GridHelper.UnitCubeCorners.Cast<Point3>().ToArray();


        public T Create(int size, int leafCellSize, int depth = 0, Point3 pos = new Point3())
        {
            var ret = new T() {size = size, depth = depth, LowerLeft = pos};//(size, depth, pos);

            if (size <= leafCellSize) return ret; // Finest detail

            Split(ret, true, leafCellSize);

            return ret;

        }

        public void DrawLines(T node, LineManager3D lm, Func<T, bool> isVisible, Func<T, Color> getColor)
        {
            if (isVisible(node))
                DrawSingleNode(node, lm, getColor(node));

            if (node.Children == null) return;
            for (int i = 0; i < 8; i++)
            {
                DrawLines(node.Children[i], lm, isVisible, getColor);
            }
        }
        public void DrawLines(T node, LineManager3D lm)
        {
            DrawSingleNode(node, lm, Color.Black);
            if (node.Children == null) return;
            for (int i = 0; i < 8; i++)
            {
                DrawLines(node.Children[i], lm);
            }
        }

        public void DrawSingleNode(T node, LineManager3D lm, Color col)
        {
            lm.AddBox(new BoundingBox(node.LowerLeft.ToVector3(), (Vector3)node.LowerLeft.ToVector3() + node.size * new Vector3(1)),
                      col);
        }

        public void Split(T ret, bool recurse = false, int minSize = 1)
        {
            if (ret.Children != null) throw new InvalidOperationException();

            var childSize = ret.size / 2;
            if (childSize < minSize) return;

            ret.Children = new T[8];

            for (int i = 0; i < 8; i++)
            {
                var c = new T();
                c.size = childSize;
                c.depth = ret.depth + 1;
                c.LowerLeft = ret.LowerLeft + ChildOffsets[i] * childSize;

                ret.Children[i] = c;//new T(childSize, ret.depth + 1, ret.LowerLeft + ChildOffsets[i] * childSize);
            }
        }

        public void Merge(T node)
        {
            if (node.Children == null) return;
            for (int i = 0; i < 8; i++) node.Children[i].Destroy();
            node.Children = null;
        }

        public void UpdateQuadtreeClipmaps(T node, Vector3 cameraPosition, int minNodeSize)
        {
            var center = (Vector3)node.LowerLeft.ToVector3() + new Vector3(1) * node.size * 0.5f;
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

        public void VisitDepthFirst(T rootNode, Action<T> action)
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