using System;
using System.Reflection;

namespace MHGameWork.TheWizards.TestRunner
{
    /// <summary>
    /// Responsible for running ITests
    /// </summary>
    public interface ITestRunner
    {
        void Run(ITest test);
    }
}