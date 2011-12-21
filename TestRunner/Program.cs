using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CommandLine;

namespace TestRunner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var options = new CommandlineOptions();
            ICommandLineParser parser = new CommandLineParser();
            if (!parser.ParseArguments(args, options))
                Console.WriteLine("Invalid input!");

            var obj = new MHGameWork.TheWizards.Utilities.TestRunner.CallbackObject();
            

        }

        private static void runTests(string testName)
        {
            if (testName == "Exception")
                throw new Exception("Test exception!");
        }
    }
}
