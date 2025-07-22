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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tableau.Migration.TestApplication.Config;
using Tableau.Migration.TestApplication.Hooks.ActionCompleted;
using Tableau.Migration.TestApplication.Hooks.BatchMigrationCompleted;
using Tableau.Migration.TestApplication.Hooks.Filters;
using Tableau.Migration.TestApplication.Hooks.Mappings;
using Tableau.Migration.TestApplication.Hooks.Transformers;

namespace Tableau.Migration.TestApplication
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, HostBuilderContext ctx)
        {
            return services
                .Configure<TestApplicationOptions>(ctx.Configuration)
                .Configure<TestTableauCloudUsernameOptions>(ctx.Configuration.GetSection("tableau:migrationOptions"));
        }

        public static IServiceCollection AddCustomizations(this IServiceCollection services)
        {
            services
                .AddScoped(typeof(SkipFilter<>))
                .AddScoped<TestTableauCloudUsernameMapping>()
                .AddScoped<TimeLoggerAfterActionHook>() // print and log the time when an action was completed
                .AddScoped(typeof(LogMigrationBatchSummaryHook<>))
                .AddScoped(typeof(SaveManifestAfterBatchMigrationCompletedHook<>))
                .AddScoped<UnlicensedUserFilter>()
                .AddScoped<UnlicensedUserMapping>()
                .AddScoped<SpecialUserFilter>()
                .AddScoped<SpecialUserMapping>()
                .AddScoped<NonDomainUserFilter>()
                .AddScoped(typeof(SkipByParentLocationFilter<>))
                .AddScoped(typeof(ContentWithinSkippedLocationMapping<>))
                .AddScoped<RemoveMissingDestinationUsersFromGroupsTransformer>()
                .AddScoped(typeof(ViewerOwnerTransformer<>))
                .AddScoped(typeof(SkipIdsFilter<>));

            return services;
        }

        public static IServiceCollection ConfigureLogging(this IServiceCollection services, HostBuilderContext ctx)
        {
            LogOptions logOptions = ctx.Configuration.GetSection("log").Get<LogOptions>() ?? new();

            return services.AddLogging(config => SerilogExtensions.ConfigureSerilog(config, logOptions));
        }
    }
}
