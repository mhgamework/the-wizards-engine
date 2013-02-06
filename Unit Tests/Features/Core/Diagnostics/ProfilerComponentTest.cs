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

            var t = new Thread(delegate()
                {
                    while (Thread.CurrentThread.IsAlive)
                    {
                        root.Begin();
                        stupidFunc();
                        root.End();
                        component.Update(root);
                        Thread.Sleep(3000);
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
            var t = new ProfilingTest();
            t.TestProfiling();
            component.ShowResultsFrom(t.Root);

        }
        [Test]
        public void TestStartEndMeasurement()
        {
            var t = new ProfilingTest();
            component.SetMeasurementPoint(t.Root);
            component.StartMeasurement();

            t.TestProfiling();

            component.EndMeasurement();


        }
        [Test]
        public void TestTakeSnapshot()
        {
            //TODO: Maybe use engine integration for this feature?
            var t = new ProfilingTest();
            component.SetMeasurementPoint(t.Root);
            component.TakeSnapshot(); // Should wait until a single execution is completed
            
            t.TestProfiling();
            


        }
        [Test]
        public void TestEnableDisableProfiling()
        {
            var t = new ProfilingTest();
            component.DisableProfiling();
            t.TestProfiling();
            component.ShowResultsFrom(t.Root);
            Thread.Sleep(1000);
            component.EnableProfiling();
            t.TestProfiling();
            component.ShowResultsFrom(t.Root);

        }

        [Test]
        public void TestResetToRoot()
        {
            var p1 = MockRepository.GenerateStub<ProfilingPoint>();
            var p2 = MockRepository.GenerateStub<ProfilingPoint>();
            component.SetRootPoint(p1);
            component.SetMeasurementPoint(p2);
            component.ResetMeasurementPoint();


            Assert.AreEqual(p1,component.MeasurementPoint);

        }

        public void TestSetMeasurementPoint()
        {
            var p1 = MockRepository.GenerateStub<ProfilingPoint>();
            
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
