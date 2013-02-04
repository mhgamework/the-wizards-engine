using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Profiling
{
    public class ProfilingStringGenerator
    {
        public static string GenerateProfileString(ProfilingPoint profilingPoint)
        {
            return GenerateProfileString(profilingPoint, p => true);
        }

        /// <summary>
        /// Returns a string with recursive profiling information
        /// </summary>
        /// <returns></returns>
        public static string GenerateProfileString(ProfilingPoint profilingPoint, Func<ProfilingPoint, bool> filter)
        {
            var builder = new StringBuilder();
            generateProfileString(profilingPoint, builder, "", calculateAverageMilliseconds(profilingPoint), filter);
            return builder.ToString();
        }

        private static void generateProfileString(ProfilingPoint profilingPoint, StringBuilder builder, string prefix, float parentTime, Func<ProfilingPoint, bool> filter)
        {
            if (!filter(profilingPoint)) return;
            var ms = calculateAverageMilliseconds(profilingPoint);
            appendOutputLine(builder, prefix, ms, parentTime, profilingPoint.Name, profilingPoint.TimesEnteredNonRecursive);

            float childrenTotal = 0;
            var additionalPrefix = "| ";
            foreach (var child in profilingPoint.NonRecursiveChildren)
            {
                generateProfileString(child, builder, prefix + additionalPrefix, ms, filter);
                childrenTotal += calculateAverageMilliseconds(child);
            }
            var remainder = ms - childrenTotal;
            if (remainder / ms > 0.1 && profilingPoint.NonRecursiveChildren.Count != 0)
                appendOutputLine(builder, prefix + additionalPrefix, remainder, ms, "[...]", 1);

        }

        private static void appendOutputLine(StringBuilder builder, string prefix, float ms, float parentTime, string name, int times)
        {
            builder.Append(prefix).Append("|-").AppendFormat(" {0}: {1:#0.00}ms | {2:#0}%    {3} times", name, ms, ms / parentTime * 100, times).AppendLine();
        }




        private static float calculateAverageMilliseconds(ProfilingPoint profilingPoint)
        {
            return profilingPoint.TotalTime * 1000;
        }
    }
}
