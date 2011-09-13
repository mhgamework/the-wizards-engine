using System;
using System.IO;
using System.Reflection;
using System.Threading;
using MHGameWork.TheWizards.Main;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Tests;
using MHGameWork.TheWizards.Utilities;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards
{
    static class Program
    {


        /// <summary>
        /// The main entry point for the application 
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            var fi = new FileInfo("Console.log");
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            var oldstrm = Console.Out;



            using (var redir = new ConsoleRedir(oldstrm, fi.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
            {
                redir.WriteLine("--------------------Start Logging!");
                Console.SetOut(redir);




                TestRunner runner = new TestRunner();
                runner.TestsAssembly = typeof(CoreTest).Assembly;
                runner.RunTestNewProcessPath = "\"" + Assembly.GetExecutingAssembly().Location + "\"" + " -test {0}";


                if (args.Length == 2 && args[0] == "-test")
                {
                    runner.RunTestByName(args[1]);
                }
                else
                {
                    runner.Run();

                }





                redir.WriteLine("--------------------End Logging!");
            }
            Console.SetOut(oldstrm);
        }


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            Console.WriteLine(e.ExceptionObject);
        }

        private class ConsoleRedir : StreamWriter
        {
            private readonly TextWriter writer;

            public ConsoleRedir(TextWriter writer, Stream strm)
                : base(strm)
            {
                this.writer = writer;
            }

            public override void WriteLine(string value)
            {
                base.WriteLine(String.Format("{0:d/M/yyyy HH:mm:ss} - {1}", DateTime.Now, value));
                writer.WriteLine(value);
            }
        }


    }
}

