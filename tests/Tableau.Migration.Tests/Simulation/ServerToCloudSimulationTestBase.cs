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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Tests.Content.Permissions;
using Tableau.Migration.Tests.Simulation.DataPreparation;
using Tableau.Migration.Tests.Unit.Content.Permissions;
using Xunit;
using Server = Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Tests.Simulation
{
    /// <summary>
    /// <see cref="SimulationTestBase"/> implementation for test classes that require source and cloud destination servers (i.e. for migrations testing).
    /// </summary>
    public abstract class ServerToCloudSimulationTestBase : SimulationTestBase
    {
        protected TableauApiSimulator SourceApi { get; }

        protected TableauSiteConnectionConfiguration SourceSiteConfig { get; }

        public TableauApiEndpointConfiguration SourceEndpointConfig { get; }

        protected TableauApiSimulator CloudDestinationApi { get; }

        protected TableauSiteConnectionConfiguration CloudDestinationSiteConfig { get; }

        public TableauApiEndpointConfiguration CloudDestinationEndpointConfig { get; }

        public ServerToCloudSimulationTestBase(string sourceUrl = "https://source", string destinationUrl = "https://destination")
        {
            UsersResponse.UserType CreateDefaultUser()
            {
                var defaultUser = AutoFixture.Build<UsersResponse.UserType>()
                    .With(u => u.SiteRole, SiteRoles.ServerAdministrator)
                    .With(u => u.Name, $"SignedInUserName{Guid.NewGuid()}")
                    .With(u => u.FullName, $"SignedInUserFullName{Guid.NewGuid()}")
                    .Create();

                // Wrong - Work item in in backlog
                defaultUser.Name = $"{defaultUser.Domain!.Name}\\{defaultUser.Name}";

                return defaultUser;
            }

            SourceApi = RegisterTableauServerApiSimulator(sourceUrl, CreateDefaultUser());
            SourceSiteConfig = BuildSiteConnectionConfiguration(SourceApi);
            SourceEndpointConfig = new(SourceSiteConfig);

            CloudDestinationApi = RegisterTableauCloudApiSimulator(destinationUrl, CreateDefaultUser());
            CloudDestinationSiteConfig = BuildSiteConnectionConfiguration(CloudDestinationApi);
            CloudDestinationEndpointConfig = new(CloudDestinationSiteConfig);
        }

        protected virtual bool UsersBatchImportEnabled { get; } = true;

        protected virtual bool GroupSetGroupOverWriteEnabled { get; } = true;

        protected override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            var mockedConfigReader = Freeze<Mock<IConfigReader>>();
            mockedConfigReader.Setup(x => x.Get<IUser>())
                .Returns(new ContentTypesOptions
                {
                    BatchPublishingEnabled = UsersBatchImportEnabled
                });

            mockedConfigReader.Setup(x => x.Get<IGroupSet>())
                 .Returns(new ContentTypesOptions
                 {
                     OverwriteGroupSetGroupsEnabled = GroupSetGroupOverWriteEnabled
                 });

            mockedConfigReader.Setup(x => x.Get())
                .Returns(new MigrationSdkOptions());

            return services.AddTableauMigrationSdk()
                .AddSingleton(mockedConfigReader.Object);
        }

        #region - Migration -

        protected IMigrationPlan CreateMigrationPlan()
            => CreatePlanBuilder().Build();

        protected IServerToCloudMigrationPlanBuilder CreatePlanBuilder()
            => ServiceProvider.GetRequiredService<IMigrationPlanBuilder>()
                .FromSource(SourceEndpointConfig)
                .ToDestination(CloudDestinationEndpointConfig)
                .ForServerToCloud();

        protected IServerToCloudMigrationPlanBuilder CreateMigrationPlanBuilderWithTableauIdAuth()
            => CreatePlanBuilder()
                .WithTableauIdAuthenticationType()
                .WithTableauCloudUsernames("test.com");

        protected IMigrationPlan CreateMigrationPlanWithTableauIdAuth()
            => CreateMigrationPlanBuilderWithTableauIdAuth().Build();

        protected async Task<MigrationResult> RunMigrationAsync(IMigrationPlan? plan = null)
            => await RunMigrationAsync(plan ?? CreateMigrationPlan(), null);


        protected async Task<MigrationResult> RunMigrationAsync(IMigrationPlan plan, IMigrationManifest? previousManifest = null)
        {
            var migrator = ServiceProvider.GetRequiredService<IMigrator>();
            return previousManifest == null
                ? await migrator.ExecuteAsync(plan, Cancel)
                : await migrator.ExecuteAsync(plan, previousManifest, Cancel);
        }

        protected async Task<MigrationResult> RunMigrationAsync(IServerToCloudMigrationPlanBuilder planBuilder)
            => await RunMigrationAsync(planBuilder.Build());

        protected async Task<MigrationResult> RunMigrationWithTableauIdAuthAsync()
            => await RunMigrationAsync(CreateMigrationPlanWithTableauIdAuth());

        #endregion

        #region - Asserts -

        protected IContentReference? MapViewReference(IMigrationManifest manifest, Guid sourceViewId)
        {
            var sourceView = SourceApi.Data.Views.SingleOrDefault(v => v.Id == sourceViewId);
            if (sourceView?.Workbook is null)
            {
                return null;
            }

            var workbookEntries = manifest.Entries.ForContentType<IWorkbook>();
            var workbookEntry = workbookEntries.SingleOrDefault(e => e.Source.Id == sourceView.Workbook.Id);
            if (workbookEntry?.Destination is null)
            {
                return null;
            }

            var destinationWorkbook = CloudDestinationApi.Data.Workbooks.SingleOrDefault(w => w.Id == workbookEntry.Destination.Id);
            if (destinationWorkbook is null)
            {
                return null;
            }

            var destinationView = destinationWorkbook.Views.SingleOrDefault(v => string.Equals(v.Name, sourceView.Name, StringComparison.OrdinalIgnoreCase));
            if (destinationView is null)
            {
                return null;
            }

            return new ContentReferenceStub(destinationView.Id, destinationView.ContentUrl!, workbookEntry.Destination.Location.Append(destinationView.Name!));
        }

        protected IContentReference? MapReference<TContent>(IMigrationManifest manifest, Guid sourceId)
        {
            if (typeof(TContent) == typeof(IView))
            {
                return MapViewReference(manifest, sourceId);
            }

            var entries = manifest.Entries.ForContentType<TContent>();
            var entry = entries.Single(e => e.Source.Id == sourceId);
            return entry.Destination;
        }

        protected GranteeCapabilityType MapGrantee(IMigrationManifest manifest, GranteeCapabilityType capability)
            => capability.GranteeType switch
            {
                GranteeType.User => new()
                {
                    User = new()
                    {
                        Id = MapReference<IUser>(manifest, capability.GranteeId)!.Id
                    },
                    Capabilities = capability.Capabilities
                },
                GranteeType.Group => new()
                {
                    Group = new()
                    {
                        Id = MapReference<IGroup>(manifest, capability.GranteeId)!.Id
                    },
                    Capabilities = capability.Capabilities
                },
                _ => throw new ArgumentException($"Grantee Type {capability.GranteeType} is invalid.", nameof(capability)),
            };

        protected void AssertPermissionsMigrated(IMigrationManifest manifest, PermissionsType? sourcePermissions, PermissionsType? destinationPermissions)
        {
            Assert.NotNull(sourcePermissions);
            Assert.NotNull(destinationPermissions);

            var sourceGranteeCapabilities = sourcePermissions.GranteeCapabilities;
            var destinationGranteeCapabilities = destinationPermissions.GranteeCapabilities;

            Assert.NotNull(sourceGranteeCapabilities);
            Assert.NotNull(destinationGranteeCapabilities);

            var mappedGranteeCapabilities = sourceGranteeCapabilities.Select(g => MapGrantee(manifest, g));

            var comparer = new IGranteeCapabilityComparer(false);

            Assert.Equal(mappedGranteeCapabilities.ToIGranteeCapabilities(), destinationGranteeCapabilities.ToIGranteeCapabilities(), comparer);
        }

        protected void AssertEmbeddedCredentialsMigrated(
            IMigrationManifest manifest,
            RetrieveKeychainResponse sourceKeychains,
            RetrieveKeychainResponse destinationKeychains,
            ConcurrentDictionary<Guid, RetrieveKeychainResponse> sourceUserSavedCredentials,
            ConcurrentDictionary<Guid, RetrieveKeychainResponse> destinationUserSavedCredentials)
        {
            Assert.Equal(sourceKeychains.EncryptedKeychainList, destinationKeychains.EncryptedKeychainList);

            var sourceUserIds = sourceKeychains.AssociatedUserLuidList.ToList();

            var userMappings = GetUserMappings(manifest, sourceUserIds);

            var expectedUserIds = userMappings.Select(uid => uid.Value).ToArray();

            Assert.Equal(expectedUserIds, destinationKeychains.AssociatedUserLuidList);

            foreach (var userMapping in userMappings)
            {
                AssertSavedCredentials(sourceUserSavedCredentials, destinationUserSavedCredentials, userMapping.Key, userMapping.Value);
            }

            Dictionary<Guid, Guid> GetUserMappings(IMigrationManifest manifest, List<Guid> sourceUserIds)
            {
                var result = new Dictionary<Guid, Guid>();

                foreach (var sourceUserId in sourceUserIds)
                {
                    var destinationUser = MapReference<IUser>(manifest, sourceUserId);
                    if (destinationUser is null)
                    {
                        continue;
                    }
                    Assert.True(result.TryAdd(sourceUserId, destinationUser.Id));
                }

                return result;
            }

            static void AssertSavedCredentials(
                ConcurrentDictionary<Guid, RetrieveKeychainResponse> sourceUserSavedCredentials,
                ConcurrentDictionary<Guid, RetrieveKeychainResponse> destinationUserSavedCredentials,
                Guid sourceUserId,
                Guid destinationUserId)
            {
                sourceUserSavedCredentials.TryGetValue(sourceUserId, out RetrieveKeychainResponse? sourceCreds);

                if (sourceCreds is null)
                    return; ;

                destinationUserSavedCredentials.TryGetValue(destinationUserId, out RetrieveKeychainResponse? destinationCreds);
                Assert.NotNull(destinationCreds);

                var sourceUserKeychains = sourceCreds.EncryptedKeychainList;
                var destinationUserKeychains = destinationCreds.EncryptedKeychainList;
                Assert.Equal(sourceUserKeychains.Length, destinationUserKeychains.Length);

                var sourceUserLuids = sourceCreds.AssociatedUserLuidList;
                var destUserLuids = destinationCreds.AssociatedUserLuidList;
                Assert.Equal(sourceUserLuids.Length, destUserLuids.Length);

                foreach (var keyChain in sourceUserKeychains)
                {
                    Assert.Contains(destinationUserKeychains, kc => string.Equals(kc, keyChain, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        protected static void AssertScheduleMigrated(ScheduleResponse.ScheduleType sourceSchedule, ICloudScheduleType? destinationSchedule)
        {
            Assert.NotNull(destinationSchedule);

            // Assert schedule information
            // This can't be done completely without manually writing the source and destination schedules to compare against.
            // Server schedules requirements are different than Cloud schedule requirements. So we just check the frequence and start time.
            // We can check frequency because non of the source schedule we built will change frequency to cloud, even though that is a possibilty,
            // we just didn't include those.
            Assert.Equal(sourceSchedule.Frequency, destinationSchedule.Frequency);
            Assert.Equal(sourceSchedule.FrequencyDetails.Start, destinationSchedule.FrequencyDetails!.Start);
            if (sourceSchedule.FrequencyDetails.End is null)
            {
                Assert.Null(destinationSchedule.FrequencyDetails.End);
            }
            else
            {
                Assert.Equal(sourceSchedule.FrequencyDetails.End, destinationSchedule.FrequencyDetails.End);
            }
        }

        #endregion

        protected (List<UsersResponse.UserType> NonSupportUsers, List<UsersResponse.UserType> SupportUsers) PrepareSourceUsersData(int? count = null)
            => UsersDataPreparation.PrepareServerSource(SourceApi, AutoFixture, count);

        protected List<GroupsResponse.GroupType> PrepareSourceGroupsData(int? count = null)
            => GroupsDataPreparation.Prepare(SourceApi, AutoFixture, count);

        protected List<GroupsResponse.GroupType> PrepareDestinationGroupsData(int? count = null)
           => GroupsDataPreparation.Prepare(CloudDestinationApi, AutoFixture, count);

        protected List<ProjectsResponse.ProjectType> PrepareSourceProjectsData()
            => ProjectsDataPreparation.PrepareServerSource(SourceApi, AutoFixture);

        protected List<DataSourceResponse.DataSourceType> PrepareSourceDataSourceData()
            => DataSourcesDataPreparation.PrepareServerSource(SourceApi, AutoFixture);

        protected List<WorkbookResponse.WorkbookType> PrepareSourceWorkbooksData()
            => WorkbooksDataPreparation.PrepareServerSource(SourceApi, AutoFixture);

        protected List<Server.ExtractRefreshTasksResponse.TaskType> PrepareSourceExtractRefreshTasksData()
            => ExtractRefreshTasksDataPreparation.PrepareServerSource(SourceApi);

        protected List<CustomViewResponse.CustomViewType> PrepareSourceCustomViewsData()
            => CustomViewsDataPreparation.PrepareServerSource(SourceApi, AutoFixture);

        protected IImmutableList<Server.GetSubscriptionsResponse.SubscriptionType> PrepareSourceSubscriptionsData()
            => SubscriptionsDataPreparation.PrepareServerSource(SourceApi, AutoFixture);

        protected ImmutableDictionary<Guid, ConcurrentDictionary<(FavoriteContentType, Guid), string>> PrepareSourceFavoritesData()
            => FavoritesDataPreparation.PrepareServerSource(SourceApi, AutoFixture);

        protected List<GroupSetsResponse.GroupSetType> PrepareSourceGroupSetsData(List<GroupsResponse.GroupType>? groups, int? count = 3)
            => GroupSetsDataPreparation.Prepare(groups, SourceApi, AutoFixture, count);

        protected List<GroupSetsResponse.GroupSetType> PrepareDestinationGroupSetsData(List<GroupsResponse.GroupType>? groups, int? count = 3)
            => GroupSetsDataPreparation.Prepare(groups, CloudDestinationApi, AutoFixture, count);

        protected (List<GroupSetsResponse.GroupSetType> sourceGroupSets, List<GroupSetsResponse.GroupSetType> destinationGroupSets) PrepareMatchingGroupSetsData(
            List<GroupsResponse.GroupType> sourceGroups, List<GroupsResponse.GroupType> destinationGroups)
            => GroupSetsDataPreparation.PrepareMatching(sourceGroups, destinationGroups, SourceApi, CloudDestinationApi, AutoFixture);

    }
}
