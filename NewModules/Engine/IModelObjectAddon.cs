using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.ModelContainer
{
    public interface IModelObjectAddon<T> : IDisposable where T : IModelObject
    {

    }
}
