using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// Contains helper functions for file-path manipulation
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Returns true when fi is in di or in one of its subdirectories
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="di"></param>
        /// <returns></returns>
        public static bool IsFileInDirectory(FileInfo fi, DirectoryInfo di)
        {

            if (fi.Directory.FullName == di.FullName)
                return true;

            return IsChildFolder(fi.Directory, di);


        }

        /// <summary>
        /// Returns true when childFolder is a subfolder of parentFolder
        /// </summary>
        /// <param name="childFolder"></param>
        /// <param name="parentFolder"></param>
        /// <returns></returns>
        public static bool IsChildFolder(DirectoryInfo childFolder, DirectoryInfo parentFolder)
        {
            if (childFolder.Parent == null)
                return false;
            childFolder = childFolder.Parent;

            if (childFolder.FullName == parentFolder.FullName)
                return true;

            return IsChildFolder(childFolder, parentFolder);
        }
    }
}