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

using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Tableau.Migration.Engine.Hooks.PostPublish.Default;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Engine.Preparation;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Static class containing migration engine extension methods for <see cref="IServiceCollection"/> objects.
    /// </summary>
    internal static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Registers migration engine services.
        /// </summary>
        /// <param name="services">The service collection to register services with.</param>
        /// <returns>The same service collection for fluent API calls.</returns>
        internal static IServiceCollection AddMigrationEngine(this IServiceCollection services)
        {
            services.AddSingleton<IMigrationManifestFactory, MigrationManifestFactory>();

            //Bootstrap and scope state tracking services.
            services.AddScoped<MigrationInput>();
            services.AddScoped<IMigrationInput>(p => p.GetRequiredService<MigrationInput>());
            services.AddScoped<IMigrationInputInitializer>(p => p.GetRequiredService<MigrationInput>());

            services.AddScoped<IMigrationEndpointFactory, MigrationEndpointFactory>();

            services.AddScoped<IMigration, Migration>();
            services.AddScoped(p => p.GetRequiredService<IMigration>().Source);
            services.AddScoped(p => p.GetRequiredService<IMigration>().Destination);
            services.AddScoped(p => p.GetRequiredService<IMigration>().Plan);
            services.AddScoped(p => p.GetRequiredService<IMigration>().Manifest);
            services.AddScoped(p => p.GetRequiredService<IMigration>().Pipeline);
            services.AddScoped<IMigrationManifest>(p => p.GetRequiredService<IMigrationManifestEditor>());

            services.AddScoped(typeof(IMigrationPlanOptionsProvider<>), typeof(MigrationPlanOptionsProvider<>));

            //Hooks infrastructure.
            services.AddScoped<IMigrationHookRunner, MigrationHookRunner>();
            services.AddScoped<IContentMappingRunner, ContentMappingRunner>();
            services.AddScoped<IContentFilterRunner, ContentFilterRunner>();
            services.AddScoped<IContentTransformerRunner, ContentTransformerRunner>();

            //Plan building.
            services.AddSingleton<IMigrationPlanBuilderFactory, MigrationPlanBuilderFactory>();
            services.AddTransient<IMigrationPlanBuilder, MigrationPlanBuilder>();
            services.AddTransient<IMigrationPlanOptionsBuilder, MigrationPlanOptionsBuilder>();
            services.AddTransient<IMigrationHookBuilder, MigrationHookBuilder>();
            services.AddTransient<IContentMappingBuilder, ContentMappingBuilder>();
            services.AddTransient<IContentFilterBuilder, ContentFilterBuilder>();
            services.AddTransient<IContentTransformerBuilder, ContentTransformerBuilder>();

            //Migrators
            //Register concrete types so that the easy way to get interface types is through IMigrationPipeline.
            services.AddScoped(typeof(EndpointContentItemPreparer<,>));
            services.AddScoped(typeof(ExtractRefreshTaskServerToCloudPreparer));
            services.AddScoped(typeof(SourceContentItemPreparer<>));
            services.AddScoped(typeof(BulkPublishContentBatchMigrator<>));
            services.AddScoped(typeof(BulkPublishContentBatchMigrator<,>));
            services.AddScoped(typeof(ItemPublishContentBatchMigrator<>));
            services.AddScoped(typeof(ItemPublishContentBatchMigrator<,>));
            services.AddScoped(typeof(ItemPublishContentBatchMigrator<,,>));
            services.AddScoped(typeof(ContentMigrator<>));

            //Serializer
            services.AddSingleton<MigrationManifestSerializer>();

            //Caches/Content Finders
            //Register concrete types so that the easy way to get interface types is through IMigrationPipeline.
            services.AddScoped(typeof(BulkSourceCache<>));
            services.AddScoped(typeof(ISourceContentReferenceFinder<>), typeof(ManifestSourceContentReferenceFinder<>));
            services.AddScoped<ISourceContentReferenceFinderFactory, ManifestSourceContentReferenceFinderFactory>();

            services.AddScoped(typeof(BulkDestinationCache<>));
            services.AddScoped<BulkDestinationProjectCache>();
            services.AddScoped(typeof(IDestinationContentReferenceFinder<>), typeof(ManifestDestinationContentReferenceFinder<>));
            services.AddScoped<IDestinationContentReferenceFinderFactory, ManifestDestinationContentReferenceFinderFactory>();

            //Pipelines.
            services.AddScoped<ServerToCloudMigrationPipeline>();
            services.AddScoped<IMigrationPipelineFactory, MigrationPipelineFactory>();
            services.AddScoped<IMigrationPipelineRunner, MigrationPipelineRunner>();

            //Migration actions.
            services.AddScoped<PreflightAction>();
            services.AddScoped(typeof(MigrateContentAction<>));

            //Top-level interface services.
            services.AddSingleton<IMigrator, Migrator>();

            //Standard/default hooks.
            services.AddScoped(typeof(PreviouslyMigratedFilter<>));
            services.AddScoped<GroupAllUsersFilter>();
            services.AddScoped<UserSiteRoleSupportUserFilter>();
            services.AddScoped(typeof(SystemOwnershipFilter<>));

            services.AddScoped<AuthenticationTypeDomainMapping>();
            services.AddScoped<TableauCloudUsernameMapping>();

            services.AddScoped<UserAuthenticationTypeTransformer>();
            services.AddScoped<UserTableauCloudSiteRoleTransformer>();
            services.AddScoped<GroupUsersTransformer>();
            services.AddScoped(typeof(OwnershipTransformer<>));
            services.AddScoped<TableauServerConnectionUrlTransformer>();
            services.AddScoped(typeof(CloudScheduleCompatibilityTransformer<>));
            services.AddScoped<CloudIncrementalRefreshTransformer>();
            services.AddScoped<MappedReferenceExtractRefreshTaskTransformer>();
            services.AddScoped(typeof(EncryptExtractTransformer<>));

            services.AddScoped<IPermissionsTransformer, PermissionsTransformer>();
            services.AddScoped<IMappedUserTransformer, MappedUserTransformer>();
            services.AddScoped(typeof(WorkbookReferenceTransformer<>));
            services.AddScoped<CustomViewDefaultUserReferencesTransformer>();
            
            services.AddScoped(typeof(OwnerItemPostPublishHook<,>));
            services.AddScoped(typeof(PermissionsItemPostPublishHook<,>));
            services.AddScoped(typeof(TagItemPostPublishHook<,>));
            services.AddScoped<ProjectPostPublishHook>();
            services.AddScoped(typeof(ChildItemsPermissionsPostPublishHook<,>));
            services.AddScoped<CustomViewDefaultUsersPostPublishHook>();

            //Migration engine file store.
            services.AddScoped<MigrationDirectoryContentFileStore>();
            services.AddScoped(s =>
            {
                /* Since this IContentFileStore factory is registerd after AddMigrationApiClient,
                 * the factory might be running in the main migration DI scope
                 * (which should create a single scoped file store for the migration),
                 * or in one of the endpoint API client DI scopes
                 * (which should mimic AddMigrationApiClient factory's behavior of using the ApiClientInput overriden value).
                 * 
                 * We look at IApiClientInputInitializer.IsInitialized to determine what scope we are in.
                 */
                var apiClientInput = s.GetRequiredService<IApiClientInputInitializer>();
                if (apiClientInput.IsInitialized)
                {
                    return apiClientInput.FileStore;
                }

                return new EncryptedFileStore(s, s.GetRequiredService<MigrationDirectoryContentFileStore>());
            });

            return services;
        }
    }
}
