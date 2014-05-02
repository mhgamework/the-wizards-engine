using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Simulation.ActionScheduling;
using SlimDX;

namespace MHGameWork.TheWizards.Testing
{
    /// <summary>
    /// Interface + implementation of this interface using the TWEngine system
    /// This is a colleciton interface, meant to be convenient for testing
    /// </summary>
    public class IRenderingTester
    {
        private readonly TWEngine engine;
        private readonly IActionScheduler scheduler;

        public IRenderingTester(TWEngine engine, IActionScheduler scheduler)
        {
            this.engine = engine;
            this.scheduler = scheduler;

            scheduler.SetCurrentTime(TW.Graphics.TotalRunTime);

            engine.AddSimulator(new BasicSimulator(update));
        }

        private void update()
        {
            scheduler.ExecuteActions(TW.Graphics.TotalRunTime);
        }

        public void SetRepeat(float interval, Action action)
        {
            Action loopedAction = null;

            loopedAction = () =>
                {
                    action();
                    SetTimeout(interval, loopedAction);
                };

            SetTimeout(0, loopedAction);
        }
        public void SetTimeout(float interval, Action action)
        {
            scheduler.SetTimeout(interval, action);
        }

        public void SetCameraPosition(Vector3 position, Vector3 lookTarget)
        {
            TW.Graphics.SpectaterCamera.CameraPosition = position;
            TW.Graphics.SpectaterCamera.CameraDirection = Vector3.Normalize(lookTarget - position);
        }
    }
}