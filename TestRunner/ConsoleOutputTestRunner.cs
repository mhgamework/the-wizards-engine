using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.TestRunner
{
    /// <summary>
    /// Test runner decorater which adds console info when starting and completing tests
    /// Only decorates the NUnitTestrunner
    /// </summary>
    public class ConsoleOutputTestRunner : ITestRunner
    {
        private readonly NUnitTestRunner runner;

        public ConsoleOutputTestRunner(NUnitTestRunner runner)
        {
            this.runner = runner;
        }

        public void Run(ITest test)
        {
            var n = (NUnitTest)test;

            var name = n.TestClass.FullName + " - " + n.TestMethod.Name;

            Console.WriteLine("Starting test " + name);
            runner.Run(test);
            Console.WriteLine("Completed test " + name);

        }
    }
}
