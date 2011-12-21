using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MHGameWork.TheWizards.TestRunner;
using MHGameWork.TheWizards.Utilities;
using System.Reflection;

namespace ProceduralTerrainGeneration
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            TestRunnerGUI runnerGui = new TestRunnerGUI();
            runnerGui.TestsAssembly = Assembly.GetExecutingAssembly();
            runnerGui.Run();

        }
    }
}
