using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MHGameWork.TheWizards.TestRunner;

namespace MHGameWork.TheWizards.CG
{
    class Program
    {
        public static void Main()
        {
            TestRunnerGUI runnerGui = new TestRunnerGUI();
            runnerGui.TestsAssembly = Assembly.GetExecutingAssembly();
            //runner.RunTestNewProcessPath = "\"" + Assembly.GetExecutingAssembly().Location + "\"" + " -test {0}";

            runnerGui.Run();
        }
    }
}
