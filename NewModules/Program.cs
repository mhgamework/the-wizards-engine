using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public static class Program
    {
        public static void Main()
        {
            var eng = new Engine();
            eng.GameplayFolder = new System.IO.DirectoryInfo("../../GamePlay");
            eng.Start();
        }
    }

}
