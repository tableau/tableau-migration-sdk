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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public class GroupSetMigrationTests
    {
        public class UsersBatch : ServerToCloud
        { }

        public class UsersIndividual : ServerToCloud
        {
            protected override bool UsersBatchImportEnabled => false;
        }

        public class GroupOverWriteEnabled : ServerToCloud
        {
            protected override bool GroupSetGroupOverWriteEnabled => true;
        }

        public class GroupOverWriteDisabled : ServerToCloud
        {
            protected override bool GroupSetGroupOverWriteEnabled => false;
        }

        public abstract class ServerToCloud : ServerToCloudSimulationTestBase
        {
            #region Helper Methods

            private (List<UsersResponse.UserType> nonSupportUsers, List<UsersResponse.UserType> supportUsers, List<GroupsResponse.GroupType> sourceGroups) PrepareCommonTestData()
            {
                var (nonSupportUsers, supportUsers) = PrepareSourceUsersData();
                var sourceGroups = PrepareSourceGroupsData();
                return (nonSupportUsers, supportUsers, sourceGroups);
            }

            private async Task<(MigrationResult result, IEnumerable<IMigrationManifestEntry> groupSetEntries)> RunMigrationAndGetGroupSetEntriesAsync()
            {
                var result = await RunMigrationAsync();
                var groupSetEntries = result.Manifest.Entries.ForContentType<IGroupSet>();
                return (result, groupSetEntries);
            }

            private void AssertBasicMigrationSuccess(MigrationResult result, IEnumerable<IMigrationManifestEntry> groupSetEntries, int expectedGroupSetCount)
            {
                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                var migratedCount = groupSetEntries.Where(e => e.Status == MigrationManifestEntryStatus.Migrated).Count();
                Assert.Equal(expectedGroupSetCount, migratedCount);
            }

            private void AssertGroupSetMigrated(GroupSetsResponse.GroupSetType sourceGroupSet, bool hasExistingGroupSets = false)
            {
                var destinationGroupSet = Assert.Single(CloudDestinationApi.Data.GroupSets, gs => gs.Name == sourceGroupSet.Name);

                Assert.NotEqual(sourceGroupSet.Id, destinationGroupSet.Id);
                Assert.Equal(sourceGroupSet.Name, destinationGroupSet.Name);

                var sourceGroups = SourceApi.Data.GetGroupSetGroups(sourceGroupSet.Id);
                var destinationGroups = CloudDestinationApi.Data.GetGroupSetGroups(destinationGroupSet.Id);

                if (hasExistingGroupSets)
                {
                    if (GroupSetGroupOverWriteEnabled)
                    {
                        Assert.Equal(sourceGroups.Count(), destinationGroups.Count());
                    }
                    else
                    {
                        Assert.NotEqual(sourceGroups.Count(), destinationGroups.Count());
                    }
                }
                else
                {
                    Assert.Equal(sourceGroups.Count(), destinationGroups.Count());
                }

                foreach (var sourceGroup in sourceGroups)
                {
                    // Find the corresponding destination group by name
                    var correspondingDestinationGroup = CloudDestinationApi.Data.Groups
                        .First(g => g.Name == sourceGroup.Name);

                    // Verify the destination group is in the destination group set
                    Assert.Contains(destinationGroups, g => g.Id == correspondingDestinationGroup.Id);
                }
            }

            #endregion

            [Fact]
            public async Task MigratesAllGroupSetsToCloudAsync()
            {
                // Arrange - create source users, groups, and group sets to migrate
                var (nonSupportUsers, supportUsers, sourceGroups) = PrepareCommonTestData();
                var groupSets = PrepareSourceGroupSetsData(sourceGroups);

                // Act - migrate
                var (result, groupSetEntries) = await RunMigrationAndGetGroupSetEntriesAsync();

                // Assert - all group sets should be migrated
                AssertBasicMigrationSuccess(result, groupSetEntries, groupSets.Count);
                Assert.All(groupSets, sourceGroupSet => AssertGroupSetMigrated(sourceGroupSet, hasExistingGroupSets: false));
            }

            [Fact]
            public async Task MigratesAllGroupSetsToCloudWithExistingGroupSetsAsync()
            {
                // Arrange - create source users, groups, and group sets to migrate
                var (nonSupportUsers, supportUsers, sourceGroups) = PrepareCommonTestData();
                var destinationGroups = PrepareDestinationGroupsData();

                // Create group sets with matching names to test overwrite functionality
                var (groupSets, initialDestinationGroupSets) = PrepareMatchingGroupSetsData(sourceGroups, destinationGroups);

                // Act - migrate
                var (result, groupSetEntries) = await RunMigrationAndGetGroupSetEntriesAsync();

                // Assert - all group sets should be migrated
                AssertBasicMigrationSuccess(result, groupSetEntries, groupSets.Count);
                Assert.All(groupSets, sourceGroupSet => AssertGroupSetMigrated(sourceGroupSet, hasExistingGroupSets: true));
            }
        }
    }
}