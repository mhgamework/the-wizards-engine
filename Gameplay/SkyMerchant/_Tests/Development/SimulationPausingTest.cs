using System;
using System.Diagnostics;
using System.Threading;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant.SimulationPausing;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using NUnit.Framework;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    /// <summary>
    /// Simple testcase of implementing a pause in simulation, by running simulation on a different thread (but not concurrently!!)
    /// </summary>
    [EngineTest]
    [TestFixture]
    public class SimulationPausingTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        /// <summary>
        /// Prints Hello, then pauses, then prints Hello2, then pauses again (in the gameloop)
        /// Pause continues until Return is pressed
        /// TODO: abort the pauser!
        /// </summary>
        [Test]
        public void TestPausingWrapper()
        {
            PausingWrapper wrapper = null;
            wrapper = new PausingWrapper(delegate
                {
                    Console.WriteLine("Hello");
                    while (!TW.Graphics.Keyboard.IsKeyPressed(Key.Return)) wrapper.Pause();
                },new SimpleThreadFactory());

            engine.AddSimulator(new BasicSimulator(wrapper.Execute));
        }

        /// <summary>
        /// TODO: Something misterious happens here, the wrapped method seems to be twice as slow as the normal one.
        /// The unusal thing is that this stays true for any execution length of the original method. One would expect a overhead proportional to the number
        /// of method calls, no to the execution time of the method.
        /// </summary>
        [Test]
        public void TestPausingWrapperPerformance()
        {
            var numFrames = 1000;
            var watch = new Stopwatch();

            var pausable = new PausingWrapper(simulateDummy, new SimpleThreadFactory());


            watch.Reset();
            watch.Start();
            simulateDummy();
            watch.Stop();
            var dummyTime = watch.ElapsedMilliseconds;

            watch.Reset();
            watch.Start();
            for (int i = 0; i < numFrames; i++) simulateDummy();
            watch.Stop();
            var normalTime = watch.ElapsedMilliseconds;

            watch.Reset();
            watch.Start();
            for (int i = 0; i < numFrames; i++) pausable.Execute();

            watch.Stop();

            var pausableTime = watch.ElapsedMilliseconds;

            Console.WriteLine("Dummy time: {0}", dummyTime);
            Console.WriteLine("Normal time: {0}", normalTime);
            Console.WriteLine("Pausable time: {0}", pausableTime);
        }

        private void simulateDummy()
        {
            var sum = 0.0;
            for (int i = 0; i < 100000; i++)
            {
                sum += Math.Sqrt(i);
            }
        }

        /// <summary>
        /// Runs a simple testcase example to test if the threading logic is viable
        /// </summary>
        [Test]
        public void TestSimpleExample()
        {
            var n = new SimplePausingExample(engine);
            n.RunExample();
        }
    }
}