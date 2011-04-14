using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public class QuadtreeStom<T> where T : IQuadtreeNode, new()
    {
        private T rootNode;

        public QuadtreeStom()
        {
            rootNode = new T();
        }

        public void Split( T node )
        {
            if ( node.CanSplit() == false ) return;

            throw new InvalidOperationException( "Not implemented" );
        }

        public void Merge( T node )
        {
            if ( node.CanMerge() == false ) return;
            throw new InvalidOperationException( "Not implemented" );
        }

    }
}
