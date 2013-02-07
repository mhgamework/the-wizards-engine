using System;
using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.Engine
{
    public interface IModelObjectAddon<T> : IDisposable where T : IModelObject
    {

    }
}
