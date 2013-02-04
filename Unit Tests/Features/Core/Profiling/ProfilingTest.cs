using System;
using System.Linq;
using System.Threading;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Profiling;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Core.Profiling
{
    [TestFixture]
    public class ProfilingTest
    {
        private ProfilingPoint root;

        [SetUp]
        public void Setup()
        {
            root = Profiler.CreateElement("[ROOT]");
            Profiler.SetProfilingEnabled(true);
        }

        [Test]
        public void TestProfiling()
        {
            root.Begin();
            for (int i = 0; i < 100; i++) { main(); }
            root.End();
            printRootResult(9);

        }

        [Test]
        public void TestProfilingMultiple()
        {
            TestProfiling();
            Profiler.ResetAll();
            TestProfiling();
        }

        [Test]
        public void TestRecursive()
        {
            var time = SlimDX.Configuration.Timer.Elapsed;
            root.Begin();
            for (int i = 0; i < 100; i++) { recursive(5); }

            root.End();

            var diff = SlimDX.Configuration.Timer.Elapsed - time;
            Console.WriteLine(diff);

            printRootResult(2);
        }

        [Test]
        public void TestDisable()
        {
            Console.WriteLine("Enabled");
            Profiler.SetProfilingEnabled(true);
            TestProfiling();

            Console.WriteLine("Disabled");
            Profiler.SetProfilingEnabled(false);
            TestProfiling();
        }

        [TWProfile("Main")]
        private void main()
        {
            Thread.Sleep(1);
            rendering();
            physics();
            animation();
            Thread.Sleep(1);
        }

        [TWProfile("Animation")]
        private void animation() { Thread.Sleep(1); }
        [TWProfile("Physics")]
        private void physics() { Thread.Sleep(2); }

        [TWProfile("Rendering")]
        private void rendering()
        {
            Thread.Sleep(1);
            lights();
            objects();
        }
        [TWProfile("Objects")]
        private void objects() { Thread.Sleep(5); }
        [TWProfile("Lights")]
        private void lights() { Thread.Sleep(3); }

        [TWProfile("Recursive")]
        private void recursive(int depth)
        {
            Thread.Sleep(4);
            if (depth > 1)
                recursive(--depth);
        }

        private void executeProfiled(ProfilingPoint point, Action func)
        {
            point.Begin();
            func();
            point.End();
        }

        private void printRootResult(int expectedLines)
        {
            var result = ProfilingStringGenerator.GenerateProfileString(root);
            Console.WriteLine(result);
            Assert.AreEqual(expectedLines, result.Count(c => c == '\n'));
        }


    }
}
