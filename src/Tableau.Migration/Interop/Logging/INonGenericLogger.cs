using System;
using Microsoft.Extensions.Logging;

namespace Tableau.Migration.Interop.Logging
{
    /// <summary>
    /// Interface for a logger that does not use a generic state type.
    /// </summary>
    public interface INonGenericLogger
    {
        /// <summary>
        /// Writes a log entry with a pre-formatted state object.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The pre-formatted entry to be written.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="message">The pre-formatted message to write.</param>
        void Log(LogLevel logLevel, EventId eventId, string state, Exception? exception, string message);
    }
}
