// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Engine.Preparation;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine
{
    public class IServiceCollectionExtensionsTests
    {
        public class AddMigrationEngine : IServiceCollectionExtensionsTestBase
        {
            public AddMigrationEngine()
            {
                AutoFixture.Register<IMigrationPlanEndpointConfiguration>(() => TableauApiEndpointConfiguration.Empty);
            }

            protected override void ConfigureServices(IServiceCollection services)
            {
                services.AddLogging()
                    .AddLocalization()
                    .AddSharedResourcesLocalization()
                    .AddMigrationApiClient()
                    .AddMigrationEngine();
            }

            protected AsyncServiceScope InitializeMigrationScope()
                => InitializeMigrationScope(Create<IMigrationPlan>(), Create<IMigrationManifest>());

            protected AsyncServiceScope InitializeMigrationScope(IMigrationPlan plan, IMigrationManifest? previousManifest)
            {
                var scope = ServiceProvider.CreateAsyncScope();

                var input = scope.ServiceProvider.GetRequiredService<IMigrationInputInitializer>();
                input.Initialize(plan, previousManifest);

                return scope;
            }

            [Fact]
            public async Task RegistersScopedMigrationInputAndInitializer()
            {
                await using var scope1 = ServiceProvider.CreateAsyncScope();
                await using var scope2 = ServiceProvider.CreateAsyncScope();

                var concreteInput1 = scope1.ServiceProvider.GetRequiredService<MigrationInput>();
                var concreteInputRepeat = scope1.ServiceProvider.GetRequiredService<MigrationInput>();
                var concreteInput2 = scope2.ServiceProvider.GetRequiredService<MigrationInput>();

                Assert.Same(concreteInput1, concreteInputRepeat);
                Assert.NotSame(concreteInput1, concreteInput2);

                var interfaceInput1 = scope1.ServiceProvider.GetRequiredService<IMigrationInput>();
                var interfaceInputRepeat = scope1.ServiceProvider.GetRequiredService<IMigrationInput>();
                var interfaceInput2 = scope2.ServiceProvider.GetRequiredService<IMigrationInput>();

                Assert.Same(interfaceInput1, interfaceInputRepeat);
                Assert.NotSame(interfaceInput2, interfaceInput1);

                Assert.Same(concreteInput1, interfaceInput1);
                Assert.Same(concreteInput2, interfaceInput2);

                var initializer1 = scope1.ServiceProvider.GetRequiredService<IMigrationInputInitializer>();
                var initializerRepeat = scope1.ServiceProvider.GetRequiredService<IMigrationInputInitializer>();
                var initializer2 = scope2.ServiceProvider.GetRequiredService<IMigrationInputInitializer>();

                Assert.Same(initializer1, initializerRepeat);
                Assert.NotSame(initializer2, initializer1);

                Assert.Same(concreteInput1, initializer1);
                Assert.Same(concreteInput2, initializer2);
            }

            [Fact]
            public async Task RegistersScopedEndpointFactory()
            {
                await AssertServiceAsync<IMigrationEndpointFactory, MigrationEndpointFactory>(ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersSingletoneManifestFactory()
            {
                await AssertServiceAsync<IMigrationManifestFactory, MigrationManifestFactory>(ServiceLifetime.Singleton);
            }

            [Fact]
            public async Task RegistersScopedMigrationAndProperties()
            {
                await using var scope1 = InitializeMigrationScope();
                await using var scope2 = InitializeMigrationScope();

                var scope1Obj = scope1.ServiceProvider.GetRequiredService<IMigration>();
                var scope1Repeat = scope1.ServiceProvider.GetRequiredService<IMigration>();
                var scope2Obj = scope2.ServiceProvider.GetRequiredService<IMigration>();

                Assert.Same(scope1Obj, scope1Repeat);
                Assert.NotSame(scope2Obj, scope1Obj);

                Assert.IsType<Migration.Engine.Migration>(scope1Obj);
                Assert.IsType<Migration.Engine.Migration>(scope2Obj);

                Assert.Same(scope1Obj.Source, scope1.ServiceProvider.GetRequiredService<ISourceEndpoint>());
                Assert.Same(scope1Obj.Destination, scope1.ServiceProvider.GetRequiredService<IDestinationEndpoint>());
                Assert.Same(scope1Obj.Plan, scope1.ServiceProvider.GetRequiredService<IMigrationPlan>());
                Assert.Same(scope1Obj.Manifest, scope1.ServiceProvider.GetRequiredService<IMigrationManifest>());
                Assert.Same(scope1Obj.Manifest, scope1.ServiceProvider.GetRequiredService<IMigrationManifestEditor>());
                Assert.Same(scope1Obj.Pipeline, scope1.ServiceProvider.GetRequiredService<IMigrationPipeline>());
            }

            [Fact]
            public async Task RegistersScopedPlanOptionsProvider()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<IMigrationPlanOptionsProvider<TestPlanOptions>, MigrationPlanOptionsProvider<TestPlanOptions>>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedHookRunner()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<IMigrationHookRunner, MigrationHookRunner>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersTransientPlanBuilder()
            {
                await AssertServiceAsync<IMigrationPlanBuilder, MigrationPlanBuilder>(ServiceLifetime.Transient);
            }

            [Fact]
            public async Task RegistersTransientOptionsBuilder()
            {
                await AssertServiceAsync<IMigrationPlanOptionsBuilder, MigrationPlanOptionsBuilder>(ServiceLifetime.Transient);
            }

            [Fact]
            public async Task RegistersTransientHookBuilder()
            {
                await AssertServiceAsync<IMigrationHookBuilder, MigrationHookBuilder>(ServiceLifetime.Transient);
            }

            [Fact]
            public async Task RegistersScopedServerToCloudPipeline()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<ServerToCloudMigrationPipeline>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedPipelineFactory()
            {
                await AssertServiceAsync<IMigrationPipelineFactory, MigrationPipelineFactory>(ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersSingletonMigrator()
            {
                await AssertServiceAsync<IMigrator, Migrator>(ServiceLifetime.Singleton);
            }

            [Fact]
            public async Task RegistersScopedContentMigratorAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<ContentMigrator<TestContentType>>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedSourcePreparerAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<SourceContentItemPreparer<TestContentType>>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedEndpointPreparerAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<EndpointContentItemPreparer<TestContentType, TestPublishType>>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedItemBatchMigratorAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<ItemPublishContentBatchMigrator<TestContentType>>(scope, ServiceLifetime.Scoped);
                AssertService<ItemPublishContentBatchMigrator<TestContentType, TestPublishType>>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedBulkBatchMigratorAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<BulkPublishContentBatchMigrator<TestContentType>>(scope, ServiceLifetime.Scoped);
                AssertService<BulkPublishContentBatchMigrator<TestContentType, TestPublishType>>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedBulkDestinationCacheAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<BulkDestinationCache<TestContentType>>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedDestinationContentFinderAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<ManifestDestinationContentReferenceFinder<TestContentType>>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedDestinationContentFinderFactoryAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<ManifestDestinationContentReferenceFinderFactory>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedSourceContentFinderAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<ManifestSourceContentReferenceFinder<TestContentType>>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedSourceContentFinderFactoryAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<ManifestSourceContentReferenceFinderFactory>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedMigrationFileStoreAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<MigrationDirectoryContentFileStore>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedFileStoreAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<IContentFileStore, EncryptedFileStore>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedTableauServerConnectionUrlTransformerAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<TableauServerConnectionUrlTransformer>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedProjectCacheAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<BulkDestinationProjectCache>(scope, ServiceLifetime.Scoped);
            }

            [Fact]
            public async Task RegistersScopedPreviouslyMigratedFilterAsync()
            {
                await using var scope = InitializeMigrationScope();

                AssertService<PreviouslyMigratedFilter<TestContentType>>(scope, ServiceLifetime.Scoped);
            }
        }
    }
}
