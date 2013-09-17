using System.Threading;
using MHGameWork.TheWizards.Engine;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.SkyMerchant.SimulationPausing
{
    /// <summary>
    /// Simple example that runs a simulation function in a seperate thread so that it can be paused.
    /// Pressing enter resumes the pause, and hello is displayed each step.
    /// </summary>
    public class SimplePausingExample
    {
        private TWEngine engine;
        private Thread thread;
        private object simulationThreadObject = new object();
        private bool canRunSimulation;

        public SimplePausingExample(TWEngine engine)
        {
            this.engine = engine;
        }

        public void RunExample()
        {
            thread = new Thread(simulationThread);
            thread.Start();

            engine.AddSimulator(new BasicSimulator(simulateWrapper));

        }

        private void simulate()
        {
            System.Console.WriteLine("Hello!");
            pauseUntilKey();
        }

        private void pauseUntilKey()
        {
            while (!TW.Graphics.Keyboard.IsKeyPressed(Key.Return))
            {
                // Should have lock already
                lock (simulationThreadObject)
                {
                    canRunSimulation = false;
                    Monitor.Pulse(simulationThreadObject);


                    // Duplicate code from the simulationthread!
                    while (!canRunSimulation && TW.Graphics.Device.ComPointer != System.IntPtr.Zero) // TW.Graphics.Running seems to be bugged!
                        Monitor.Wait(simulationThreadObject, 200);

                    if (TW.Graphics.Device.ComPointer == System.IntPtr.Zero) return;
                }
            }
        }

        private void simulateWrapper()
        {
            lock (simulationThreadObject)
            {
                canRunSimulation = true;
                Monitor.Pulse(simulationThreadObject);

                while (canRunSimulation)
                    Monitor.Wait(simulationThreadObject);

            }
        }

        private void simulationThread()
        {
            lock (simulationThreadObject)
            {
                while (TW.Graphics.Running)
                {
                    while (!canRunSimulation && TW.Graphics.Device.ComPointer != System.IntPtr.Zero) // TW.Graphics.Running seems to be bugged!
                        Monitor.Wait(simulationThreadObject, 200);

                    if (TW.Graphics.Device.ComPointer == System.IntPtr.Zero) return;

                    simulate();

                    canRunSimulation = false;
                    Monitor.Pulse(simulationThreadObject);
                }

            }
        }

    }
}