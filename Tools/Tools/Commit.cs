using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Tools
{
    public class Commit : ITool
    {
        public void Execute()
        {
            TortoiseProc.Do(TortoiseProc.Command.Commit, Config.TheWizardsRoot);
        }
    }
}
