using System;
using System.Collections;
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



































        #region Copied stuff, no tests yet
        /// <summary>
        /// Extracts filename from full path+filename, cuts of extension
        /// if cutExtension is true. Can be also used to cut of directories
        /// from a path (only last one will remain).
        /// </summary>
        static public string ExtractFilename(string pathFile, bool cutExtension)
        {
            if (pathFile == null)
                return "";

            // Update 2006-09-29: also checking for normal slashes, needed
            // for support reading 3ds max stuff.
            string[] fileName = pathFile.Split(new char[] { '\\', '/' });
            if (fileName.Length == 0)
            {
                if (cutExtension)
                    return CutExtension(pathFile);
                return pathFile;
            } // if (fileName.Length)

            if (cutExtension)
                return CutExtension(fileName[fileName.Length - 1]);
            return fileName[fileName.Length - 1];
        } // ExtractFilename(pathFile, cutExtension)

        /// <summary>
        /// Get directory of path+File, if only a path is given we will cut off
        /// the last sub path!
        /// </summary>
        static public string GetDirectory(string pathFile)
        {
            if (pathFile == null)
                return "";
            int i = pathFile.LastIndexOf("\\");
            if (i >= 0 && i < pathFile.Length)
                // Return directory
                return pathFile.Substring(0, i);
            // No sub directory found (parent of some dir is "")
            return "";
        } // GetDirectory(pathFile)

        /// <summary>
        /// Same as GetDirectory(): Get directory of path+File,
        /// if only a path is given we will cut of the last sub path!
        /// </summary>
        static public string CutOneFolderOff(string path)
        {
            // GetDirectory does exactly what we need!
            return GetDirectory(path);
        } // CutOneFolderOff(path)

        /// <summary>
        /// Splits a path into all parts of its directories,
        /// e.g. "maps\\sub\\kekse" becomes
        /// {"maps\\sub\\kekse","maps\\sub","maps"}
        /// </summary>
        static public string[] SplitDirectories(string path)
        {
            ArrayList localList = new ArrayList();
            localList.Add(path);
            do
            {
                path = CutOneFolderOff(path);
                if (path.Length > 0)
                    localList.Add(path);
            } while (path.Length > 0);

            return (string[])localList.ToArray(typeof(string));
        } // SplitDirectories(path)

        /// <summary>
        /// Remove first directory of path (if one exists).
        /// e.g. "maps\\mymaps\\hehe.map" becomes "mymaps\\hehe.map"
        /// Also used to cut first folder off, especially useful for relative
        /// paths. e.g. "maps\\test" becomes "test"
        /// </summary>
        static public string RemoveFirstDirectory(string path)
        {
            int i = path.IndexOf("\\");
            if (i >= 0 && i < path.Length)
                // Return rest of path
                return path.Substring(i + 1);
            // No first directory found, just return original path
            return path;
        } // RemoveFirstDirectory(path)

        /// <summary>
        /// Check if a folder is a direct sub folder of a main folder.
        /// True is only returned if this is a direct sub folder, not if
        /// it is some sub folder few levels below.
        /// </summary>
        static public bool IsDirectSubfolder(string subfolder, string mainFolder)
        {
            // First check if subFolder is really a sub folder of mainFolder
            if (subfolder != null &&
                subfolder.StartsWith(mainFolder))
            {
                // Same order?
                if (subfolder.Length < mainFolder.Length + 1)
                    // Then it ain't a sub folder!
                    return false;
                // Ok, now check if this is direct sub folder or some sub folder
                // of mainFolder sub folder
                string folder = subfolder.Remove(0, mainFolder.Length + 1);
                // Check if this is really a direct sub folder
                for (int i = 0; i < folder.Length; i++)
                    if (folder[i] == '\\')
                        // No, this is a sub folder of mainFolder sub folder
                        return false;
                // Ok, this is a direct sub folder of mainFolder!
                return true;
            } // if (subFolder)
            // Not even any sub folder!
            return false;
        } // IsDirectSubFolder(subFolder, mainFolder)

        /// <summary>
        /// Cut of extension, e.g. "hi.txt" becomes "hi"
        /// </summary>
        static public string CutExtension(string file)
        {
            if (file == null)
                return "";
            int l = file.LastIndexOf('.');
            if (l > 0)
                return file.Remove(l, file.Length - l);
            return file;
        } // CutExtension(file)

        /// <summary>
        /// Get extension (the stuff behind that '.'),
        /// e.g. "test.bmp" will return "bmp"
        /// </summary>
        static public string GetExtension(string file)
        {
            if (file == null)
                return "";
            int l = file.LastIndexOf('.');
            if (l > 0 && l < file.Length)
                return file.Remove(0, l + 1);
            return "";
        } // GetExtension(file)
        #endregion


    }
}