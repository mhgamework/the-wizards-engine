using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Profiling;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Profiling
{
    [TestFixture]
    public class ProfilingTest
    {
        [Test]
        public void TestProfiling()
        {

            var main = Profiler.CreateElement("Main");
            var rendering = Profiler.CreateElement("Rendering");
            var lights = Profiler.CreateElement("Lights");
            var objects = Profiler.CreateElement("Objects");
            var animation = Profiler.CreateElement("Animation");
            var physics = Profiler.CreateElement("Physics");

            for (int i = 0; i < 1000; i++)
            {
                main.Begin();
                Thread.Sleep(1);
                
                rendering.Begin();
                Thread.Sleep(1);
                lights.Begin();
                Thread.Sleep(3);
                lights.End();
                objects.Begin();
                Thread.Sleep(5);
                objects.End();
                rendering.End();
                
                physics.Begin();
                Thread.Sleep(2);
                physics.End();
                
                animation.Begin();
                Thread.Sleep(1);
                animation.End();

                Thread.Sleep(1);
                main.End();
            }

            var result = main.GenerateProfileString();
        }

    }
}
