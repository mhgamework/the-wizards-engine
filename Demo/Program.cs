using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MHGameWork.TheWizards.VoxelEngine;

namespace Demo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var test = new DualContouringBuilderTest();
            test.InitializeEngine();
            test.Setup();
            test.Run();
            test.RunEngine();
            
        }
    }
}
