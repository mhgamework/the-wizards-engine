using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public class Section
    {
        public IQuadtreeNode quadtreeNode;

        public Section( IQuadtreeNode nQuadtreeNode )
        {
            quadtreeNode = nQuadtreeNode;
        }
    }
}
