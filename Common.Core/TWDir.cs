using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// This is a static class that encapsulates The Wizards filestructure
    /// </summary>
    public static class TWDir
    {
        private static DirectoryInfo rootDirectory;

        /// <summary>
        /// Set by application. Default is PARENT OF startup path.
        /// </summary>
        public static DirectoryInfo RootDirectory
        {
            get { return rootDirectory; }
            set
            {
                rootDirectory = value;
                initDirectories();
            }
        }

        public static DirectoryInfo Binaries { get; private set; }
        public static DirectoryInfo Test { get; private set; }
        public static DirectoryInfo Cache { get; private set; }
        public static DirectoryInfo GameData { get; private set; }
        public static DirectoryInfo Scripts { get; private set; }

        static TWDir()
        {
            var fi = new FileInfo(Assembly.GetExecutingAssembly().Location);

            rootDirectory = fi.Directory.Parent;

            initDirectories();
        }

        private static void initDirectories()
        {
            Binaries = RootDirectory.CreateSubdirectory("Binaries");
            Test = RootDirectory.CreateSubdirectory("Test");
            Cache = RootDirectory.CreateSubdirectory("Cache");
            GameData = RootDirectory.CreateSubdirectory("GameData");
            Scripts = RootDirectory.CreateSubdirectory("Scripts");
        }

        public static string GenerateRandomCacheFile(string path, string extension)
        {
            return Cache + "\\path\\CacheFile" + (new Random()).Next(0, 10000000) + "." + extension;
        }
    }
}
