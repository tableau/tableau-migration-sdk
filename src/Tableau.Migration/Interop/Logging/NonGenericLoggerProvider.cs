using System;
using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;

namespace Tableau.Migration.Interop.Logging
{
    /// <summary>
    /// <see cref="ILoggerProvider"/> implementation that produces <see cref="NonGenericLoggerBase"/> loggers.
    /// </summary>
    [UnsupportedOSPlatform("browser")]
    [ProviderAlias("NonGeneric")]
    public class NonGenericLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, NonGenericLoggerBase> _loggers =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly Func<string, NonGenericLoggerBase> _newLogger;

        /// <summary>
        /// Creates a new <see cref="NonGenericLoggerProvider"/> object.
        /// </summary>
        /// <param name="loggerFactory">A factory to use to create new loggers for a given category name.</param>
        public NonGenericLoggerProvider(Func<string, NonGenericLoggerBase> loggerFactory)
        {
            _newLogger = loggerFactory;
        }

        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The instance of <see cref="ILogger"/> that was created.</returns>
        /// <exception cref="NullReferenceException">If the configured logger factory returns a null object.</exception>
        public ILogger CreateLogger(string categoryName)
        {
            var concreteLogger = _newLogger(categoryName);
            if (concreteLogger is not null)
            {
                return _loggers.GetOrAdd(categoryName, concreteLogger);
            }

            throw new NullReferenceException("Unable to create NonGenericLogger");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _loggers.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
