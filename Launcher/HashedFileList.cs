using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Launcher
{
    public class HashedFileList
    {
        public class File
        {
            public string RelativePath;
            public long SizeInBytes;
            public long ZippedSize;
            public byte[] HashSHA1;

        }

        public List<File> Files = new List<File>();

        /// <summary>
        /// Used in determining the relative path
        /// </summary>
        [XmlIgnore]
        public DirectoryInfo LocalRoot;


        public void AddFolder(DirectoryInfo di, bool recursive)
        {
            var children = di.GetDirectories();
            if (recursive)
                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];
                    AddFolder(child, true);
                }

            var files = di.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                var fi = files[i];

                if (fi.FullName.StartsWith(LocalRoot.FullName + "\\") == false)
                    throw new InvalidOperationException("Files should be inside the LocalRootPath");

                var p = new SHA1CryptoServiceProvider();

                byte[] hash;
                FileStream fs = null;
                try
                {
                    fs = fi.OpenRead();
                    hash = p.ComputeHash(fs);
                    var file = new File();
                    file.HashSHA1 = hash;
                    file.RelativePath = fi.FullName.Substring(LocalRoot.FullName.Length + 1);
                    file.SizeInBytes = fi.Length;

                    Files.Add(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cannot hash file: " + fi.FullName);
                    Console.WriteLine(ex);
                }
                finally
                {
                    if (fs != null)
                        fs.Close();

                }




            }
        }
        public void AddFolder(DirectoryInfo di)
        {

            AddFolder(di, false);
        }

    }
}
