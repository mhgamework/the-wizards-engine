using System;
using MHGameWork.TheWizards.ModelContainer;

namespace MHGameWork.TheWizards.Engine
{
    public interface IModelObjectAddon<T> : IDisposable where T : IModelObject
    {

    }
}
