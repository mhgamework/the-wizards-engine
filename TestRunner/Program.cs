using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            {
                Console.Error.WriteLine(options.GetUsage());
                return;
            }


            var obj = new MHGameWork.TheWizards.Utilities.TestRunnerGUI.CallbackObject();
            obj.MethodName = options.MethodName;
            obj.TypeFullQualifiedName = options.TypeFullQualifiedName;

            obj.RunAutomated = true;
            obj.AutoShutDown = 3;
            
            obj.Assembly = Assembly.LoadFrom(options.AssemblyName);



            obj.RunTest();

        }

      
    }
}
