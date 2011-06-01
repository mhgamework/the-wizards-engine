using System;

namespace MHGameWork.TheWizards.Forms
{
    public interface IAttribute
    {
        string Name { get; }
        Type Type { get; }
        object GetData(object obj);
        void SetData(object obj, object value);
    }
}