using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tools.Tools
{
    public class CodeMetrics : ITool
    {
        public void Execute()
        {


            var lineCount = 0;
            var fileCount = 0;
            foreach (var file in Directory.EnumerateFiles(Directory.GetParent(Environment.CurrentDirectory).FullName, "*.cs", SearchOption.AllDirectories))
            {
                if (file.Contains("Deprecated")) continue;
                if (file.Contains("_Libraries")) continue;
                if (file.Contains("ColladaSchema")) continue;
                if (file.Contains("_Source\\bin")) continue;
                Console.WriteLine(file);
                fileCount++;

                //if (fileCount % 30 == 0)
                //    Console.ReadLine();

                lineCount += File.ReadAllLines(file).Length;
            }
            Console.WriteLine("Linecount: " + lineCount);
            Console.WriteLine("Filecount: " + fileCount);

            Console.ReadLine();
        }
    }
}
