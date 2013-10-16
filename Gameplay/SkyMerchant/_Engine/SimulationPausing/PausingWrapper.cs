using System;
using System.Threading;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.SimulationPausing
{
    /// <summary>
    /// Responsible for wrapping a method so that the wrapped method can pause it's execution.
    /// The method which is wrapped by this class is called 'Little'. The method that is called on this wrapper (so the wrapper) is called 'Big'
    /// Big calls little. When little returns, Big returns.
    /// When Little calls pause, Big returns immediately. When Big is called again, Little is resumed where it previously called pause.
    /// 
    /// TODO: add aborting?
    /// </summary>
    public class PausingWrapper
    {
        private readonly Action pausableMethod;
        private Thread thread;
        private object syncObject = new object();
        private bool canRunSimulation = false;
        private bool executingWrappedMethod = false;

        public PausingWrapper(Action pausableMethod, IThreadFactory simpleThreadFactory)
        {
            this.pausableMethod = pausableMethod;

            //Note: this is deliberately not a background thread, since this could potentially cause slowdowns due to the OS not scheduling this thread fast enough
            thread = simpleThreadFactory.CreateThread(simulationThread);
            thread.Start();

            
        }

        /// <summary>
        /// To be called from the pausableMethod 
        /// TODO: maybe add runtime check to verify this is called from within the pausable method?
        /// </summary>
        public void Pause()
        {
            lock (syncObject)
            {
                if (!executingWrappedMethod) throw new InvalidOperationException("Only call this from within the wrapped pausable method");
                onWrappedMethodDone();
                waitForInvocation();
            }

        }

        private void waitForInvocation()
        {
            lock (syncObject)
            {
                while (!canRunSimulation)
                    Monitor.Wait(syncObject);

                executingWrappedMethod = true;

            }

        }

        private void onWrappedMethodDone()
        {
            // Should have lock already
            lock (syncObject)
            {
                executingWrappedMethod = false;
                canRunSimulation = false;
                Monitor.Pulse(syncObject);
            }
        }

        /// <summary>
        /// Runs the method (or resumes in case of pausing)
        /// </summary>
        public virtual void Execute()
        {
            lock (syncObject)
            {
                canRunSimulation = true;
                Monitor.Pulse(syncObject);

                while (canRunSimulation)
                    Monitor.Wait(syncObject);

            }
        }

        private void simulationThread()
        {
            lock (syncObject)
            {
                while (true)
                {
                    waitForInvocation();

                    invokeWrappedMethod();

                    onWrappedMethodDone();
                }
            }
        }

        private void invokeWrappedMethod()
        {
            lock (syncObject)
            {
                pausableMethod();
            }

        }
    }
}