using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Tools.Tools
{
    public class Bash : ITool
    {
        public void Execute()
        {
            //"C:\Program Files (x86)\Git\Git Bash.lnk"


            //Process p;
            //p = Process.Start(new ProcessStartInfo
            //{
            //    FileName = @"cmd",
            //    Arguments = "/c \"\"C:\\Program Files (x86)\\Git\\bin\\sh.exe\" --login -i\"",
            //    WorkingDirectory = Config.TheWizardsRoot,
            //    CreateNoWindow = false,
            //    UseShellExecute = false,
            //    RedirectStandardOutput = true
            //});

            //Console.ReadLine();
            Process p;
            p = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/c \"\"" + Config.GitSh + "\" --login -i\"",
                WorkingDirectory = Config.TheWizardsRoot,
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = false
            });
        }
    }
}
