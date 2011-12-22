using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
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

            var testResult = TestResult.FromException(result);

            var ser = new XmlSerializer(typeof (TestResult));
            var w = new StringWriter();
            ser.Serialize(w, testResult);

            Console.WriteLine(">>>Start Result");
            
            Console.WriteLine(w.GetStringBuilder().ToString());

            Console.WriteLine("<<<End Result");
        }

       
    }
}
