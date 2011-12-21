using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace TestRunner
{
    public class CommandlineOptions
    {
        class Options
        {

            [Option("c", "testclass", Required = true, HelpText = "Test class to run")]
            public string TypeFullQualifiedName;
            [Option("m", "testmethod", Required = true, HelpText = "Test method to run.")]
            public string MethodName;
            [Option("a", "assembly", Required = true, HelpText = "Path to the assembly containing the test.")]
            public string AssemblyName;


            [HelpOption(HelpText = "Display this help screen.")]
            public string GetUsage()
            {
                var usage = new StringBuilder();
                usage.AppendLine("The Wizards TestRunner.");
                return usage.ToString();
            }
        }
    }
}
