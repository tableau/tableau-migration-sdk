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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class GroupsApiClientTests
    {
        public class GroupsApiClientTest : ApiClientTestBase
        { }

        public class GetAllGroupsAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                const int GROUP_COUNT = 10;

                for (var i = 0; i != GROUP_COUNT; i++)
                {
                    Api.Data.AddGroup(Create<GroupsResponse.GroupType>());
                }

                // Act
                var result = await sitesClient.Groups.GetAllGroupsAsync(1, 100, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task Returns_success_with_single_item_when_no_users_exist()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act
                var result = await sitesClient.Groups.GetAllGroupsAsync(1, 100, Cancel);

                // Assert
                Assert.True(result.Success);
                // AllUsers Group
                var group = Assert.Single(result.Value);
                Assert.NotNull(group);
                Assert.Equal("All Users", group.Name);
            }
        }

        public class GetGroupUsersAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var group = Create<GroupsResponse.GroupType>();

                const int ADDITIONAL_USER_COUNT = 3;

                // Add a few more users/groups
                for (int i = 0; i < ADDITIONAL_USER_COUNT; i++)
                {
                    Api.Data.AddUserToGroup(
                        Create<UsersResponse.UserType>(),
                        Create<GroupsResponse.GroupType>());
                }

                const int USER_COUNT = 10;

                for (var i = 0; i != USER_COUNT; i++)
                {
                    Api.Data.AddUserToGroup(
                        Create<UsersResponse.UserType>(),
                        group);
                }

                // Act
                var result = await sitesClient.Groups.GetGroupUsersAsync(group.Id, 1, 100, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(USER_COUNT, result.Value.Count);
                Assert.Equal(USER_COUNT, result.TotalCount);
            }

            [Fact]
            public async Task Returns_success_with_no_item_when_no_users_exist()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var group = Create<GroupsResponse.GroupType>();

                const int ADDITIONAL_USER_COUNT = 3;

                // Add a few more users/groups
                for (int i = 0; i < ADDITIONAL_USER_COUNT; i++)
                {
                    Api.Data.AddUserToGroup(
                        Create<UsersResponse.UserType>(),
                        Create<GroupsResponse.GroupType>());
                }
                Api.Data.AddGroup(group);

                // Act
                var result = await sitesClient.Groups.GetGroupUsersAsync(group.Id, 1, 100, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Empty(result.Value);
            }
        }

        public class AddUserToGroupAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var group = Create<GroupsResponse.GroupType>();
                var user = Create<UsersResponse.UserType>();

                Api.Data.AddGroup(group);
                Api.Data.AddUser(user);

                // Act
                var result = await sitesClient.Groups.AddUserToGroupAsync(group.Id, user.Id, Cancel);

                // Assert
                result.AssertSuccess();
                Assert.Equal(user.Id, result.Value!.Id);
                Assert.Equal(user.Name, result.Value.Name);
                Assert.Equal(user.SiteRole, result.Value.SiteRole);

                var groupUsersResult = await sitesClient.Groups.GetGroupUsersAsync(group.Id, 1, 100, Cancel);

                Assert.NotNull(groupUsersResult);
                Assert.True(groupUsersResult.Success);
                Assert.NotNull(groupUsersResult.Value);
                var groupUsers = groupUsersResult.Value.ToList();

                Assert.Contains(groupUsers, u => u.Id == user.Id);
            }
        }

        public class RemoveUserFromGroupAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var group = Create<GroupsResponse.GroupType>();
                var user = Create<UsersResponse.UserType>();

                Api.Data.AddGroup(group);
                Api.Data.AddUser(user);
                Api.Data.AddUserToGroup(user, group);

                // Act
                var result = await sitesClient.Groups.RemoveUserFromGroupAsync(group.Id, user.Id, Cancel);

                // Assert
                result.AssertSuccess();

                var groupUsersResult = await sitesClient.Groups.GetGroupUsersAsync(group.Id, 1, 100, Cancel);

                groupUsersResult.AssertSuccess();
                var groupUsers = groupUsersResult.Value!.ToList();

                Assert.Empty(groupUsers);
            }
        }

        public class ImportGroupFromActiveDirectoryAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var group = Create<GroupsResponse.GroupType>();

                // Act
                var result = await sitesClient.Groups.ImportGroupFromActiveDirectoryAsync(group?.Name ?? string.Empty, group?.Domain?.Name ?? string.Empty, group?.Import?.SiteRole ?? string.Empty, group?.Import?.GrantLicenseMode, Cancel);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                var groupResultValue = result.Value as IGroup;

                Assert.Equal(groupResultValue?.Name, group?.Name);
            }
        }

        public class CreateLocalGroupAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var group = Create<GroupsResponse.GroupType>();
                if (group?.Domain != null)
                {
                    group.Domain.Name = "local";
                }

                // Act
                var result = await sitesClient.Groups.CreateLocalGroupAsync(group?.Name ?? string.Empty, group?.Import?.SiteRole ?? string.Empty, Cancel);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                var groupResultValue = result.Value as IGroup;

                Assert.Equal(groupResultValue?.Name, group?.Name);
            }
        }

        public class PullAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task Returns_success_with_users()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var group = Create<GroupsResponse.GroupType>();
                var groupContent = new Group(group);

                const int ADDITIONAL_USER_COUNT = 3;

                // Add a few more users/groups
                for (int i = 0; i < ADDITIONAL_USER_COUNT; i++)
                {
                    Api.Data.AddUserToGroup(
                        Create<UsersResponse.UserType>(),
                        Create<GroupsResponse.GroupType>());
                }

                const int USER_COUNT = 10;

                for (var i = 0; i != USER_COUNT; i++)
                {
                    Api.Data.AddUserToGroup(
                        Create<UsersResponse.UserType>(),
                        group);
                }

                // Act
                var result = await sitesClient.Groups.PullAsync(groupContent, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Equal(group.Id, result.Value.Id);
                Assert.NotNull(result.Value.Users);
                Assert.NotEmpty(result.Value.Users);
                Assert.Equal(USER_COUNT, result.Value.Users.Count);
            }

            [Fact]
            public async Task Returns_success_with_many_users_and_pagination()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var group = Create<GroupsResponse.GroupType>();
                var groupContent = new Group(group);

                const int ADDITIONAL_USER_COUNT = 30;

                // Add a few more users/groups
                for (int i = 0; i < ADDITIONAL_USER_COUNT; i++)
                {
                    Api.Data.AddUserToGroup(
                        Create<UsersResponse.UserType>(),
                        Create<GroupsResponse.GroupType>());
                }

                const int USER_COUNT = 155;

                for (var i = 0; i != USER_COUNT; i++)
                {
                    Api.Data.AddUserToGroup(
                        Create<UsersResponse.UserType>(),
                        group);
                }

                // Act
                var result = await sitesClient.Groups.PullAsync(groupContent, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Equal(group.Id, result.Value.Id);
                Assert.NotNull(result.Value.Users);
                Assert.NotEmpty(result.Value.Users);
                Assert.Equal(USER_COUNT, result.Value.Users.Count);
            }

            [Fact]
            public async Task Returns_success_without_users()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var group = Create<GroupsResponse.GroupType>();
                var groupContent = new Group(group);

                const int ADDITIONAL_USER_COUNT = 3;

                // Add a few more users/groups
                for (int i = 0; i < ADDITIONAL_USER_COUNT; i++)
                {
                    Api.Data.AddUserToGroup(
                        Create<UsersResponse.UserType>(),
                        Create<GroupsResponse.GroupType>());
                }
                // An empty group
                Api.Data.AddGroup(group);

                // Act
                var result = await sitesClient.Groups.PullAsync(groupContent, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Equal(group.Id, result.Value.Id);
                Assert.NotNull(result.Value.Users);
                Assert.Empty(result.Value.Users);
            }
        }

        public class PublishAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task GroupWithoutUsers_importToAD_returns_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var group = Freeze<GroupsResponse.GroupType>();
                var groupContent = new PublishableGroup(
                    new Group(group),
                    new List<IGroupUser>());

                // Act
                var result = await sitesClient.Groups.PublishAsync(groupContent, Cancel);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task GroupWithUser_importToAD_returns_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var user = Freeze<UsersResponse.UserType>();
                Api.Data.AddUser(user);
                var group = Freeze<GroupsResponse.GroupType>();
                var groupContent = new PublishableGroup(
                    new Group(group),
                    new List<IGroupUser>
                    {
                        new GroupUser(
                            new User(user))
                    });

                // Act
                var result = await sitesClient.Groups.PublishAsync(groupContent, Cancel);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task GroupWithoutUsers_local_returns_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var group = Freeze<GroupsResponse.GroupType>();
                group.Domain!.Name = "local";
                var groupContent = new PublishableGroup(
                    new Group(group),
                    new List<IGroupUser>());

                // Act
                var result = await sitesClient.Groups.PublishAsync(groupContent, Cancel);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task GroupWithUser_local_returns_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var user = Freeze<UsersResponse.UserType>();
                Api.Data.AddUser(user);
                var group = Freeze<GroupsResponse.GroupType>();
                group.Domain!.Name = "local";
                var groupContent = new PublishableGroup(
                    new Group(group),
                    new List<IGroupUser>
                    {
                        new GroupUser(
                            new User(user))
                    });

                // Act
                var result = await sitesClient.Groups.PublishAsync(groupContent, Cancel);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }
    }
}
