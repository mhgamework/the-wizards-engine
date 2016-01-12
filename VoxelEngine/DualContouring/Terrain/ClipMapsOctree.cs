using System;
using System.Drawing;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    public class ClipMapsOctree<T> where T : class, IOctreeNode<T>
    {
        private readonly IOctreeNodeFactory<T> factory;
        public static Point3[] ChildOffsets = GridHelper.UnitCubeCorners.Cast<Point3>().ToArray();

        public ClipMapsOctree()
        {
            factory = new EmptyConstructorFactory();
        }

        public class EmptyConstructorFactory : IOctreeNodeFactory<T>
        {
            public void Destroy(T node)
            {
            }

            public T Create(T parent, int size, int depth, Point3 pos)
            {
                var ret = Activator.CreateInstance<T>();
                ret.Size = size;
                ret.Depth = depth;
                ret.LowerLeft = pos;
                return ret;
            }
        }

        public ClipMapsOctree(IOctreeNodeFactory<T> factory)
        {
            this.factory = factory;
        }


        public T Create(int size, int leafCellSize, int depth = 0, Point3 pos = new Point3())
        {
            var ret = factory.Create(null, size, depth, pos);
            ret.Initialize(null);

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
            lm.AddBox(new BoundingBox(node.LowerLeft.ToVector3(), (Vector3)node.LowerLeft.ToVector3() + node.Size * new Vector3(1)),
                      col);
        }

        public void Split(T ret, bool recurse = false, int minSize = 1)
        {
            if (ret.Children != null) throw new InvalidOperationException();

            var childSize = ret.Size / 2;
            if (childSize < minSize) return;

            ret.Children = new T[8];

            for (int i = 0; i < 8; i++)
            {
                var c = factory.Create(ret, childSize, ret.Depth + 1, ret.LowerLeft + ChildOffsets[i] * childSize);
                c.Initialize(ret);
                ret.Children[i] = c;
                if ( recurse )
                    Split( c, recurse, minSize );
            }
        }

        public void Merge(T node)
        {
            if (node.Children == null) return;
            for (int i = 0; i < 8; i++)
            {
                // Dispose and merge children
                Merge(node.Children[i]);
                node.Children[i].Destroy();
                factory.Destroy(node.Children[i]);
            }
            node.Children = null;
        }

        public void UpdateQuadtreeClipmaps(T node, Vector3 cameraPosition, int minNodeSize)
        {
            var center = (Vector3)node.LowerLeft.ToVector3() + new Vector3(1) * node.Size * 0.5f;
            var dist = Vector3.Distance(cameraPosition, center);

            // Should take into account the fact that if minNodeSize changes, the quality of far away nodes changes so the threshold maybe should change too
            if (dist > node.Size * 1.2f)
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

        /// <summary>
        /// Walks the tree until finds a leaf, then does siblings (true depth first)
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="action"></param>
        public void VisitDepthFirst(T rootNode, Action<T> action)
        {
            if ( rootNode.Children != null )
            {
                for ( int i = 0; i < 8; i++ )
                {
                    VisitDepthFirst(rootNode.Children[i], action);
                }
            }
            action(rootNode);

        }

        /// <summary>
        /// TODO: could implement using the Func(T,bool) variant
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="action"></param>
        public void VisitTopDown(T rootNode, Action<T> action)
        {
            action(rootNode);
            if (rootNode.Children == null) return;
            for (int i = 0; i < 8; i++)
            {
                VisitTopDown(rootNode.Children[i], action);
            }
        }
        /// <summary>
        /// Visits the tree top-down, meaning that it traverses each node and than that nodes children before its siblings. return value determines the visit behaviour
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="action"></param>
        public VisitOptions VisitTopDown(T rootNode, Func<T, VisitOptions> action)
        {
            var ac = action(rootNode);
            if (ac != VisitOptions.Continue) return ac;
            if (rootNode.Children == null) return VisitOptions.Continue;// No abort
            for (int i = 0; i < 8; i++)
            {
                var ret = VisitTopDown(rootNode.Children[i], action);
                if (ret == VisitOptions.AbortVisit) return VisitOptions.AbortVisit;
            }
            return VisitOptions.Continue; // No abort
        }

      
    }
    public enum VisitOptions
    {
        Continue,
        SkipChildren,
        AbortVisit
    }

}