using System;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Several usefull extension for pretty printing 
    /// </summary>
    public static class PrettyPrintExtensions
    {
        public static string PrettyPrint(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds > 1000) throw new InvalidOperationException("Only for less than 1000 second intervals(more not yet implemented)");
            var secs = timeSpan.TotalSeconds;
            if (secs > 1)
                return string.Format("{0:0.00} sec", secs);
            secs *= 1000;
            if (secs > 1)
                return string.Format("{0:0.00} ms", secs);
            secs *= 1000;
            return string.Format("{0:0.00} µs", secs);

        }
    }
}