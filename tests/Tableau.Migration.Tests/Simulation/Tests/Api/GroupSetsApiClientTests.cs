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

using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class GroupSetsApiClientTests
    {
        public class GroupSetsApiClientTest : ApiClientTestBase<IGroupSetsApiClient, GroupSetsResponse.GroupSetType>
        { }

        #region - GetAllGroupSetsAsync -

        public class GetAllGroupSetsAsync : GroupSetsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Create some test group sets
                var groupSet1 = Create<GroupSetsResponse.GroupSetType>();
                var groupSet2 = Create<GroupSetsResponse.GroupSetType>();

                Api.Data.AddGroupSet(groupSet1);
                Api.Data.AddGroupSet(groupSet2);

                // Act
                var result = await sitesClient.GroupSets.GetPageAsync(1, 10, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(2, result.Value.Count);
            }
        }

        #endregion

        #region - GetByIdAsync -

        public class GetByIdAsync : GroupSetsApiClientTest
        {
            [Fact]
            public async Task Returns_success_with_groups()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Create test group
                var groups = CreateMany<GroupsResponse.GroupType>(1);
                var group = groups.First();
                Api.Data.AddGroup(group);

                // Create test group set
                var groupSet = Create<GroupSetsResponse.GroupSetType>();

                Api.Data.AddGroupSet(groupSet);
                Api.Data.AddGroupToGroupSet(group.Id, groupSet.Id);

                // Act
                var result = await sitesClient.GroupSets.GetByIdAsync(groupSet.Id, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(groupSet.Id, result.Value.Id);
                Assert.Equal(groupSet.Name, result.Value.Name);
            }
        }

        #endregion


        #region - AddGroupToGroupSetAsync -

        public class AddGroupToGroupSetAsync : GroupSetsApiClientTest
        {
            [Fact]
            public async Task Adds_group_to_group_set()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Create test group
                var groups = CreateMany<GroupsResponse.GroupType>(1);
                var group = groups.First();
                Api.Data.AddGroup(group);

                // Create test group set
                var groupSet = Create<GroupSetsResponse.GroupSetType>();
                Api.Data.AddGroupSet(groupSet);

                // Act
                var result = await sitesClient.GroupSets.AddGroupToGroupSetAsync(groupSet.Id, group.Id, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);

                // Verify the group was added to the group set
                var groupSetGroups = Api.Data.GetGroupSetGroups(groupSet.Id);
                Assert.Contains(groupSetGroups, g => g.Id == group.Id);
            }
        }

        #endregion

        #region - RemoveGroupFromGroupSetAsync -

        public class RemoveGroupFromGroupSetAsync : GroupSetsApiClientTest
        {
            [Fact]
            public async Task Removes_group_from_group_set()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Create test group
                var groups = CreateMany<GroupsResponse.GroupType>(1);
                var group = groups.First();
                Api.Data.AddGroup(group);

                // Create test group set and add group to it
                var groupSet = Create<GroupSetsResponse.GroupSetType>();
                Api.Data.AddGroupSet(groupSet);
                Api.Data.AddGroupToGroupSet(group.Id, groupSet.Id);

                // Act
                var result = await sitesClient.GroupSets.RemoveGroupFromGroupSetAsync(groupSet.Id, group.Id, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);

                // Verify the group was removed from the group set
                var groupSetGroups = Api.Data.GetGroupSetGroups(groupSet.Id);
                Assert.DoesNotContain(groupSetGroups, g => g.Id == group.Id);
            }
        }

        #endregion
    }
}