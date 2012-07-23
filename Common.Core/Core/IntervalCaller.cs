using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// This class calls a given delegate at specified interval. Call the Update method to process the delegates
    /// </summary>
    public class IntervalCaller
    {
        public float Interval { get; set; }
        private Action action;
        private float timeSinceCall;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="interval">Interval in seconds</param>
        public IntervalCaller(Action action, float interval)
        {
            Interval = interval;
            this.action = action;
        }

        public void Update(float elapsed)
        {
            timeSinceCall += elapsed;

            int i = 0;
            while (timeSinceCall > Interval)
            {
                if (i > 10) throw new Exception("Too many calls required in IntervalCaller");
                timeSinceCall -= Interval;
                action();
                i++;

            }

        }
    }
}
