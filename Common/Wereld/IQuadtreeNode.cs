using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Common.Wereld
{
    public interface IQuadtreeNode
    {
        void Split();
        void Merge();

        IQuadtreeNode GetIChild( QuadtreeChildDirection childDir );

        void SetStatic( bool value );
    }
}
