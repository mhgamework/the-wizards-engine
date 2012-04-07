using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Tools
{
    /// <summary>
    /// This class takes as input a *.cs file and compiles and runs it.
    /// The runner creates a cached exe for execution
    /// </summary>
    public class CSRunner
    {
        public static void Main(String[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Error, invalid arguments given!");
                return;
            }

            var targetFile = args[0];



            var cscPath = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe";
            var demo = "/define:DEBUG /optimize /out:File2.exe /Main:Program /reference:other.dll *.cs";

            var exePath = Path.GetTempFileName();
            var mainClassName = Path.GetFileNameWithoutExtension(targetFile);

            var arguments =
                string.Format("/define:DEBUG /optimize /Main:{1} /out:{0} *.cs", exePath, mainClassName);


            var p = Process.Start(new ProcessStartInfo
                                      {
                                          FileName = cscPath,
                                          Arguments = arguments,
                                          CreateNoWindow = true,
                                          UseShellExecute = false,
                                          RedirectStandardOutput = true,
                                          WorkingDirectory = Path.GetDirectoryName(targetFile)
                                      });

            p.WaitForExit();
            //Console.WriteLine(p.StandardOutput.ReadToEnd());


            RunExecutable(exePath, "");

            Console.WriteLine("<<<<<<<<Completed run of " + targetFile);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="arguments"></param>
        /// <param name="runLockstep">true when the exe output should be redirected to this program and executing should be halted</param>
        public static void RunExecutable(string filename, string arguments, bool runLockstep = true)
        {
            Process p;
            p = Process.Start(new ProcessStartInfo
                                  {
                                      FileName = filename,
                                      Arguments = arguments,
                                      CreateNoWindow = true,
                                      UseShellExecute = false,
                                      RedirectStandardOutput = true
                                  });

            if (!runLockstep)
                return;

            while (!p.HasExited)
            {
                Console.WriteLine(p.StandardOutput.ReadLine());
            }
        }
    }
}
