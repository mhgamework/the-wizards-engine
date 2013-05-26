using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using EnvDTE;
using EnvDTE80;
using Process = System.Diagnostics.Process;

namespace MHGameWork.TheWizards.VSIntegration
{
    public class VSDebugAttacher
    {
        /// <summary>
        /// Attaches a visual studio to this process
        /// </summary>
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

        /// <summary>
        /// Selects a line in given file in VS
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="line"></param>
        public void GotoLine(string filename, int line)
        {
            var dte = VSDebugAttacher.GetDTE2();
            dte.ItemOperations.OpenFile(filename);

            var txtSel = dte.ActiveDocument.Selection as TextSelection;

            txtSel.GotoLine(line, true);

        }



        /// <summary>
        /// Returns the source .cs file for given type, from an open visual studio instance
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public string FindSourceFile(Type t)
        {
            var assName = getAssemblyName(t.Assembly);
            Console.WriteLine(assName);
            var dte = GetDTE2();
            var project = dte.Solution.Projects.OfType<Project>().First(p => p.Name == assName);




            var nSpace = GetNamespace(project, t.Namespace);

            var cls = nSpace.Members.OfType<CodeClass>().First(c => c.Name == t.Name);

            return cls.ProjectItem.FileNames[0];

        }

        private string getAssemblyName(Assembly assembly)
        {
            var parts = assembly.FullName.Split(',');
            return parts[0];
        }


        private EnvDTE.CodeNamespace GetNamespace(EnvDTE.Project f, string name)
        {
            var namespaceParts = name.Split('.');
            EnvDTE.CodeNamespace current = null;
            foreach (var part in namespaceParts)
            {
                if (current == null)
                {
                    current = f.CodeModel.CodeElements.OfType<EnvDTE.CodeNamespace>().First(n => n.Name == part);
                    continue;
                }
                current = current.Members.OfType<EnvDTE.CodeNamespace>().First(n => n.Name == part);
            }
            return current;
        }


        /// <summary>
        /// Selects the exception line in visual studio
        /// </summary>
        /// <param name="exc"></param>
        public void SelectExceptionLine(Exception exc)
        {
            Console.WriteLine(exc.StackTrace);
            Match m = Regex.Match(exc.StackTrace, @"in (.+\.cs):line ([0-9]+)");
            var file = m.Groups[1].Value;
            var line = int.Parse(m.Groups[2].Value);
            GotoLine(file, line);
        }
    }
}
