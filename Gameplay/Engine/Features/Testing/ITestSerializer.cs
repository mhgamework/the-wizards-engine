using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.TestRunner;

namespace MHGameWork.TheWizards.Engine.Testing
{
    public interface ITestSerializer
    {
        string Serialize(ITest test);
        ITest Deserialize(string data);
    }
}
