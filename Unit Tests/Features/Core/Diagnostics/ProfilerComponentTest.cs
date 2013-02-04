using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Diagnostics.Profiling;
using MHGameWork.TheWizards.Profiling;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Diagnostics
{
    [TestFixture, RequiresSTA]
    public class ProfilerComponentTest
    {
        [Test]
        public void TestUserControl()
        {
            var disp = new Diagnostics.Profiling.ProfilerDisplay();
            var app = new Application();

            app.Run(new Window { Content = disp });
        }

        [Test]
        public void TestProfilerComponent()
        {
            var comp = new ProfilerComponent();

            var root = Profiler.CreateElement("TestProfilerComponent.Loop");

            var t = new Thread(delegate()
                {
                    while (Thread.CurrentThread.IsAlive)
                    {
                        root.Begin();
                        stupidFunc();
                        root.End();
                        comp.Update(root);
                        Thread.Sleep(3000);
                    }
                });
            t.IsBackground = true;
            t.Start();

            var app = new Application();
            app.Run(new Window { Content = comp.GetView() });
        }

        Random r = new Random();
        [TWProfile]
        private void stupidFunc(int depth)
        {
            if (depth == 0)
            {
                stupidFunc();
                return;
            }

            int a = 0;
            for (int i = 0; i < r.Next(3, 8); i++)
            {
                stupidFunc(depth - 1);
            }

        }
        [TWProfile]
        private void stupidFunc()
        {
            int a = 0;
            for (int i = 0; i < r.Next(10000, 100000); i++)
            {
                a = blabla(a);
            }
        }

        [TWProfile]
        private  int blabla(int a)
        {
            for (int i = 0; i < 200; i++)
            {
                a++;
                
            }
            return a;
        }
    }
}
