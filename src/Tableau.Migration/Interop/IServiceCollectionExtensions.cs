using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Interop.Logging;

namespace Tableau.Migration.Interop
{
    /// <summary>
    /// Static class containing extension methods for <see cref="IServiceCollection"/> objects.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add python support. This will clear all existing <see cref="ILoggerProvider"/>s. All other logger provider should be added after this call.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="pythonProviderFactory"></param>
        /// <returns></returns>
        public static IServiceCollection AddPythonSupport(this IServiceCollection services, Func<IServiceProvider, NonGenericLoggerProvider> pythonProviderFactory)
        {
            services
                // Replace the default IUserAgentSuffixProvider with the python one
                .Replace(new ServiceDescriptor(typeof(IUserAgentSuffixProvider), typeof(PythonUserAgentSuffixProvider), ServiceLifetime.Singleton))
                // Add Python Logging
                .AddLogging(b => b.AddPythonLogging(pythonProviderFactory));
            return services;
        }

        /// <summary>
        /// Adds a python support, including supporting the python logger
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/></param>
        /// <param name="pythonProviderFactory">Function that creates a new <see cref="NonGenericLoggerProvider"/></param>
        /// <returns></returns>
        public static ILoggingBuilder AddPythonLogging(this ILoggingBuilder builder, Func<IServiceProvider, NonGenericLoggerProvider> pythonProviderFactory)
        {
            // Clear all previous providers
            builder.ClearProviders();
            builder.Services.TryAddSingleton<ILoggerProvider>(pythonProviderFactory);
            // Enable all logs from .NET
            // They will be filtered on the migration_logger.py class
            builder.AddFilter<NonGenericLoggerProvider>(null, LogLevel.Trace);

            return builder;
        }
    }
}
