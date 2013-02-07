using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.DirectX11
{
    [TestFixture]
    public class DX11PerformanceTest
    {
        private const int testDuration = 5;
        private float? start;
        private float current;
        private int count;

        [SetUp]
        public void Setup()
        {
            start = (float?)null;
            current = 0f;
            count = 0;
        }
        [Test]
        public void TestForm()
        {

            var form = new DX11Form();
            form.GameLoopEvent += delegate
                {
                    onEnterFrame();
                    if (getTotalTime() > testDuration) form.Exit();
                };

            form.Run();
            printResults();
        }

        private void printResults()
        {
            Console.WriteLine("Average FPS: {0}", count/(current - start));
            Console.WriteLine("Average Frame (ms): {0}", (current - start)/count*1000);
        }

        [Test]
        public void TestGame()
        {

            var form = new DX11Game();
            form.GameLoopEvent += delegate
            {
                onEnterFrame();
                if (getTotalTime() > testDuration) form.Exit();
            };

            form.Run();
            printResults();

        }

        private float getTotalTime()
        {
            return current - start.Value;
        }

        private void onEnterFrame()
        {
            count++;
            current = (float)SlimDX.Configuration.Timer.Elapsed.TotalSeconds;

            if (!start.HasValue) start = current;
        }
    }
}
