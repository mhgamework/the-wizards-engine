using System;
using System.IO;

namespace Tools.Tools
{
    public class BuildServer : ITool
    {
        public void Execute()
        {
            var root = Directory.GetParent(Environment.CurrentDirectory);

            var serverDir = root.CreateSubdirectory("build").CreateSubdirectory("Server");

            serverDir.Delete(true);
            serverDir.Create();


            var binaries = serverDir.CreateSubdirectory("Binaries");
            foreach (var fi in root.CreateSubdirectory("bin").CreateSubdirectory("Binaries").EnumerateFiles())
            {
                fi.CopyTo(binaries.FullName + "\\" + fi.Name);
                Console.WriteLine("Copying Binaries\\{0}", fi.Name);
            }

            var gamedataCore = serverDir.CreateSubdirectory("GameData").CreateSubdirectory("Core");
            var oriDir = root.CreateSubdirectory("bin").CreateSubdirectory("GameData").CreateSubdirectory("Core");
            foreach (var fi in oriDir.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                var relativePath = fi.FullName.Substring(oriDir.FullName.Length);
                var target = new FileInfo(gamedataCore.FullName + relativePath);
                target.Directory.Create();
                fi.CopyTo(target.FullName);
                Console.WriteLine("Copying GameData\\Core{0}", relativePath);
            }



            Console.WriteLine("Server build successfully!");
        }
    }
}
