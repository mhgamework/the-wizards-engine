using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    class WereldQuadtreeNode : IQuadtreeNode
    {
        private WereldQuadtreeNode parent;
        private WereldQuadtreeNode[] children;

        public WereldQuadtreeNode()
        {
            parent = null;
            children = new WereldQuadtreeNode[ 4 ];
        }


        public WereldQuadtreeNode Parent
        {
            get { return parent; }
        }

        public WereldQuadtreeNode GetChildNode( QuadtreeNodeDir dir )
        {
            return children[ (byte)dir ];
        }


        #region IQuadtreeNode Members

        IQuadtreeNode IQuadtreeNode.Parent
        {
            get { return parent; }
        }

        IQuadtreeNode IQuadtreeNode.GetChildNode( QuadtreeNodeDir dir )
        {
            return children[ (byte)dir ];
        }

        void IQuadtreeNode.OnMerge()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        void IQuadtreeNode.OnSplit()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        bool IQuadtreeNode.CanSplit()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        bool IQuadtreeNode.CanMerge()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}
