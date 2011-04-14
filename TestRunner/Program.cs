using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestRunner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            throw new NotImplementedException();
            if (args[0] == "-test")
            {
                runTests(args[1]);
                    return;
            }
                
            /*var testsAssemblyPath = args[0];
            var testClass = args[1];
            var testMethod = args[1];*/

            for(;;)
            {
                Console.WriteLine("Alive!");
                Thread.Sleep(1000);
            }
        }

        private static void runTests(string testName)
        {
            if (testName == "Exception")
                throw new Exception("Test exception!");
        }
    }
}
