using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Responsible for hotloading a dll and listening for changes
    /// </summary>
    public class AssemblyHotloader
    {
        private readonly FileInfo file;
        public event Action Changed;

        public AssemblyHotloader(FileInfo file)
        {
            this.file = file;
            startFilesystemWatcher();
        }

        private void startFilesystemWatcher()
        {
            var watcher = new FileSystemWatcher(file.Directory.FullName);
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);

            watcher.EnableRaisingEvents = true;
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == file.FullName)
                Changed();
        }

        /// <summary>
        /// Loads a copy of the assembly to hotload, so that the file is not blocked by the engine
        /// </summary>
        /// <returns></returns>
        public Assembly LoadCopied()
        {
            //var tempFile = Path.GetTempFileName();
            //File.Copy(file.FullName, tempFile + ".dll", true);
            //File.Copy(Path.ChangeExtension(file.FullName, "pdb"), tempFile + ".pdb", true);

            var srcDll = file.FullName;
            var srcPdb = Path.ChangeExtension(file.FullName, "pdb");

            var r = new Random();
            var tempFolder = TWDir.Cache.CreateSubdirectory(r.Next().ToString());
            var dll = tempFolder.FullName + "\\" + Path.GetFileName(srcDll);
            var pdb = tempFolder.FullName + "\\" + Path.GetFileName(srcPdb);

            File.Copy(srcDll,dll);
            File.Copy(srcPdb,pdb);

            return Assembly.LoadFile(dll);
        }


    }
}
