using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MHGameWork.TheWizards.Common.Core
{
    public class EmbeddedFile
    {

        public static string DebugFilesDirectory;

        static EmbeddedFile()
        {
            DebugFilesDirectory = System.Windows.Forms.Application.StartupPath + "/DebugFiles";
        }

        /// <summary>
        /// Looks if the given file 'filenameDisk' exists on the disk, if it does, return a filestream.
        /// Otherwise use the embedded file with 'filenameAssembly' in the assembly calling this function.
        /// This allows for runtime overriding the files (for ex shader testing), without recompiling.
        /// </summary>
        /// <param name="filenameAssembly"></param>
        /// <param name="filenameDisk"></param>
        [Obsolete("This is not bad, but it is better to use the new function that uses relative paths and the DebugFilesDirectory.")]
        public static Stream GetStreamFullPath(string filenameAssembly, string filenameDisk)
        {
            return GetStreamFullPath(System.Reflection.Assembly.GetCallingAssembly(), filenameAssembly, filenameDisk);
        }
        /// <summary>
        /// Looks if the given file 'filenameDisk' exists on the disk, if it does, return a filestream.
        /// Otherwise use the embedded file with 'filenameAssembly' in the assembly calling this function.
        /// This allows for runtime overriding the files (for ex shader testing), without recompiling.
        /// </summary>
        /// <param name="filenameAssembly"></param>
        /// <param name="filenameDisk"></param>
        /// <returns></returns>
        [Obsolete("This is not bad, but it is better to use the new function that uses relative paths and the DebugFilesDirectory.")]
        public static Stream GetStreamFullPath(System.Reflection.Assembly assembly, string filenameAssembly, string filenameDisk)
        {
            if (File.Exists(filenameDisk))
            {
                System.Console.WriteLine("Debug: Loading '{0}' debug version instead of embedded file in assembly '{1}'", filenameDisk, filenameAssembly);
                return new FileStream(filenameDisk, FileMode.Open, FileAccess.Read);
            }
            else
            {
                Stream strm = assembly.GetManifestResourceStream(filenameAssembly);
                if (strm == null) throw new Exception("Given file not found on disk or in the assembly!");
                return strm;
            }
        }



        /// <summary>
        /// Looks if the given file 'filenameDisk' exists in the DebugFilesFolder, if it does, returns a filestream.
        /// Otherwise use the embedded file with 'filenameAssembly' in the assembly calling this function.
        /// This allows for runtime overriding the files (for ex shader testing), without recompiling.
        /// </summary>
        /// <param name="filenameAssembly"></param>
        /// <param name="filenameDisk"></param>
        public static Stream GetStream(string filenameAssembly, string filenameDisk)
        {
            //I removed this because i thought it was a mistake:
            //return GetStreamFullPath(System.Reflection.Assembly.GetCallingAssembly(), filenameAssembly, filenameDisk);
            return GetStream(System.Reflection.Assembly.GetCallingAssembly(), filenameAssembly, filenameDisk);
        }
        /// <summary>
        /// Looks if the given file 'filenameDisk' exists in the DebugFilesFolder, if it does, returns a filestream.
        /// Otherwise use the embedded file with 'filenameAssembly' in the assembly calling this function.
        /// This allows for runtime overriding the files (for ex shader testing), without recompiling.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="filenameAssembly"></param>
        /// <param name="filenameDisk"></param>
        /// <returns></returns>
        public static Stream GetStream(System.Reflection.Assembly assembly, string filenameAssembly, string filenameDisk)
        {
            if (filenameDisk != null)
            {
                filenameDisk = DebugFilesDirectory + "/" + filenameDisk;
                if (File.Exists(filenameDisk))
                {
                    Console.WriteLine("Debug: Loading '{0}' debug version instead of embedded file in assembly '{1}'", filenameDisk, filenameAssembly);
                    return new FileStream(filenameDisk, FileMode.Open, FileAccess.Read);
                }
            }

            return GetStream(assembly, filenameAssembly);

        }

        public static Stream GetStream(string filenameAssembly)
        {
            return GetStream(System.Reflection.Assembly.GetCallingAssembly(), filenameAssembly);
        }
        public static Stream GetStream(System.Reflection.Assembly assembly, string filenameAssembly)
        {
            Stream strm = assembly.GetManifestResourceStream(filenameAssembly);

            if (strm == null) throw new Exception("Given file not found on disk or in the assembly!");
            return strm;
        }




    }
}
