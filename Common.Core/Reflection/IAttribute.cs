using System;

namespace MHGameWork.TheWizards.Reflection
{
    public interface IAttribute
    {
        string Name { get; }
        Type Type { get; }
        object GetData(object obj);
        void SetData(object obj, object value);
    }
}