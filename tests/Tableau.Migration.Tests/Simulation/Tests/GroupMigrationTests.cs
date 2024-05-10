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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public class GroupMigrationTests
    {
        public class UsersBatch : ServerToCloud
        {
        }

        public class UsersIndividual : ServerToCloud
        {
            protected override bool UsersBatchImportEnabled => false;
        }

        public abstract class ServerToCloud : ServerToCloudSimulationTestBase
        {
            [Fact]
            public async Task MigratesAllGroupsToCloudAsync()
            {
                //Arrange - create source users to migrate.
                var (nonSupportUsers, supportUsers) = PrepareSourceUsersData();
                var groups = PrepareSourceGroupsData();

                //Migrate
                var plan = ServiceProvider.GetRequiredService<IMigrationPlanBuilder>()
                    .FromSource(SourceEndpointConfig)
                    .ToDestination(CloudDestinationEndpointConfig)
                    .ForServerToCloud()
                    .Build();

                var migrator = ServiceProvider.GetRequiredService<IMigrator>();
                var result = await migrator.ExecuteAsync(plan, Cancel);
                // Wait import all groups
                await Task.Delay(500);

                //Assert - all users should be migrated.

                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                Assert.Equal(groups.Count,
                    result.Manifest.Entries.ForContentType<IGroup>().Where(e => e.Status == MigrationManifestEntryStatus.Migrated).Count());
                Assert.Single(result.Manifest.Entries.ForContentType<IGroup>().Where(e => e.Status == MigrationManifestEntryStatus.Skipped));

                void AssertGroupMigrated(GroupsResponse.GroupType sourceGroup)
                {
                    var destinationGroup = Assert.Single(
                        CloudDestinationApi.Data.Groups.Where(
                            g => g.Name == sourceGroup.Name));

                    Assert.NotEqual(sourceGroup.Id, destinationGroup.Id);
                    Assert.Equal(sourceGroup.Name, destinationGroup.Name);
                    Assert.Equal(sourceGroup.Domain?.Name, destinationGroup.Domain?.Name);
                    Assert.Equal(sourceGroup.Import?.SiteRole, destinationGroup.Import?.SiteRole);
                }

                Assert.All(SourceApi.Data.Groups.Where(g => g.Name != "All Users"), AssertGroupMigrated);
            }
        }
    }
}
