using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace MHGameWork.TheWizards.Launcher
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {

            var rootDir = new FileInfo(Assembly.GetExecutingAssembly().Location);

            TWDir.RootDirectory = rootDir.Directory;

            var main = new LauncherMain(rootDir.Directory.FullName);

            main.AddServer(new System.Net.IPEndPoint(IPAddress.Parse("5.149.17.16"), 15014));
            main.AddServer(new System.Net.IPEndPoint(IPAddress.Parse("5.23.165.201"), 15014));

            main.Run();
        }
    }
}
