using System;

namespace MHGameWork.TheWizards.Debugging
{
    /// <summary>
    /// Responsible logging exceptions that were caught in order to prevent the app from failing, 
    /// but are real exceptions.
    /// For example: sockets give timeout exceptions, these are expected behaviour and are thus not real exceptions. These exceptions should not be logged to this logger
    /// When using plugin architecture, all calls to the plugin are probably surrounded by try.. catch. Exceptions that are caught here ARE real exceptions, so it should
    /// be possible to see these in some way. This service takes care of that.
    /// </summary>
    public interface IErrorLogger
    {
        void Log(Exception exception, string extra);
    }

}