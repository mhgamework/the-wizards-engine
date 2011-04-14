using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace MHGameWork.TheWizards.Networking
{
    public class AssemblyBuilder
    {
        public static Assembly CompileExecutable( String code )
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

            return CompileExecutable( cp, code );

        }


        public static Assembly CompileExecutable( CompilerParameters cp, String code )
        {
            return CompileExecutable( cp, new string[] { code } );
        }
        public static Assembly CompileExecutable( CompilerParameters cp, String[] code )
        {
            CodeDomProvider provider = null;
            bool compileOk = false;

            // Select the code provider based on the input file extension.
            provider = new Microsoft.CSharp.CSharpCodeProvider();



            // Invoke compilation of the source file.
            CompilerResults cr = provider.CompileAssemblyFromSource( cp, code );

            if ( cr.Errors.Count > 0 )
            {
                // Display compilation errors.
                //Console.WriteLine( "Errors building {0} into {1}", sourceName, cr.PathToAssembly );
                Console.WriteLine( "Errors building code:" );
                foreach ( CompilerError ce in cr.Errors )
                {
                    Console.WriteLine( "  {0}", ce.ToString() );
                    Console.WriteLine();
                }
            }
            else
            {
                // Display a successful compilation message.
                Console.WriteLine( "Source built successfully." );
            }


            // Return the results of the compilation.
            if ( cr.Errors.Count == 0 )
            {
                return cr.CompiledAssembly;
            }
            else
            {
                return null;
            }
        }


    }

}