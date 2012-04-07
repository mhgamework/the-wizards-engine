using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    public class TortoiseProc
    {
        public static void Do(Command cmd, string path)
        {
            CSRunner.RunExecutable(Config.TortoiseProc, "/command:" + cmd.ToString().ToLower() + " /path:\"" + path + "\"",false);
        }
        public enum Command
        {
            None,
            Commit,
            Log
        }
    }
}
