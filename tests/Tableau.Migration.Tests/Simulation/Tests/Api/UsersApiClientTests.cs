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

using System;
using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class UsersApiClientTests
    {
        public class UsersApiClientTest : ApiClientTestBase
        {
            internal static void AddUserAssert(UsersResponse.UserType user, IResult<IAddUserResult> result)
            {
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.NotEqual(Guid.Empty, result.Value.Id);
                Assert.Equal(user.Name, result.Value.Name);
                Assert.Equal("Unlicensed", result.Value.SiteRole);
                Assert.Equal(user.AuthSetting, result.Value.AuthSetting);
            }
        }

        public class GetAllUsersAsync : UsersApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                const int USER_COUNT = 10;

                for (var i = 0; i != USER_COUNT; i++)
                {
                    Api.Data.AddUser(Create<UsersResponse.UserType>());
                }

                // Act
                var result = await sitesClient.Users.GetAllUsersAsync(1, 100, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task Returns_success_with_only_default_user()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act
                var result = await sitesClient.Users.GetAllUsersAsync(1, 100, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Single(result.Value);
            }
        }

        public class GetUserGroupsAsync : UsersApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var user = Create<UsersResponse.UserType>();

                const int ADDITIONAL_GROUP_COUNT = 3;

                // Add a few more users/groups
                for (int i = 0; i < ADDITIONAL_GROUP_COUNT; i++)
                {
                    Api.Data.AddUserToGroup(
                        Create<UsersResponse.UserType>(),
                        Create<GroupsResponse.GroupType>());
                }

                const int GROUP_COUNT = 10;

                for (var i = 0; i != GROUP_COUNT; i++)
                {
                    Api.Data.AddUserToGroup(
                        user,
                        Create<GroupsResponse.GroupType>());
                }

                // Act
                var result = await sitesClient.Users.GetUserGroupsAsync(user.Id, 1, 100, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(GROUP_COUNT, result.Value.Count);
                Assert.Equal(GROUP_COUNT, result.TotalCount);
            }

            [Fact]
            public async Task Returns_success_with_no_item_when_no_groups_exist()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var user = Create<UsersResponse.UserType>();

                const int ADDITIONAL_GROUP_COUNT = 3;

                // Add a few more users/groups
                for (int i = 0; i < ADDITIONAL_GROUP_COUNT; i++)
                {
                    Api.Data.AddUserToGroup(
                        Create<UsersResponse.UserType>(),
                        Create<GroupsResponse.GroupType>());
                }
                Api.Data.AddUser(user);

                // Act
                var result = await sitesClient.Users.GetUserGroupsAsync(user.Id, 1, 100, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Empty(result.Value);
            }
        }

        public class AddUserAsync : UsersApiClientTest
        {
            [Fact]
            public async Task Returns_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var user = Create<UsersResponse.UserType>();
                Assert.NotNull(user);
                Assert.NotNull(user.Name);
                Assert.NotNull(user.SiteRole);

                // Act
                var result = await sitesClient.Users.AddUserAsync(user.Name, user.SiteRole, user.AuthSetting, Cancel);

                // Assert
                AddUserAssert(user, result);
            }
        }

        public class UpdateUserAsync : UsersApiClientTest
        {
            [Fact]
            public async Task Returns_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var user = Create<UsersResponse.UserType>();
                Assert.NotNull(user);
                Assert.NotNull(user.Name);
                Assert.NotNull(user.SiteRole);

                // Add a user to update later
                var addResult = await sitesClient.Users.AddUserAsync(user.Name, user.SiteRole, user.AuthSetting, Cancel);

                // Check accuracy before proceeding with the real test
                AddUserAssert(user, addResult);

                // Act
                Assert.NotNull(addResult.Value);
                var addedUserId = addResult.Value.Id;
                var newFullName = "newFullName";
                var newSiteRole = "Viewer";
                var newAuthSetting = "newAuthSetting";

                var updateResult = await sitesClient.Users.UpdateUserAsync(id: addedUserId,
                                                                           newSiteRole: newSiteRole,
                                                                           cancel: Cancel,
                                                                           newfullName: newFullName,
                                                                           newAuthSetting: newAuthSetting);
                // Check the update result
                Assert.True(updateResult.Success);
                Assert.Equal(newFullName, updateResult.Value?.FullName);
                Assert.Equal(newSiteRole, updateResult.Value?.SiteRole);
                Assert.Equal(newAuthSetting, updateResult.Value?.AuthSetting);

                // Query the user and then check if the simulation tests really updated the user.
                var getResult = await sitesClient.Users.GetAllUsersAsync(1, 100, Cancel);

                Assert.True(getResult.Success);
                var updatedUser = getResult.Value.First(u => u.Id == addedUserId);
                Assert.Equal(newFullName, updatedUser.FullName);
                Assert.Equal(newSiteRole, updatedUser.SiteRole);
                Assert.Equal(newAuthSetting, updatedUser.AuthenticationType);
            }

            [Fact]
            public async Task Returns_error_when_nothing_to_update()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act                
                var addedUserId = Guid.NewGuid();
                var newFullName = "newFullName";
                var newSiteRole = "newSiteRole";
                var newAuthSetting = "newAuthSetting";
                var updateResult = await sitesClient.Users.UpdateUserAsync(id: addedUserId,
                                                                           newSiteRole: newSiteRole,
                                                                           cancel: Cancel,
                                                                           newfullName: newFullName,
                                                                           newAuthSetting: newAuthSetting);
                // Check the update result
                Assert.False(updateResult.Success);
                Assert.NotEmpty(updateResult.Errors);

                // Query the user and then check if the simulation tests didn't add the user.
                var getResult = await sitesClient.Users.GetAllUsersAsync(1, 100, Cancel);

                Assert.True(getResult.Success);
                var updatedUser = getResult.Value.FirstOrDefault(u => u.Id == addedUserId);
                Assert.Null(updatedUser);
            }
        }
    }
}
