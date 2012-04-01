using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace MHGameWork.TheWizards.Networking
{
    /// <summary>
    /// TODO: move to a generic Core project
    /// </summary>
    public class AssemblyBuilder
    {
        public static Assembly CompileExecutable(String code)
        {

            CompilerParameters cp = new CompilerParameters();

            // Generate class library
            cp.GenerateExecutable = false;

            // Specify the assembly file name to generate.
            //cp.OutputAssembly = exeName;

            // Only in memory
            cp.GenerateInMemory = true;
            cp.GenerateExecutable = false;

            // Set whether to treat all warnings as errors.
            cp.TreatWarningsAsErrors = false;

            return CompileExecutable(cp, code);

        }


        public static Assembly CompileExecutable(CompilerParameters cp, String code)
        {
            return CompileExecutable(cp, new string[] { code });
        }
        public static Assembly CompileExecutableFromFile(CompilerParameters cp, String[] files)
        {
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();

            CompilerResults cr = provider.CompileAssemblyFromFile(cp, files);

            return processCompilationResult(cr);
        }
        public static Assembly CompileExecutable(CompilerParameters cp, String[] code)
        {
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();

            CompilerResults cr = provider.CompileAssemblyFromSource(cp, code);

            return processCompilationResult(cr);
        }
        public static Assembly CompileExecutableFile(CompilerParameters cp, String[] filepaths)
        {
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, filepaths);

            return processCompilationResult(cr);
        }

        private static Assembly processCompilationResult(CompilerResults cr)
        {
            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                //Console.WriteLine( "Errors building {0} into {1}", sourceName, cr.PathToAssembly );
                Console.WriteLine("Errors building code:");
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
            }
            else
            {
                // Display a successful compilation message.
                Console.WriteLine("Source built successfully.");
            }


            // Return the results of the compilation.
            if (cr.Errors.Count == 0)
            {
                return cr.CompiledAssembly;
            }
            return null;
        }
    }

}