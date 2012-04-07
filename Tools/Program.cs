using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Tools
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Missing argument!");
                return;
            }

            var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(o => o.Name.ToLower() == args[0]);

            if (type == null || !typeof(ITool).IsAssignableFrom(type))
            {
                Console.WriteLine("Tool not found!");
                return;
            }
            var tool = (ITool)Activator.CreateInstance(type);

            tool.Execute();

            //Environment.CurrentDirectory = (new DirectoryInfo(Environment.CurrentDirectory)).Parent.Parent.FullName;
            //CSRunner.Main(new[] { "../../TestTool.cs" });
            //CSRunner.Main(args);



        }
    }
}
