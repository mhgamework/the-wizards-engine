using System;
using System.Collections.Generic;
using System.Linq;

namespace MHGameWork.TheWizards.Simulation.ActionScheduling
{
    /// <summary>
    /// Gameloop-based action scheduler and executor
    /// TODO: create optimized implementation
    /// </summary>
    public class IActionScheduler
    {
        private List<ScheduledAction> actions = new List<ScheduledAction>();

        private float currentTime;

        public void SetTimeout(float interval, Action action)
        {
            actions.Add(new ScheduledAction(currentTime + interval, action));
        }

        public void SetCurrentTime(float newTime)
        {
            currentTime = newTime;
        }

        public void ExecuteActions(float newTime)
        {
            var expiredActions = actions.Where(a => a.Time <= newTime).ToArray();

            currentTime = newTime;


            foreach (var a in expiredActions)
            {
                actions.Remove(a);
                a.Action();
            }

        }

        private class ScheduledAction
        {
            public float Time;
            public Action Action;

            public ScheduledAction(float time, Action action)
            {
                Time = time;
                Action = action;
            }
        }
    }
}