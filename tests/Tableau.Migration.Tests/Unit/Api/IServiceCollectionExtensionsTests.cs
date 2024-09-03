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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Search;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class IServiceCollectionExtensionsTests
    {
        public class AddMigrationApiClient : IServiceCollectionExtensionsTestBase
        {
            protected override void ConfigureServices(IServiceCollection services)
            {
                services
                    .AddLocalization()
                    .AddSharedResourcesLocalization()
                    .AddMigrationApiClient();
            }

            protected AsyncServiceScope InitializeApiScope(TableauSiteConnectionConfiguration? config = null,
                IContentReferenceFinderFactory? finderFactoryOverride = null)
            {
                config ??= Create<TableauSiteConnectionConfiguration>();
                var scope = ServiceProvider.CreateAsyncScope();

                var input = scope.ServiceProvider.GetRequiredService<IApiClientInputInitializer>();
                input.Initialize(config.Value, finderFactoryOverride);

                return scope;
            }

            [Fact]
            public async Task Registers_expected_services()
            {
                await AssertServiceAsync<IFileSystem, FileSystem>(ServiceLifetime.Singleton);
                await AssertServiceAsync<ITaskDelayer, TaskDelayer>(ServiceLifetime.Singleton);
                await AssertServiceAsync<IPermissionsApiClientFactory, PermissionsApiClientFactory>(ServiceLifetime.Scoped);
                await AssertServiceAsync<ITagsApiClientFactory, TagsApiClientFactory>(ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedApiClientInputAndInitializer()
            {
                await using var scope1 = ServiceProvider.CreateAsyncScope();
                await using var scope2 = ServiceProvider.CreateAsyncScope();

                var concreteInput1 = scope1.ServiceProvider.GetRequiredService<ApiClientInput>();
                var concreteInputRepeat = scope1.ServiceProvider.GetRequiredService<ApiClientInput>();
                var concreteInput2 = scope2.ServiceProvider.GetRequiredService<ApiClientInput>();

                Assert.Same(concreteInput1, concreteInputRepeat);
                Assert.NotSame(concreteInput1, concreteInput2);

                var interfaceInput1 = scope1.ServiceProvider.GetRequiredService<IApiClientInput>();
                var interfaceInputRepeat = scope1.ServiceProvider.GetRequiredService<IApiClientInput>();
                var interfaceInput2 = scope2.ServiceProvider.GetRequiredService<IApiClientInput>();

                Assert.Same(interfaceInput1, interfaceInputRepeat);
                Assert.NotSame(interfaceInput2, interfaceInput1);

                Assert.Same(concreteInput1, interfaceInput1);
                Assert.Same(concreteInput2, interfaceInput2);

                var initializer1 = scope1.ServiceProvider.GetRequiredService<IApiClientInputInitializer>();
                var initializerRepeat = scope1.ServiceProvider.GetRequiredService<IApiClientInputInitializer>();
                var initializer2 = scope2.ServiceProvider.GetRequiredService<IApiClientInputInitializer>();

                Assert.Same(initializer1, initializerRepeat);
                Assert.NotSame(initializer2, initializer1);

                Assert.Same(concreteInput1, initializer1);
                Assert.Same(concreteInput2, initializer2);
            }

            [Fact]
            public async Task RegistersScopedApiClients()
            {
                await using var scope = InitializeApiScope();

                AssertService<IApiClient, ApiClient>(scope, ServiceLifetime.Scoped);
                AssertService<IDataSourcesApiClient, DataSourcesApiClient>(scope, ServiceLifetime.Scoped);
                AssertService<IFlowsApiClient, FlowsApiClient>(scope, ServiceLifetime.Scoped);
                AssertService<IGroupsApiClient, GroupsApiClient>(scope, ServiceLifetime.Scoped);
                AssertService<IJobsApiClient, JobsApiClient>(scope, ServiceLifetime.Scoped);
                AssertService<ISchedulesApiClient, SchedulesApiClient>(scope, ServiceLifetime.Scoped);
                AssertService<IProjectsApiClient, ProjectsApiClient>(scope, ServiceLifetime.Scoped);
                AssertService<ISitesApiClient, SitesApiClient>(scope, ServiceLifetime.Scoped);
                AssertService<IUsersApiClient, UsersApiClient>(scope, ServiceLifetime.Scoped);
                AssertService<IWorkbooksApiClient, WorkbooksApiClient>(scope, ServiceLifetime.Scoped);
                AssertService<IViewsApiClient, ViewsApiClient>(scope, ServiceLifetime.Scoped);
                AssertService<ICustomViewsApiClient, CustomViewsApiClient>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedApiFinderFactoryAsync()
            {
                await AssertServiceAsync<ApiContentReferenceFinderFactory>(ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedBulkContentCacheAsync()
            {
                await using var scope = InitializeApiScope();

                AssertService<BulkApiContentReferenceCache<IProject>>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task FinderFactoryRedirectFromInputAsync()
            {
                await using var scope = InitializeApiScope();

                var apiInput = scope.ServiceProvider.GetRequiredService<IApiClientInput>();
                var finderFactory = scope.ServiceProvider.GetRequiredService<IContentReferenceFinderFactory>();

                Assert.Same(apiInput.ContentReferenceFinderFactory, finderFactory);
            }

            [Fact]
            public void Adds_http_services_if_not_previously_added()
            {
                using var serviceProvider = new ServiceCollection()
                    .AddLocalization()
                    .AddSharedResourcesLocalization()
                    .AddMigrationApiClient()
                    .BuildServiceProvider();

                Assert.NotNull(serviceProvider.GetService<IHttpClient>());
            }

            [Fact]
            public async Task Uses_existing_DefaultPermissionsContentTypeOptions()
            {
                var existingOptions = new DefaultPermissionsContentTypeOptions();

                Services.AddScoped(_ => existingOptions);

                await using var scope = ServiceProvider.CreateAsyncScope();

                var options = scope.ServiceProvider.GetService<DefaultPermissionsContentTypeOptions>();

                Assert.NotNull(options);
                Assert.IsType<DefaultPermissionsContentTypeOptions>(options);
                Assert.Same(existingOptions, options);
            }

            [Fact]
            public async Task RegistersSingletonPathResolver()
            {
                await AssertServiceAsync<IContentFilePathResolver, ContentTypeFilePathResolver>(ServiceLifetime.Singleton);
            }

            [Fact]
            public async Task RegistersSingletonEncryptionFactory()
            {
                await AssertServiceAsync<ISymmetricEncryptionFactory, Aes256EncryptionFactory>(ServiceLifetime.Singleton);
            }

            [Fact]
            public async Task RegistersScopedTempFileStoreAsync()
            {
                await using var scope = InitializeApiScope();

                AssertService<TemporaryDirectoryContentFileStore>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task FileStoreRedirectFromInputAsync()
            {
                await using var scope = InitializeApiScope();

                var apiInput = scope.ServiceProvider.GetRequiredService<IApiClientInput>();
                var contentFileStore = scope.ServiceProvider.GetRequiredService<IContentFileStore>();

                Assert.Same(apiInput.FileStore, contentFileStore);
            }

            [Fact]
            public async Task Registers_content_caches()
            {
                await using var scope = InitializeApiScope();

                AssertService<IContentCacheFactory, ContentCacheFactory>(scope, ServiceLifetime.Scoped);
                AssertService<IContentCache<IServerSchedule>, ApiContentCache<IServerSchedule>>(scope, ServiceLifetime.Scoped);
                AssertService<BulkApiContentReferenceCache<IServerSchedule>, ApiContentCache<IServerSchedule>>(scope, ServiceLifetime.Scoped);

                var serverScheduleCache = scope.ServiceProvider.GetRequiredService<BulkApiContentReferenceCache<IServerSchedule>>();

                var caches = new object[]
                {
                    scope.ServiceProvider.GetRequiredService<ApiContentCache<IServerSchedule>>(),
                    scope.ServiceProvider.GetRequiredService<IContentCache<IServerSchedule>>(),
                    scope.ServiceProvider.GetRequiredService<BulkApiContentReferenceCache<IServerSchedule>>()
                };

                caches.AssertAllSame();
            }
        }
    }
}
