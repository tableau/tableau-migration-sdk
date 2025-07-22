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

using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.ContentConverters.Schedules;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Conversion;
using Tableau.Migration.Engine.Conversion.ExtractRefreshTasks;
using Tableau.Migration.Engine.Conversion.Schedules;
using Tableau.Migration.Engine.Conversion.Subscriptions;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Caching;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Hooks.InitializeMigration.Capabilities;
using Tableau.Migration.Engine.Hooks.InitializeMigration.Default;
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
        /// Registers migration engine.
        /// </summary>
        /// <param name="services">The service collection to register services with.</param>
        /// <returns>The same service collection for fluent API calls.</returns>
        internal static IServiceCollection AddMigrationEngine(this IServiceCollection services) => services
            .AddSingleton<IMigrationManifestFactory, MigrationManifestFactory>()
            .AddBoostrapServices()
            .AddStateTrackingServices()
            .AddScoped(typeof(IMigrationPlanOptionsProvider<>), typeof(MigrationPlanOptionsProvider<>))
            .AddHooksInfraServices()
            .AddPlanBuildingServices()
            .AddMigratorServices()
            .AddConversionServices()
            .AddContentClientServices()
            .AddSingleton<MigrationManifestSerializer>() // Serializer
            .AddCacheServices()
            .AddContentFinderServices()
            .AddPipelineServices()
            .AddMigrationActionServices()
            .AddSingleton<IMigrator, Migrator>() //Top-level interface .
            .AddMigrationCapabilityServices()
            .AddDefaultPreflightHookServices()
            .AddDefaultFilterServices()
            .AddDefaultMappingServices()
            .AddDefaultTransformerServices()
            .AddDefaultActionCompletedHookServices()
            .AddDefaultPostPublishHookServices()
            .AddFileStoreServices(); //Migration engine file store.

        private static IServiceCollection AddBoostrapServices(this IServiceCollection services) => services
            .AddScoped<MigrationInput>()
            .AddScoped<IMigrationInput>(p => p.GetRequiredService<MigrationInput>())
            .AddScoped<IMigrationInputInitializer>(p => p.GetRequiredService<MigrationInput>())
            .AddScoped<IMigrationEndpointFactory, MigrationEndpointFactory>();

        private static IServiceCollection AddStateTrackingServices(this IServiceCollection services) => services
            .AddScoped<IMigration, Migration>()
            .AddScoped(p => p.GetRequiredService<IMigration>().Source)
            .AddScoped(p => p.GetRequiredService<IMigration>().Destination)
            .AddScoped(p => p.GetRequiredService<IMigration>().Plan)
            .AddScoped(p => p.GetRequiredService<IMigration>().Manifest)
            .AddScoped(p => p.GetRequiredService<IMigration>().Pipeline)
            .AddScoped<IMigrationManifest>(p => p.GetRequiredService<IMigrationManifestEditor>());

        private static IServiceCollection AddHooksInfraServices(this IServiceCollection services) => services
            .AddScoped<IMigrationHookRunner, MigrationHookRunner>()
            .AddScoped<IContentMappingRunner, ContentMappingRunner>()
            .AddScoped<IContentFilterRunner, ContentFilterRunner>()
            .AddScoped<IContentTransformerRunner, ContentTransformerRunner>();

        private static IServiceCollection AddPlanBuildingServices(this IServiceCollection services) => services
            .AddSingleton<IMigrationPlanBuilderFactory, MigrationPlanBuilderFactory>()
            .AddTransient<IMigrationPlanBuilder, MigrationPlanBuilder>()
            .AddTransient<IMigrationPlanOptionsBuilder, MigrationPlanOptionsBuilder>()
            .AddTransient<IMigrationHookBuilder, MigrationHookBuilder>()
            .AddTransient<IContentMappingBuilder, ContentMappingBuilder>()
            .AddTransient<IContentFilterBuilder, ContentFilterBuilder>()
            .AddTransient<IContentTransformerBuilder, ContentTransformerBuilder>();

        private static IServiceCollection AddMigratorServices(this IServiceCollection services) => services
            //Register concrete types so that the easy way to get interface types is through IMigrationPipeline.
            .AddScoped(typeof(EndpointContentItemPreparer<,,>))
            .AddScoped(typeof(ExtractRefreshTaskServerToCloudPreparer))
            .AddScoped(typeof(SourceContentItemPreparer<>))
            .AddScoped(typeof(SourceContentItemPreparer<,>))
            .AddScoped(typeof(BulkPublishContentBatchMigrator<>))
            .AddScoped(typeof(BulkPublishContentBatchMigrator<,,>))
            .AddScoped(typeof(ItemPublishContentBatchMigrator<>))
            .AddScoped(typeof(ItemPublishContentBatchMigrator<,>))
            .AddScoped(typeof(ItemPublishContentBatchMigrator<,,>))
            .AddScoped(typeof(ItemPublishContentBatchMigrator<,,,>))
            .AddScoped(typeof(ContentMigrator<>));

        private static IServiceCollection AddConversionServices(this IServiceCollection services) => services
            //Register concrete types so that the easy way to get interface types is through IMigrationPipeline.
            .AddSingleton(typeof(DirectContentItemConverter<,>))
            // Schedule validators and converters
            .AddSingleton<IScheduleValidator<IServerSchedule>, ServerScheduleValidator>()
            .AddSingleton<IScheduleValidator<ICloudSchedule>, CloudScheduleValidator>()
            .AddSingleton<IScheduleConverter<IServerSchedule, ICloudSchedule>, ServerToCloudScheduleConverter>()
            .AddSingleton<IExtractRefreshTaskConverter<IServerExtractRefreshTask, ICloudExtractRefreshTask>, ServerToCloudExtractRefreshTaskConverter>()
            .AddSingleton<ISubscriptionConverter<IServerSubscription, ICloudSubscription>, ServerToCloudSubscriptionConverter>();

        private static IServiceCollection AddCacheServices(this IServiceCollection services) => services
            //Register concrete types so that the easy way to get interface types is through IMigrationPipeline.
            .AddScoped(typeof(BulkSourceCache<>))
            .AddScoped(typeof(BulkDestinationCache<>))
            .AddScoped<BulkDestinationProjectCache>()
            .AddScoped<IUserSavedCredentialsCache, UserSavedCredentialsCache>()
            .AddScoped<IDestinationAuthenticationConfigurationsCache, BulkApiAuthenticationConfigurationsCache>()
            .AddScoped<IEndpointViewCache, TableauApiEndpointViewCache>()
            .AddScoped<IEndpointWorkbookViewsCache, TableauApiEndpointWorkbookViewsCache>();

        private static IServiceCollection AddContentFinderServices(this IServiceCollection services) => services
            .AddScoped(typeof(ISourceContentReferenceFinder<>), typeof(ManifestSourceContentReferenceFinder<>))
            .AddScoped<ISourceContentReferenceFinderFactory, ManifestSourceContentReferenceFinderFactory>()
            .AddScoped(typeof(IDestinationContentReferenceFinder<>), typeof(ManifestDestinationContentReferenceFinder<>))
            .AddScoped<IDestinationContentReferenceFinderFactory, ManifestDestinationContentReferenceFinderFactory>()
            .AddScoped<IDestinationViewReferenceFinder, DestinationViewReferenceFinder>();

        private static IServiceCollection AddPipelineServices(this IServiceCollection services) => services
            .AddScoped<ServerToCloudMigrationPipeline>()
            .AddScoped<IMigrationPipelineFactory, MigrationPipelineFactory>()
            .AddScoped<IMigrationPipelineRunner, MigrationPipelineRunner>();

        private static IServiceCollection AddMigrationActionServices(this IServiceCollection services) => services
            .AddScoped<PreflightAction>()
            .AddScoped(typeof(MigrateContentAction<>));

        private static IServiceCollection AddMigrationCapabilityServices(this IServiceCollection services) => services
            .AddSingleton<IMigrationCapabilitiesEditor, MigrationCapabilities>()
            .AddSingleton<IMigrationCapabilities>(p => p.GetRequiredService<IMigrationCapabilitiesEditor>())
            .AddScoped<IMigrationCapabilityManager, EmbeddedCredentialsCapabilityManager>()
            .AddScoped<IMigrationCapabilityManager, SubscriptionsCapabilityManager>()
            .AddScoped<IMigrationCapabilityManager, GroupSetsCapabilityManager>();

        private static IServiceCollection AddDefaultPreflightHookServices(this IServiceCollection services) => services
            .AddScoped<InitializeCapabilitiesHook>();

        private static IServiceCollection AddDefaultFilterServices(this IServiceCollection services) => services
            .AddScoped(typeof(PreviouslyMigratedFilter<>))
            .AddScoped<GroupAllUsersFilter>()
            .AddScoped<UserSiteRoleSupportUserFilter>()
            .AddScoped(typeof(SystemOwnershipFilter<>))
            .AddScoped<FavoriteFilter>();

        private static IServiceCollection AddDefaultMappingServices(this IServiceCollection services) => services
            .AddScoped<AuthenticationTypeDomainMapping>()
            .AddScoped<TableauCloudUsernameMapping>()
            .AddScoped<FavoriteMapping>();

        private static IServiceCollection AddDefaultTransformerServices(this IServiceCollection services) => services
            .AddScoped<UserAuthenticationTypeTransformer>()
            .AddScoped<UserTableauCloudSiteRoleTransformer>()
            .AddScoped<GroupUsersTransformer>()
            .AddScoped<GroupSetGroupsTransformer>()
            .AddScoped(typeof(OwnershipTransformer<>))
            .AddScoped<TableauServerConnectionUrlTransformer>()
            .AddScoped<MappedReferenceExtractRefreshTaskTransformer>()
            .AddScoped(typeof(EncryptExtractTransformer<>))
            .AddScoped<PermissionsTransformer>()
            .AddScoped<IMappedUserTransformer, MappedUserTransformer>()
            .AddScoped(typeof(WorkbookReferenceTransformer<>))
            .AddScoped<CustomViewDefaultUserReferencesTransformer>()
            .AddScoped<SubscriptionTransformer>()
            .AddScoped<FavoriteTransformer>();

        private static IServiceCollection AddDefaultActionCompletedHookServices(this IServiceCollection services) => services;

        private static IServiceCollection AddDefaultPostPublishHookServices(this IServiceCollection services) => services
            .AddScoped(typeof(OwnerItemPostPublishHook<,>))
            .AddScoped(typeof(PermissionsItemPostPublishHook<,>))
            .AddScoped(typeof(TagItemPostPublishHook<,>))
            .AddScoped<ProjectPostPublishHook>()
            .AddScoped(typeof(ChildItemsPermissionsPostPublishHook<,>))
            .AddScoped<CustomViewDefaultUsersPostPublishHook>()
            .AddScoped(typeof(EmbeddedCredentialsItemPostPublishHook<,>))
            .AddScoped<DeleteUserFavoritesPostPublishHook>()
            .AddScoped<PopulateViewCachePostPublishHook>();

        private static IServiceCollection AddFileStoreServices(this IServiceCollection services) => services
            .AddScoped<MigrationDirectoryContentFileStore>()
            .AddScoped(s =>
            {
                /* Since this IContentFileStore factory is registered after AddMigrationApiClient,
                 * the factory might be running in the main migration DI scope
                 * (which should create a single scoped file store for the migration),
                 * or in one of the endpoint API client DI scopes
                 * (which should mimic AddMigrationApiClient factory's behavior of using the ApiClientInput overridden value).
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

        private static IServiceCollection AddContentClientServices(this IServiceCollection services) => services
            .AddScoped<IWorkbooksContentClient, WorkbooksContentClient>()
            .AddScoped<IViewsContentClient, ViewsContentClient>()
            .AddScoped<IFavoritesContentClient, FavoritesContentClient>();
    }
}
