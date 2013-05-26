using System;
using MHGameWork.TheWizards.Debugging;

namespace MHGameWork.TheWizards.Engine.Diagnostics
{
    /// <summary>
    /// Implements the error logger service for use in the TWEngine
    /// </summary>
    public class EngineErrorLogger : IErrorLogger
    {
        public Exception LastException { get; set; }
        public string LastExtra { get; set; }

        public void Log(Exception exception, string extra)
        {
            LastException = exception;
            LastExtra = extra;
        }

    }
}