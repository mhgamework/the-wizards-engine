using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Scene
{
    public interface IDataElement<T>
    {
        T Get();
        void Set(T value);
    }
}
