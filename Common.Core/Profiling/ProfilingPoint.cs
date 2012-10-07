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
    /// TODO: add support for profiling the same part of code twice sequentially (now old results get overwritten with no visible feedback)
    /// </summary>
    [Serializable]
    public class ProfilingPoint
    {
        private readonly Profiler profiler;
        private readonly string name;

        private List<ProfilingPoint> lastChildren = new List<ProfilingPoint>();
        private float average = -1;

        public ProfilingPoint(Profiler profiler, string name)
        {
            this.profiler = profiler;
            this.name = name;
        }

        private bool started = false;
        private TimeSpan start;

        public List<ProfilingPoint> LastChildren
        {
            get { return lastChildren; }
        }

        public float AverageSeconds
        {
            get { return average; }
        }

        private static int profileCount = 0;

        private int currentProfileCount = -1;

        private int instanceCount = 0;

        private void addChild(ProfilingPoint child)
        {
            lastChildren.Add(child);
        }

        public void Begin()
        {
            if (started) throw new InvalidOperationException("This element has already been started!!");
            started = true;
            start = Configuration.Timer.Elapsed;

            if (currentProfileCount != profileCount)
            {
                instanceCount = 0;

                lastChildren.Clear();

            }

            instanceCount++;


            if (profiler.pointStack.Count > 0)
                profiler.pointStack.Peek().addChild(this);
            profiler.pointStack.Push(this);



        }
        public void End()
        {
            started = false;

            var duration = (float)(Configuration.Timer.Elapsed - start).TotalSeconds;
            if (AverageSeconds < 0)
                average = duration;
            float factor = 1;
            average = AverageSeconds * (1 - factor) + duration * factor;

            profiler.pointStack.Pop();
            if (profiler.pointStack.Count == 0)
            {
                profileCount++;

            }
            if (hasCallback) // Performance trick
                OnProfilingComplete();
        }

        private bool hasCallback = false;
        private List<Action<ProfilingPoint>> callbacks = new List<Action<ProfilingPoint>>();
        public void OnProfilingComplete()
        {

            callbacks.ForEach(c => c(this));
        }

        public void AddProfileCompleteCallback(Action<ProfilingPoint> callback)
        {
            hasCallback = true;
            callbacks.Add(callback);
        }


        public string GenerateProfileString()
        {
            return GenerateProfileString(p => true);
        }
        /// <summary>
        /// Returns a string with recursive profiling information
        /// </summary>
        /// <returns></returns>
        public string GenerateProfileString(Func<ProfilingPoint, bool> filter)
        {
            var builder = new StringBuilder();
            generateProfileString(builder, "", calculateAverageMilliseconds(), filter);
            return builder.ToString();
        }
        private void generateProfileString(StringBuilder builder, string prefix, float parentTime, Func<ProfilingPoint, bool> filter)
        {
            if (!filter(this)) return;
            var ms = calculateAverageMilliseconds();
            appendOutputLine(builder, prefix, ms, parentTime, name, instanceCount);

            float childrenTotal = 0;
            var additionalPrefix = "| ";
            foreach (var child in lastChildren)
            {
                child.generateProfileString(builder, prefix + additionalPrefix, ms, filter);
                childrenTotal += child.calculateAverageMilliseconds();
            }
            var remainder = ms - childrenTotal;
            if (remainder > 0.5 && lastChildren.Count != 0)
                appendOutputLine(builder, prefix + additionalPrefix, remainder, ms, "[...]", 1);

        }

        private void appendOutputLine(StringBuilder builder, string prefix, float ms, float parentTime, string name, int times)
        {
            builder.Append(prefix).Append("|-").AppendFormat(" {0}: {1:#0.#}ms | {2:#0}%    {3} times", name, ms, ms / parentTime * 100, times).AppendLine();
        }



        private float calculateAverageMilliseconds()
        {
            return AverageSeconds * 1000;
        }
    }
}
