using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// A quadtreenode specification containing the basics for a quadtreenode. This interface can be used with the static QuadTree class. The argument T is the type implementing the IQuadTreeNode interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQuadTreeNode<T> where T : IQuadTreeNode<T>
    {
        QuadTreeNodeData<T> NodeData
        {
            get;
            set;
        }

        /// <summary>
        /// Note that the parameter nodeData can dissapear if OnSplit is added!
        /// </summary>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        T CreateChild(QuadTreeNodeData<T> nodeData);

        // TODO: can add events here, OnSplit, OnMerge etc. 
    }
}
