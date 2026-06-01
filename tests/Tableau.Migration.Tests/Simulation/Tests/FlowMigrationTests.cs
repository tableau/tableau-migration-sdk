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
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    /// <summary>
    /// Simulation tests for flow migration.
    /// </summary>
    public class FlowMigrationTests
    {
        /// <summary>
        /// Batch users mode.
        /// </summary>
        public class UsersBatch : ServerToCloud
        {
        }

        /// <summary>
        /// Individual users mode.
        /// </summary>
        public class UsersIndividual : ServerToCloud
        {
            protected override bool UsersBatchImportEnabled => false;
        }

        /// <summary>
        /// Server-to-cloud flow migration tests.
        /// </summary>
        public abstract class ServerToCloud : ServerToCloudSimulationTestBase
        {
            [Fact]
            public async Task MigratesAllFlowsToCloudAsync()
            {
                // Arrange - create source content to migrate.
                var (NonSupportUsers, SupportUsers) = PrepareSourceUsersData(5);
                var groups = PrepareSourceGroupsData(5);
                var sourceProjects = PrepareSourceProjectsData();
                var sourceFlows = PrepareSourceFlowsData();

                // Migrate
                var result = await RunMigrationWithTableauIdAuthAsync();

                // Assert - migration completes without errors
                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                var migratedFlowEntries = result.Manifest.Entries.ForContentType<IFlow>().Where(e => e.Status == MigrationManifestEntryStatus.Migrated).ToList();

                Assert.NotEmpty(migratedFlowEntries);

                // When flows are migrated, assert count and destination simulator state (flow commit populates CloudDestinationApi.Data.Flows)
                if (migratedFlowEntries.Count > 0)
                {
                    Assert.Equal(sourceFlows.Count, migratedFlowEntries.Count);
                    Assert.Equal(sourceFlows.Count, CloudDestinationApi.Data.Flows.Count);

                    foreach (var entry in migratedFlowEntries)
                    {
                        var sourceFlow = SourceApi.Data.Flows.FirstOrDefault(f => f.Id == entry.Source.Id);
                        var destinationFlow = CloudDestinationApi.Data.Flows.FirstOrDefault(f => f.Id == entry.Destination!.Id);
                        Assert.NotNull(sourceFlow);
                        Assert.NotNull(destinationFlow);

                        if (SourceApi.Data.FlowPermissions.TryGetValue(sourceFlow.Id, out var sourcePerms)
                            && CloudDestinationApi.Data.FlowPermissions.TryGetValue(destinationFlow.Id, out var destPerms))
                        {
                            AssertPermissionsMigrated(result.Manifest, sourcePerms, destPerms);
                        }

                        if (SourceApi.Data.FlowKeychains.TryGetValue(sourceFlow.Id, out var sourceKeychains)
                            && CloudDestinationApi.Data.FlowKeychains.TryGetValue(destinationFlow.Id, out var destKeychains))
                        {
                            AssertEmbeddedCredentialsMigrated(result.Manifest,
                                sourceKeychains, destKeychains,
                                SourceApi.Data.UserSavedCredentials,
                                CloudDestinationApi.Data.UserSavedCredentials);
                        }
                    }
                }
            }

            [Fact]
            public async Task MappedToNonMigratedProjectAsync()
            {
                // Arrange
                var sourceFlows = PrepareSourceFlowsData();

                var destinationOnlyOwner = CloudDestinationApi.Data.AddUser(new()
                {
                    Id = Guid.NewGuid(),
                    Name = Create<string>(),
                    FullName = Create<string>(),
                    SiteRole = SiteRoles.Creator,
                    Domain = new() { Name = Constants.LocalDomain }
                });
                var destinationOnlyProject = CloudDestinationApi.Data.AddProject(new()
                {
                    Id = Guid.NewGuid(),
                    Name = Create<string>(),
                    ParentProjectId = null,
                    ContentPermissions = ContentPermissions.ManagedByOwner,
                    Owner = new() { Id = destinationOnlyOwner.Id }
                });

                // Migrate
                var planBuilder = CreateMigrationPlanBuilderWithTableauIdAuth();

                planBuilder.Mappings.Add<IFlow>(f =>
                {
                    return f.MapTo(new(destinationOnlyProject.Name!, f.MappedLocation.Name));
                });

                var result = await RunMigrationAsync(planBuilder);

                // Assert
                Assert.All(result.Manifest.Entries.ForContentType<IFlow>(), e => Assert.Equal(MigrationManifestEntryStatus.Migrated, e.Status));
            }
        }
    }
}
