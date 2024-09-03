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

using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api.Labels;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Search;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Simulation;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Static class containing API client extension methods for <see cref="IServiceCollection"/> objects.
    /// </summary>
    internal static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Registers migration API client services.
        /// </summary>
        /// <param name="services">The service collection to register services with.</param>
        /// <returns>The same service collection for fluent API calls.</returns>
        internal static IServiceCollection AddMigrationApiClient(this IServiceCollection services)
        {
            //Check for HTTP dependencies and add them if they haven't already been added.
            if (!services.Any(s => s.ServiceType == typeof(IHttpClient)))
                services.AddHttpServices();

            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<ITaskDelayer, TaskDelayer>();
            services.AddSingleton<IMemoryStreamManager, MemoryStreamManager>();

            //Bootstrap and scope state tracking services.
            services.AddScoped<ApiClientInput>();
            services.AddScoped<IApiClientInput>(p => p.GetRequiredService<ApiClientInput>());
            services.AddScoped<IApiClientInputInitializer>(p => p.GetRequiredService<ApiClientInput>());
            services.AddScoped<IScopedApiClientFactory, ScopedApiClientFactory>();
            services.AddScoped<IPermissionsApiClientFactory, PermissionsApiClientFactory>();
            services.AddScoped<ITagsApiClientFactory, TagsApiClientFactory>();
            services.AddScoped<IViewsApiClientFactory, ViewsApiClientFactory>();
            services.AddScoped<ILabelsApiClientFactory, LabelsApiClientFactory>();

            //Main API client.
            services.AddScoped<IApiClient, ApiClient>();
            services.AddScoped<IDataSourcesApiClient, DataSourcesApiClient>();
            services.AddScoped<IFlowsApiClient, FlowsApiClient>();
            services.AddScoped<IGroupsApiClient, GroupsApiClient>();
            services.AddScoped<IJobsApiClient, JobsApiClient>();
            services.AddScoped<ISchedulesApiClient, SchedulesApiClient>();
            services.AddScoped<IProjectsApiClient, ProjectsApiClient>();
            services.AddScoped<ISitesApiClient, SitesApiClient>();
            services.AddScoped<IUsersApiClient, UsersApiClient>();
            services.AddScoped<IViewsApiClient, ViewsApiClient>();
            services.AddScoped<ICustomViewsApiClient, CustomViewsApiClient>();
            services.AddScoped<IWorkbooksApiClient, WorkbooksApiClient>();
            services.AddScoped<ITasksApiClient, TasksApiClient>();
            services.AddScoped<ICustomViewsApiClient, CustomViewsApiClient>();

            //API Simulator.
            services.AddSingleton<ITableauApiSimulatorFactory, TableauApiSimulatorFactory>();
            services.AddSingleton<ITableauApiSimulatorCollection, TableauApiSimulatorCollection>();
            services.AddSingleton<IResponseSimulatorProvider, TableauApiResponseSimulatorProvider>();

            //Publishing services.
            services.AddScoped<IDataSourcePublisher, DataSourcePublisher>();
            services.AddScoped<IFlowPublisher, FlowPublisher>();
            services.AddScoped<IWorkbookPublisher, WorkbookPublisher>();
            services.AddScoped<ICustomViewPublisher, CustomViewPublisher>();
            services.AddScoped<IConnectionManager, ConnectionManager>();
            services.AddScoped(typeof(ILabelsApiClient<>), typeof(LabelsApiClient<>));

            //Non-Engine content search services.
            services.AddScoped<ApiContentReferenceFinderFactory>();
            services.AddScoped(p => p.GetRequiredService<ApiClientInput>().ContentReferenceFinderFactory);
            services.AddScoped(typeof(BulkApiContentReferenceCache<>));

            //Content caches
            services.AddScoped<IContentCacheFactory, ContentCacheFactory>();

            //Server schedules content cache
            services.AddScoped<ApiContentCache<IServerSchedule>>();
            services.AddScoped<IContentCache<IServerSchedule>>(p => p.GetRequiredService<ApiContentCache<IServerSchedule>>());
            services.AddScoped<BulkApiContentReferenceCache<IServerSchedule>>(p => p.GetRequiredService<ApiContentCache<IServerSchedule>>());

            //Non-Engine content file services.
            services.AddSingleton<IContentFilePathResolver, ContentTypeFilePathResolver>();
            services.AddSingleton<ISymmetricEncryptionFactory, Aes256EncryptionFactory>();
            services.AddScoped<TemporaryDirectoryContentFileStore>();
            services.AddScoped(p => new EncryptedFileStore(p, p.GetRequiredService<TemporaryDirectoryContentFileStore>()));
            services.AddScoped(p => p.GetRequiredService<ApiClientInput>().FileStore);

            return services;
        }
    }
}
