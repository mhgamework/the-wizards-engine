using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Diagnostics.Profiling;
using MHGameWork.TheWizards.Profiling;
using MHGameWork.TheWizards.Tests.Features.Core.Profiling;
using NUnit.Framework;
using Rhino.Mocks;

namespace MHGameWork.TheWizards.Diagnostics
{
    [TestFixture, RequiresSTA]
    public class ProfilerComponentTest
    {
        [SetUp]
        public void Setup()
        {
            component = new ProfilerComponent();
        }

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

            var root = Profiler.CreateElement("TestProfilerComponent.Loop");

            component.SetRootPoint(root);

            var t = new Thread(delegate()
                {
                    var test = createProfilingTest();
                    while (Thread.CurrentThread.IsAlive)
                    {
                        root.Begin();
                        stupidFunc();
                        test.main();
                        root.End();

                    }
                });
            t.IsBackground = true;
            t.Start();

            var app = new Application();
            app.Run(new Window { Content = component.GetView() });
        }

        [Test]
        public void TestSetViewData()
        {
            var t = createProfilingTest();
            t.Execute100Mains();
            component.ShowResultsFrom(t.Root);

        }
        [Test]
        public void TestStartEndMeasurement()
        {
            var t = createProfilingTest();
            component.SetMeasurementPoint(t.Root);
            component.StartMeasurement();

            t.Execute100Mains();

            component.EndMeasurement();


        }
        [Test]
        public void TestTakeSnapshot()
        {

            var t = createProfilingTest();
            component.SetMeasurementPoint(t.Root);
            component.TakeSnapshot(); // Should show results once a single execution is completed

            t.Execute100Mains();
        }

        [Test]
        public void TestTakeSnapshotTwice()
        {
            var ev = new AutoResetEvent(false);

            var t = createProfilingTest();
            component.SetMeasurementPoint(t.Root);
            component.TakeSnapshot(); // Should show results once a single execution is completed

            t.Root.Ended += delegate { ev.Set(); };

            t.Execute100Mains();
            ev.WaitOne();

            component.TakeSnapshot();

        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestSingleSnapshot()
        {
            //TODO: Maybe use engine integration for this feature?
            var t = createProfilingTest();
            component.SetMeasurementPoint(t.Root);
            component.TakeSnapshot();
            component.TakeSnapshot();

            t.Execute100Mains();
        }

        private static ProfilingTest createProfilingTest()
        {
            var ret = new ProfilingTest();
            ret.Setup();
            return ret;

        }

        [Test]
        public void TestEnableDisableProfiling()
        {
            var t = new ProfilingTest();
            t.Setup();
            component.DisableProfiling();
            t.Execute100Mains();
            component.ShowResultsFrom(t.Root);
            Thread.Sleep(1000);
            component.EnableProfiling();
            t.Execute100Mains();
            component.ShowResultsFrom(t.Root);

        }

        [Test]
        public void TestResetToRoot()
        {
            var p1 = createProfilingPoint();
            var p2 = createProfilingPoint();
            component.SetRootPoint(p1);
            component.SetMeasurementPoint(p2);
            component.ResetMeasurementPoint();


            Assert.AreEqual(p1, component.MeasurementPoint);

        }

        private ProfilingPoint createProfilingPoint()
        {
            return new ProfilingPoint(null, "qmsldkfj");
        }

        [Test]
        public void TestSetMeasurementPoint()
        {
            var p1 = createProfilingPoint();

            component.SetMeasurementPoint(p1);

            Assert.AreEqual(p1, component.MeasurementPoint);

        }




        Random r = new Random();
        private ProfilerComponent component;

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
        private int blabla(int a)
        {
            for (int i = 0; i < 200; i++)
            {
                a++;

            }
            return a;
        }
    }
}
