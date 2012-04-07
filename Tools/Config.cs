using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tools
{
    public static class Config
    {
        public static string TheWizardsRoot
        {get { return Directory.GetParent(Environment.CurrentDirectory).FullName; }}

        public static string TortoiseProc
        {get { return @"C:\Program Files\TortoiseGit\bin\TortoiseProc.exe"; }}

        public static string GitSh
        { get { return @"C:\Program Files (x86)\Git\bin\sh.exe"; } }
    }
}
