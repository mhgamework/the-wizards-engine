using System.Reflection;

namespace MHGameWork.TheWizards.TestRunner
{
    public interface ITest
    {
        Assembly TestAssembly { get; }
    }
}