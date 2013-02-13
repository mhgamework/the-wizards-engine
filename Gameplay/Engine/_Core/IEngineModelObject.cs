using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Engine._Core
{
    public interface IEngineModelObject
    {
        T get<T>() where T : class;

        void set<T>(T value) where T : class;
    }
}
