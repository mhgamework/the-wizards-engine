using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Profiling
{
    /// <summary>
    /// Responsible for managing profiling points throughout the application and calculating elapsed time.
    /// </summary>
    public class Profiler
    {
        private static Profiler instance = new Profiler();


        public ProfilingPoint ProfilingRoot { get; set; }
        /// <summary>
        /// The id of the current Profile task
        /// </summary>
        public int ProfileNumber { get; set; }

        public bool Enabled { get; set; }


        public Profiler()
        {
            Enabled = true;

        }


        public static ProfilingPoint CreateElement(string name)
        {
            var el = new ProfilingPoint(instance, name);

            return el;
        }

        /// <summary>
        /// Resets all measured data
        /// </summary>
        public static void ResetAll()
        {
            instance.ProfileNumber++;
        }

        /// <summary>
        /// Problems when changing this in-profile!
        /// </summary>
        /// <param name="value"></param>
        public static void SetProfilingEnabled(bool value)
        {
            instance.Enabled = value;
        }




    }
}
