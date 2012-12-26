using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Data;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainOnAssemblyLoad;

            otherMethod();
        }

        private static void otherMethod()
        {
            var attacher = new VSIntegration.VSDebugAttacher();
            //attacher.AttachToVisualStudio();

            Thread.Sleep(2000);
            var eng = new Engine.TWEngine();
            eng.GameplayDll = "Gameplay.dll";
            eng.HotloadingEnabled = true;
            eng.Start();
        }

        private static void CurrentDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (args.LoadedAssembly.FullName.ToLower().Contains("gameplay"))
            {
                File.AppendAllText("loads.txt",args.LoadedAssembly.Location + "\n");
                
            }
        }
    }

}
