using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Tools
{
    public class Log : ITool
    {
        public void Execute()
        {
            TortoiseProc.Do(TortoiseProc.Command.Log, Config.TheWizardsRoot);
        }
    }
}
