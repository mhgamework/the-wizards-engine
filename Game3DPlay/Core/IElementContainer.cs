using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core
{
    public interface IElementContainer : IElement
    {
        bool AcceptsElement(Element IE);

        //void AddElement(Element IE);
        void RemoveElement(Element IE);

        bool TryAdd(Element IE);
    }
}
