using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.Profiling
{
    /// <summary>
    /// Responsible for representing a code unit that gets profiled.
    /// This class implements a profiling point that tracks the total time spend in a method, including recursive calls.
    /// Recursive calls to the same area are ignored(but are counted in the total for the root of the recursive call).
    /// Each area contains a list of the direct subpoints that were accessed. Note that execution path is not considered anywhere, 
    /// so the sum of the total time of the children can be greater than the total time of the parent, since children can also be called on
    /// other execution paths.
    /// 
    /// This profiling point should be used to examine the total time spend. 
    /// 
    /// A root point can be provided, all profiling not inside the bounds of this root point are ignored.
    /// 
    /// NOTE: if another type of profiling area is to be added, don't change this class but use the profiler abstract factory to change behaviour
    /// </summary>
    [Serializable]
    public class ProfilingPoint
    {

        /// <summary>
        /// Static data for traversal
        /// </summary>
        private static Stack<ProfilingPoint> pointStack = new Stack<ProfilingPoint>();
        private static bool hasEnteredRoot = false;

        /// <summary>
        /// Instance data
        /// </summary>
        private readonly Profiler profiler;
        public string Name { get; private set; }

        private TimeSpan start;
        private bool isRoot = false;
        private int depth;
        private int currentProfileNumber = -1;

        public float TotalTime { get; private set; }
        public HashSet<ProfilingPoint> NonRecursiveChildren { get; private set; }
        public int TimesEnteredNonRecursive { get; private set; }

        public event Action Started;
        public event Action Ended;

        public ProfilingPoint(Profiler profiler, string name)
        {
            NonRecursiveChildren = new HashSet<ProfilingPoint>();
            TotalTime = -1;
            this.profiler = profiler;
            this.Name = name;
        }




        public void Begin()
        {
            if (shouldSkip()) return;

            ProfilingPoint prev = null;
            if (pointStack.Count != 0) prev = pointStack.Peek();
            pointStack.Push(this);
            log.Add(this);

            checkNeedsReset(); // Check if the data needs a reset

            depth++;

            if (depth != 1) return;
            startProfiling();
            if (prev != null) prev.NonRecursiveChildren.Add(this);

        }


        private static List<object> log = new List<object>();
        public void End()
        {
            if (shouldSkip()) return;

            if (pointStack.Peek() != this) throw new InvalidOperationException(); // Bug in algo!
            pointStack.Pop();
            
            depth--;

            if (depth == 0) endProfiling();
        }


        private void startProfiling()
        {
            tryEnterRoot();

            start = Configuration.Timer.Elapsed; // Only set start on entry
            if (Started != null) Started();
        }
        private void endProfiling()
        {
            log.Clear();
            // Now we are not inside another execution of this profiling area, store the elapsed
            tryExitRoot();

            storeMeasurements();
            if (Ended != null) Ended();

        }

        private void storeMeasurements()
        {
            var duration = (float)(Configuration.Timer.Elapsed - start).TotalSeconds; // calculate duration


            TotalTime += duration;
            TimesEnteredNonRecursive++;
        }


        private void tryEnterRoot()
        {
            if (this != profiler.ProfilingRoot) return;

            hasEnteredRoot = true;
            isRoot = true;
        }
        private void tryExitRoot()
        {
            if (!isRoot) return;

            hasEnteredRoot = false;
            isRoot = false;

            
        }



        private void checkNeedsReset()
        {
            if (currentProfileNumber == profiler.ProfileNumber) return;
            currentProfileNumber = profiler.ProfileNumber;

            TotalTime = 0;
            NonRecursiveChildren.Clear();
            TimesEnteredNonRecursive = 0;


        }

        private bool shouldSkip()
        {
            if (!profiler.Enabled) return true;
            if (hasEnteredRoot) return false;
            if (profiler.ProfilingRoot == null) return false;
            if (profiler.ProfilingRoot == this) return false;

            return true;
        }


        /// <summary>
        /// Returns the first point found with given name
        /// </summary>
        /// <returns></returns>
        public ProfilingPoint FindByName(string name)
        {
            if (this.Name == name) return this;
            foreach (var child in NonRecursiveChildren)
            {
                var ret = child.FindByName(name);
                if (ret == null) continue;

                return ret;
            }
            return null;
        }


    }
}
