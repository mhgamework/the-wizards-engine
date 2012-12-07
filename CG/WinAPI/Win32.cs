using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MHGameWork.TheWizards.CG.WinAPI
{
    public static class Win32
    {
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric smIndex);
    }
}
