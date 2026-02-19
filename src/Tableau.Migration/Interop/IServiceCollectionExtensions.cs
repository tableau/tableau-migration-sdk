//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Interop.Logging;

namespace Tableau.Migration.Interop
{
    /// <summary>
    /// Static class containing extension methods for <see cref="IServiceCollection"/> objects.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        private const string LOGGING_SECTION = "Logging";

        private static ImmutableDictionary<string, string> DEFAULT_PYTHON_LOG_LEVELS = new Dictionary<string, string>()
        {
            ["System"] = "Warning",
            ["Polly"] = "Warning"
        }.ToImmutableDictionary();

        /// <summary>
        /// Add python support by adding python logging and configuration via environment variables.
        /// This will clear all existing <see cref="ILoggerProvider"/>s. All other logger provider should be added after this call.
        /// </summary>
        /// <param name="services">The service collection to register services with.</param>
        /// <param name="loggerFactory">A factory to use to create new loggers for a given category name.</param>
        /// <returns>The original service collection, for fluent API usage.</returns>
        public static IServiceCollection AddPythonSupport(this IServiceCollection services, Func<string, NonGenericLoggerBase> loggerFactory)
        {
            // Add environment variable configuration.
            var userOptions = BuildEnvironmentVariableConfiguration();

            return services
                // Add underlying configuration to DI
                .AddSingleton(userOptions)
                .AddTableauMigrationSdk(userOptions)
                // Add additional Python logging.
                .AddLogging(b => b.AddPythonLogging(userOptions.GetSection(LOGGING_SECTION), loggerFactory));
        }

        /// <summary>
        /// Adds a python support, including supporting the python logger
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/></param>
        /// <param name="configSection">The configuration section to use for logging.</param>
        /// <param name="loggerFactory">A factory to use to create new loggers for a given category name.</param>
        /// <returns>The original logging builder, for fluent API usage.</returns>
        public static ILoggingBuilder AddPythonLogging(this ILoggingBuilder builder, IConfigurationSection configSection, Func<string, NonGenericLoggerBase> loggerFactory)
        {
            // Add the Python log provider.
            builder.ClearProviders();
            builder.AddProvider(new NonGenericLoggerProvider(loggerFactory));

            // Allow Python users to configure .NET logging through configuration, i.e. environment variables.
            builder.AddConfiguration(configSection);

            return builder;
        }

        /// <summary>
        /// Adds support for setting configuration values via environment variables.
        /// Environment variables start with "MigrationSDK__".
        /// </summary>
        /// <retruns>The build configuration.</retruns>
        private static IConfigurationRoot BuildEnvironmentVariableConfiguration()
        {
            // Set standard python configuration values.
            Environment.SetEnvironmentVariable(Constants.PYTHON_USER_AGENT_COMMENT_CONFIG_KEY, Constants.PYTHON_USER_AGENT_COMMENT);

            /*
             * Set a default logging configuration since a Python user is unlikely to be familiar with appsettings.json usage,
             * but allow the user to override.
             */
            foreach((var logName, var defaultLogLevel) in DEFAULT_PYTHON_LOG_LEVELS)
            {
                var logEnvironmentVariable = $"{Constants.PYTHON_ENVIRONMENT_VARIABLE_PREFIX}{LOGGING_SECTION}__LogLevel__{logName}";
                if(Environment.GetEnvironmentVariable(logEnvironmentVariable) is null)
                {
                    Environment.SetEnvironmentVariable(logEnvironmentVariable, defaultLogLevel);
                }
            }

            var configBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables(Constants.PYTHON_ENVIRONMENT_VARIABLE_PREFIX);

            var config = configBuilder.Build();
            return config;
        }
    }
}
