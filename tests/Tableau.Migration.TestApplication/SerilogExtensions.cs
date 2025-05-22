//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Linq;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Tableau.Migration.TestApplication.Config;

namespace Tableau.Migration.TestApplication
{
    internal static class SerilogExtensions
    {
        public static void ConfigureSerilog(ILoggingBuilder config, LogOptions logOptions)
        {
            config.ClearProviders();

            var serilogConfig = new LoggerConfiguration()
                .AddEnrichments()
                .AddMinimumLogLevelOverrides(logOptions)
                .ConfigureConsoleLogging(logOptions)
                .ConfigureFileLogging(logOptions);

            config.AddSerilog(serilogConfig.CreateLogger());
        }

        private static LoggerConfiguration AddEnrichments(this LoggerConfiguration serilogConfig)
            => serilogConfig.Enrich.WithThreadId().Enrich.With<ActivityEnricher>();

        private static LoggerConfiguration ConfigureFileLogging(this LoggerConfiguration serilogConfig, LogOptions logOptions)
        {
            var logPath = logOptions.FolderPath;

            if (string.IsNullOrEmpty(logPath))
            {
                return serilogConfig;
            }

            return serilogConfig.WriteTo.File(
                path: LogFileHelper.GetLogFilePath(logPath),
                outputTemplate: LogFileHelper.LOG_LINE_TEMPLATE,
                fileSizeLimitBytes: logOptions.FileSizeLimitMB * 1024 * 1024,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: null); // Retain all log files
        }

        private static LoggerConfiguration ConfigureConsoleLogging(this LoggerConfiguration serilogConfig, LogOptions logOptions)
        {
            return serilogConfig
                .WriteTo.Logger(lc => AddSourceContextFilter(lc, logOptions.ConsoleLoggingSources))
                .WriteTo.Logger(lc => AddFatalEventFilter(lc));

            static LoggerConfiguration AddSourceContextFilter(LoggerConfiguration lc, string[] consoleLoggingSources)
                => lc.Filter.ByIncludingOnly((logEvent) =>
                    logEvent.Properties.TryGetValue("SourceContext", out var srcContext)
                    && consoleLoggingSources.Contains(srcContext.ToString().Trim('\"')))
                    .WriteTo.Console();

            // Create a filter that writes fatal log events to the console
            static LoggerConfiguration AddFatalEventFilter(LoggerConfiguration lc)
                => lc.Filter.ByIncludingOnly(logEvent => logEvent.Level == LogEventLevel.Fatal).WriteTo.Console();
        }

        private static LoggerConfiguration AddMinimumLogLevelOverrides(this LoggerConfiguration config, LogOptions logOptions)
        {
            foreach (var item in logOptions.MinLogLevelOverrides.Where(i => !string.IsNullOrEmpty(i.Source)))
            {
                config.MinimumLevel.Override(item.Source, item.Level);
            }
            return config;
        }
    }
}
