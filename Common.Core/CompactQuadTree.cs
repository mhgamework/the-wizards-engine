using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// This uses an array to represent the tree
    /// </summary>
    public class CompactQuadTree<T>
    {
        private T[] data;

        private int lastLevelFirstNodeIndex; // Optimization

        public CompactQuadTree(int levels)
        {
            data = new T[CalculateNbNodes(levels)];

            lastLevelFirstNodeIndex = CalculateNbNodes(levels - 1);
        }

        public static int CalculateNbNodes(int levels)
        {
            if (levels < 0) throw new ArgumentOutOfRangeException();
            // sum of row 2^k: a (1-r^n)/(1-r) met a eerste element en r = 4

            return (1 - (1 << (levels << 1))) / (1 - 4);
        }

        public Node GetRoot()
        {
            return new Node(0);
        }


        public T this[Node node]
        {
            get { return data[node.Index]; }
            set { data[node.Index] = value; }
        }
        public T this[int index]
        {
            get { return data[index]; }
            set { data[index] = value; }
        }

        public struct Node
        {
            public int Index;

            public Node(int index)
            {
                Index = index;
            }

            public Node GetChild(CompactQuadTreeChild c)
            {
                return new Node(Index * 4 + (int)c); // parent * 4 + childindex
            }
            public Node Parent
            {
                get
                {
                    return new Node(Index - 1 >> 2); // floor( (parent-1)/4 )
                }
            }
            public Node LowerLeft { get { return GetChild(CompactQuadTreeChild.LowerLeft); } }
            public Node LowerRight { get { return GetChild(CompactQuadTreeChild.LowerRight); } }
            public Node UpperLeft { get { return GetChild(CompactQuadTreeChild.UpperLeft); } }
            public Node UpperRight { get { return GetChild(CompactQuadTreeChild.UpperRight); } }

            public T Get(CompactQuadTree<T> tree)
            {
                return tree[this];
            }
            public void Set(CompactQuadTree<T> tree, T value)
            {
                tree[this] = value;
            }
            public bool IsLeaf(CompactQuadTree<T> tree)
            {
                return Index >= tree.lastLevelFirstNodeIndex;
            }
            public bool IsRoot { get { return Index == 0; } }
        }


    }
    public enum CompactQuadTreeChild
    {
        /// <summary>
        /// Upperleft node: -X -Z
        /// </summary>
        UpperLeft = 1,
        /// <summary>
        /// Upperright node: +X -Z
        /// </summary>
        UpperRight,
        /// <summary>
        /// Lowerleft node: -X +Z
        /// </summary>
        LowerLeft,
        /// <summary>
        /// LowerRight node: +X +Z
        /// </summary>
        LowerRight
    }
}
