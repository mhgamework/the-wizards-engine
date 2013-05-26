﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Process = System.Diagnostics.Process;

namespace MHGameWork.TheWizards.VSIntegration
{
    public class VSDebugAttacher
    {
        public void AttachToVisualStudio()
        {
            Console.WriteLine("Attempting to attach to debugger");
            //// Reference Visual Studio core
            var dte = GetDTE2();
            if (dte == null) return;

            // Try loop - Visual Studio may not respond the first time.

            Debug.WriteLine("Debug test");
            int tryCount = 5;
            while (tryCount-- > 0)
            {
                try
                {
                    Processes processes = dte.Debugger.LocalProcesses;
                    foreach (EnvDTE.Process proc in processes.Cast<EnvDTE.Process>())
                    {
                        //Console.WriteLine(proc.Name);
                        if (!proc.Name.Contains("NewModules")) continue;
                        if (dte.Debugger.CurrentMode == dbgDebugMode.dbgDesignMode)
                            proc.Attach();
                        Console.WriteLine(String.Format
                        ("Attached to process {0} successfully.", proc.Name));
                        tryCount = 0;
                        break;
                    }
                    break;
                }
                catch (COMException e)
                {
                    Console.WriteLine(e.ToString());
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        public static DTE2 GetDTE2()
        {
            DTE2 dte = null;
            try
            {
                // Get an instance of the currently running Visual Studio IDE.
                dte = (DTE2)System.Runtime.InteropServices.Marshal.
                                    GetActiveObject("VisualStudio.DTE.11.0");
            }
            catch (COMException)
            {
                Console.WriteLine(String.Format(@"Visual studio not found."));
            }
            return dte;
        }

        public void GotoLine(string filename, int line)
        {
            var dte = VSDebugAttacher.GetDTE2();
            dte.ItemOperations.OpenFile(filename);

            var txtSel = dte.ActiveDocument.Selection as TextSelection;

            txtSel.GotoLine(line, true);

        }
    }
}
