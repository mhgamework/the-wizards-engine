using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public interface IQuadtreeNode
    {
        IQuadtreeNode Parent { get;}
        IQuadtreeNode GetChildNode( QuadtreeNodeDir dir );
        void OnMerge();
        void OnSplit();
        bool CanSplit();
        bool CanMerge();
    }

    public enum QuadtreeNodeDir : byte
    {
        Upperleft = 1,
        Lowerleft,
        UpperRight,
        LowerRight
    }
}
