using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Scripting.API;

namespace MHGameWork.TheWizards.Scene
{
    public class SimpleDataElement<T> : IDataElement<T>
    {
        private T value;
        public T Get()
        {
            return value;
        }

        public void Set(T value)
        {
            this.value = value;
        }

    }
}
