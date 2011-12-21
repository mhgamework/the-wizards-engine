using System;
using CommandLine;

namespace MHGameWork.TheWizards.TestRunner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var options = new CommandlineOptions();
            ICommandLineParser parser = new CommandLineParser();
            if (!parser.ParseArguments(args, options))
            {
                Console.Error.WriteLine(options.GetUsage());
                return;
            }


            var runner = new TestRunner();
            var result = runner.RunTest(options.AssemblyName, options.TypeFullQualifiedName, options.MethodName);




          

        }

      
    }
}
