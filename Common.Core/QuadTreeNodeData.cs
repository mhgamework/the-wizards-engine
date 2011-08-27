using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// This is a structure, holding the basic data for an IQuadTreeNode. The argument T is the IQuadTreeNode implementation it holds data for.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct QuadTreeNodeData<T> where T : IQuadTreeNode<T>
    {
        /// <summary>
        /// Upperleft node: -X -Z
        /// </summary>
        public T UpperLeft;
        /// <summary>
        /// Upperright node: +X -Z
        /// </summary>
        public T UpperRight;
        /// <summary>
        /// Lowerleft node: -X +Z
        /// </summary>
        public T LowerLeft;
        /// <summary>
        /// LowerRight node: +X +Z
        /// </summary>
        public T LowerRight;
        public T Parent;

        /// <summary>
        /// Not sure the use of a boundingbox here is legit.
        /// </summary>
        public BoundingBox BoundingBox;

        public QuadTreeNodeData( BoundingBox boundingBox )
            : this()
        {
            BoundingBox = boundingBox;
        }

        public QuadTreeNodeData( T parent, BoundingBox boundingBox, T upperRight, T upperLeft, T lowerRight, T lowerLeft )
        {
            Parent = parent;
            BoundingBox = boundingBox;
            LowerRight = lowerRight;
            LowerLeft = lowerLeft;
            UpperRight = upperRight;
            UpperLeft = upperLeft;
        }
    }
}
