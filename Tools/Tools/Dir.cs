using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Tools
{
    public class Dir : ITool
    {
        public void Execute()
        {
            CSRunner.RunExecutable("explorer.exe", Config.TheWizardsRoot);
        }
    }
}
