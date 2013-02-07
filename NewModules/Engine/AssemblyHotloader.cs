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
            var tempFile = Path.GetTempFileName();

            File.Copy(file.FullName, tempFile + ".dll", true);
            File.Copy(Path.ChangeExtension(file.FullName, "pdb"), tempFile + ".pdb", true);
            //File.Delete(GameplayDll);
            //File.Delete(Path.ChangeExtension(GameplayDll,"pdb"));
            //File.Delete(Path.ChangeExtension(GameplayDll, "pssym"));
            //File.Delete(Path.ChangeExtension(GameplayDll, "dll.config"));
            return Assembly.LoadFile(tempFile + ".dll");
        }


    }
}
