using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using Process = EnvDTE.Process;

namespace MHGameWork.TheWizards.VSIntegration
{
    public class VSDebugAttacher
    {
        public void AttachToVisualStudio()
        {
            //// Reference Visual Studio core
            DTE dte;
            try
            {
                dte = (DTE)Marshal.GetActiveObject("VisualStudio.DTE"); // can be found in HKEY_CLASSES_ROOT
            }
            catch (COMException)
            {
                Debug.WriteLine(String.Format(@"Visual studio not found."));
                return;
            }

            // Try loop - Visual Studio may not respond the first time.
            int tryCount = 5;
            while (tryCount-- > 0)
            {
                try
                {
                    Processes processes = dte.Debugger.LocalProcesses;
                    foreach (Process proc in processes.Cast<Process>())
                    {
                        Console.WriteLine(proc.Name);
                        if (!proc.Name.Contains("NewModules")) continue;
                        proc.Attach();
                        Debug.WriteLine(String.Format
                        ("Attached to process {0} successfully.", proc.Name));
                        tryCount = 0;
                        break;
                    }
                    break;
                }
                catch (COMException)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }
    }
}
