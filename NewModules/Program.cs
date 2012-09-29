using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public static class Program
    {
        public static void Main()
        {
            var eng = new Engine.TWEngine();
            eng.GameplayDll = "../../Gameplay/bin/x86/Debug/Gameplay.dll";
            eng.Start();
        }
    }

}
