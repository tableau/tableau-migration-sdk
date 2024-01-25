using System;
using Microsoft.Extensions.Logging;

namespace Tableau.Migration.Interop.Logging
{
    /// <summary>
    /// Concrete abstract implementation of <see cref="INonGenericLogger"/>
    /// This is needed as Python can not inhert from interfaces and needs concrete objects
    /// </summary>
    public abstract class NonGenericLoggerBase : ILogger, INonGenericLogger
    {
        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <param name="state">The identifier for the scope.</param>
        /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
        /// <returns>An <see cref="IDisposable"/> that ends the logical operation scope on dispose.</returns>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a <see cref="string"/> message of the <paramref name="state"/> and <paramref name="exception"/>.</param>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            // In python.net, a generic method like Log<T> can't be concretely implemented because python has no understanding of generic methods.
            // To get around this, we pre-format the message, and pass it to the non-generic `Log` method
            this.Log(logLevel, eventId, formatter(state, null), exception, formatter(state, exception));
        }

        /// <inheritdoc />
        public abstract void Log(LogLevel logLevel, EventId eventId, string state, Exception? exception, string message);

        /// <summary>
        /// Checks if the given <paramref name="logLevel"/> is enabled.
        /// </summary>
        /// <param name="logLevel">Level to be checked.</param>
        /// <returns><c>true</c> if enabled.</returns>
        public abstract bool IsEnabled(LogLevel logLevel);
    }
}
