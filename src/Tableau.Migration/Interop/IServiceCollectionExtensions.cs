//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Config;
using Tableau.Migration.Interop.Logging;

namespace Tableau.Migration.Interop
{
    /// <summary>
    /// Static class containing extension methods for <see cref="IServiceCollection"/> objects.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add python support by adding python logging and configuration via environment variables.
        /// This will clear all existing <see cref="ILoggerProvider"/>s. All other logger provider should be added after this call.
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
                .AddLogging(b => b.AddPythonLogging(pythonProviderFactory))
                // Add environment variable configuration
                .AddEnvironmentVariableConfiguration();
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

        /// <summary>
        /// Adds support for setting configuration values via environment variables.
        /// Environment variables start with "MigrationSDK__".
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEnvironmentVariableConfiguration(this IServiceCollection services)
        {
            var configBuilder =
                new ConfigurationBuilder()
                    .AddEnvironmentVariables("MigrationSDK__");
            var config = configBuilder.Build();

            services.Configure<MigrationSdkOptions>(nameof(MigrationSdkOptions), config);

            return services;
        }
    }
}
