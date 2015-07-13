namespace MHGameWork.TheWizards.Engine.Diagnostics.Tracing
{
    /// <summary>
    /// Logs engine trace information
    /// </summary>
    public interface ITraceLogger
    {
        void Log(string log);
    }
}