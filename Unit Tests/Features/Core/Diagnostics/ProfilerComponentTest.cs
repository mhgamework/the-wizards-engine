using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Diagnostics
{
    [TestFixture]
    public class ProfilerComponentTest
    {
        [Test]
        public void TestUserControl()
        {
            var disp = new  Diagnostics.Profiling.ProfilerDisplay();
            var app = new Application();

            var window = new Window();
            window.Content = disp;

            app.Run(window);
        }
    }
}
