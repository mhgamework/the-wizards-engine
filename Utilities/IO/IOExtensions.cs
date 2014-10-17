using System;
using System.IO;

namespace MHGameWork.TheWizards.IO
{
    public static class IOExtensions
    {
         public static DirectoryInfo GetChild(this DirectoryInfo parent, string childName)
         {
             var ret = new DirectoryInfo(parent.FullName + "\\" + childName);
             if (!ret.Exists) throw new InvalidOperationException("Child dir does not exist: " + ret.FullName);
             return ret;
         }
         public static DirectoryInfo CreateChild(this DirectoryInfo parent, string childName)
         {
             var ret = new DirectoryInfo(parent.FullName + "\\" + childName);
             ret.Create();
             return ret;
         }
         public static FileInfo GetFile(this DirectoryInfo parent, string fileName)
         {
             var ret = new FileInfo(parent.FullName + "\\" + fileName);
             if (!ret.Exists) throw new InvalidOperationException("File does not exist: " + ret.FullName);
             return ret;
         }
        /// <summary>
        /// TODO: CreateFile is a misnomer, does not actually create a new file
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
         public static FileInfo CreateFile(this DirectoryInfo parent, string fileName)
         {
             var ret = new FileInfo(parent.FullName + "\\" + fileName);
             return ret;
         }
    }
}