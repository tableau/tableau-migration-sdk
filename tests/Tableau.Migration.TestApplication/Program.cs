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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Tableau.Migration.Content;
using Tableau.Migration.TestApplication.Config;
using Tableau.Migration.TestApplication.Hooks;

namespace Tableau.Migration.TestApplication
{
    public static class Program
    {
        public static readonly DateTime StartTime = DateTime.Now;

        public static async Task Main(string[] args)
        {
            // Set the DOTNET_ENVIRONMENT environment variable to the name of the environment.
            // This loads the appsettings.<DOTNET_ENVIRONMENT>.json config file.
            // If no DOTNET_ENVIRONMENT is set, appsettings.json will be used
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((ctx, services) =>
                {
                    services
                        .Configure<TestApplicationOptions>(ctx.Configuration)
                        .Configure<TestTableauCloudUsernameOptions>(ctx.Configuration.GetSection("tableau:migrationOptions"))
                        .AddTableauMigrationSdk(ctx.Configuration.GetSection("tableau:migrationSdk"))
                        .AddHostedService<TestApplication>()
                        .ConfigureLogging(ctx)
                        .AddCustomizations();
                })
                .Build();

            await host.RunAsync();
        }

        public static IServiceCollection AddCustomizations(this IServiceCollection services)
        {
            services
                .AddScoped<TestTableauCloudUsernameMapping>()
                .AddScoped<TimeLoggerAfterActionHook>() // print and log the time when an action was completed

                // I would like to have a AfterBatchMigrationCompletedHook without the content type
                .AddScoped<SaveManifestAfterBatchMigrationCompletedHook<IUser>>()
                .AddScoped<SaveManifestAfterBatchMigrationCompletedHook<IGroup>>()
                .AddScoped<SaveManifestAfterBatchMigrationCompletedHook<IProject>>()
                .AddScoped<SaveManifestAfterBatchMigrationCompletedHook<IDataSource>>()
                .AddScoped<SaveManifestAfterBatchMigrationCompletedHook<IWorkbook>>()
                .AddScoped<UnlicensedUserFilter>()
                .AddScoped<UnlicensedUserMapping>()
                .AddScoped<SpecialUserFilter>()
                .AddScoped<SpecialUserMapping>()
                .AddScoped<NonDomainUserFilter>()
                .AddScoped(typeof(SkipByParentLocationFilter<>))
                .AddScoped(typeof(ContentWithinSkippedLocationMapping<>))
                .AddScoped<RemoveMissingDestinationUsersFromGroupsTransformer>();
            return services;
        }

        public static IServiceCollection ConfigureLogging(this IServiceCollection services, HostBuilderContext ctx)
        {
            services.AddLogging(
                config =>
                {
                    config.ClearProviders();
                    config.AddSerilog(
                        new LoggerConfiguration()
                        .Enrich.WithThreadId()
                        .WriteTo.File(
                            path: LogFileHelper.GetLogFilePath(ctx.Configuration.GetSection("log:folderPath").Value),
                            outputTemplate: LogFileHelper.LOG_LINE_TEMPLATE)

                        // Set the log level to Debug for select interfaces.
                        .MinimumLevel.Override("Tableau.Migration.Engine.Hooks.Filters.IContentFilter", LogEventLevel.Debug)
                        .MinimumLevel.Override("Tableau.Migration.Engine.Hooks.Mappings.IContentMapping", LogEventLevel.Debug)
                        .MinimumLevel.Override("Tableau.Migration.Engine.Hooks.Transformers.IContentTransformer", LogEventLevel.Debug)

                        .WriteTo.Logger(lc => lc
                             // Create a filter that writes certain loggers to the console
                             .Filter.ByIncludingOnly((logEvent) =>
                             {
                                 var sourceContext = logEvent.Properties.ContainsKey("SourceContext")
                                         ? logEvent.Properties["SourceContext"].ToString().Trim('\"')
                                                 : null;

                                 if (sourceContext is null)
                                     return false;

                                 // Only write these loggers to console (they will still get written file log)
                                 string[] sourceContextToPrint =
                                 [
                                     "Tableau.Migration.TestApplication.TestApplication",
                                     "Tableau.Migration.TestApplication.Hooks.TimeLoggerAfterActionHook"
                                 ];

                                 return sourceContextToPrint.Contains(sourceContext);
                             })
                             .WriteTo.Console())
                         .CreateLogger());
                });
            return services;
        }
    }
}