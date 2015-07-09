using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Data;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Thread.Sleep(2000); //TODO: why is this here?
            var eng = new Engine.TWEngine();
            //eng.GameplayDll = "..Gameplay.dll";
            eng.HotloadingEnabled = true;
            eng.Run();
        }

    }

}
