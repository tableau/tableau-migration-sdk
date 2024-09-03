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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class GroupsApiClientTests
    {
        public abstract class GroupsApiClientTest : ApiClientTestBase<IGroupsApiClient>
        {
            public GroupsApiClientTest()
            {
                MockConfigReader
                    .Setup(x => x.Get<IUser>())
                    .Returns(new ContentTypesOptions());
            }
            internal GroupsApiClient GroupsApiClient => GetApiClient<GroupsApiClient>();
        }

        #region - List -

        public class ListClient : PagedListApiClientTestBase<IGroupsApiClient, IGroup, GroupsResponse>
        { }

        public class PageAccessor : ApiPageAccessorTestBase<IGroupsApiClient, IGroup, GroupsResponse>
        { }

        #endregion

        public class CreateLocalGroupAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task Returns_success()
            {
                var groupName = Create<string>();
                var siteRole = Create<string>();

                var response = AutoFixture.CreateResponse<CreateGroupResponse>();

                var mockResponse = new MockHttpResponseMessage<CreateGroupResponse>(response);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await GroupsApiClient.CreateLocalGroupAsync(groupName, siteRole, Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/groups");

                var requestContent = Assert.IsType<StringContent>(request.Content);

                var requestModel = await HttpContentSerializer.Instance.DeserializeAsync<CreateLocalGroupRequest>(requestContent, Cancel);

                Assert.NotNull(requestModel);
                Assert.NotNull(requestModel.Group);
                Assert.Equal(groupName, requestModel.Group.Name);
                Assert.Equal(siteRole, requestModel.Group.MinimumSiteRole);
            }

            [Fact]
            public async Task Returns_failure()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<CreateGroupResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await GroupsApiClient.CreateLocalGroupAsync(Create<string>(), Create<string>(), Cancel);

                Assert.False(result.Success);

                var error = Assert.Single(result.Errors);

                Assert.Same(exception, error);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/groups");
            }
        }

        public class AddUserToGroupAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                //Setup
                var addResponse = AutoFixture.CreateResponse<AddUserResponse>();
                Assert.NotNull(addResponse?.Item);

                var expectedUserId = addResponse?.Item?.Id;
                expectedUserId = Assert.NotNull(expectedUserId);

                var expectedGroupId = Guid.NewGuid();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(addResponse));

                //Act
                var result = await GroupsApiClient.AddUserToGroupAsync(expectedGroupId, expectedUserId.Value, Cancel);

                //Test
                result.AssertSuccess();
                var addUserResult = result.Value;
                Assert.NotNull(addUserResult);
                Assert.Equal(expectedUserId.Value, addUserResult.Id);
            }

            [Fact]
            public async Task Failure()
            {
                //Setup
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(HttpStatusCode.InternalServerError, null));

                //Act
                var result = await GroupsApiClient.AddUserToGroupAsync(Guid.NewGuid(), Guid.NewGuid(), Cancel);

                //Test
                result.AssertFailure();
                Assert.Single(result.Errors);
            }
        }

        public class RemoveUserFromGroupAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                //Setup
                var groupId = Guid.NewGuid();
                var userId = Guid.NewGuid();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.NoContent));

                //Act
                var result = await GroupsApiClient.RemoveUserFromGroupAsync(groupId, userId, Cancel);

                //Test
                result.AssertSuccess();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                    r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/groups/{groupId}/users/{userId}");
                });
            }

            [Fact]
            public async Task Failure()
            {
                //Setup
                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.InternalServerError, null));

                //Act
                var result = await GroupsApiClient.RemoveUserFromGroupAsync(Guid.NewGuid(), Guid.NewGuid(), Cancel);

                //Test
                result.AssertFailure();
            }
        }

        public class ImportAdGroupAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                //Setup
                var addResponse = AutoFixture.CreateResponse<CreateGroupResponse>();
                Assert.NotNull(addResponse?.Item);

                var group = new Group(addResponse);

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<CreateGroupResponse>(addResponse));

                //Act
                var result = await GroupsApiClient.ImportGroupFromActiveDirectoryAsync(group.Name, group.Domain, group.SiteRole ?? string.Empty, group.GrantLicenseMode, Cancel);

                //Test
                Assert.True(result.Success);
                Assert.Empty(result.Errors);
                Assert.Equal(group.Id, result.Value.Id);
            }

            [Fact]
            public async Task Failure()
            {
                //Setup
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<CreateGroupResponse>(HttpStatusCode.InternalServerError, null));

                //Act
                var result = await GroupsApiClient.ImportGroupFromActiveDirectoryAsync(string.Empty, string.Empty, string.Empty, null, CancellationToken.None);

                //Test
                Assert.False(result.Success);
                Assert.NotEmpty(result.Errors);
                Assert.Single(result.Errors);
                Assert.Null(result.Value);
            }
        }

        public class DeleteGroupAsync : GroupsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                //Setup
                var groupId = Guid.NewGuid();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.NoContent));

                //Act
                var result = await GroupsApiClient.DeleteGroupAsync(groupId, Cancel);

                //Test
                result.AssertSuccess();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                    r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/groups/{groupId}");
                });
            }

            [Fact]
            public async Task Failure()
            {
                //Setup
                var groupId = Guid.NewGuid();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.InternalServerError));

                //Act
                var result = await GroupsApiClient.DeleteGroupAsync(groupId, Cancel);

                //Test
                result.AssertFailure();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                    r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/groups/{groupId}");
                });
            }
        }

        public class PublishAsync : GroupsApiClientTest
        {
            private IPublishableGroup CreateGroup(Action<Mock<IPublishableGroup>>? configure = null)
            {
                var groupUsers = CreateMany<IGroupUser>().ToImmutableArray();

                var mockGroup = Create<Mock<IPublishableGroup>>();
                mockGroup.SetupGet(x => x.Domain).Returns(Constants.LocalDomain);
                mockGroup.SetupGet(x => x.Users).Returns(groupUsers);

                configure?.Invoke(mockGroup);

                return mockGroup.Object;
            }

            private void SetupAddUsersToGroup()
            {
                var addResponse = AutoFixture.CreateResponse<AddUserResponse>();
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(addResponse));
            }

            [Fact]
            public async Task Returns_success()
            {
                SetupAddUsersToGroup();

                var group = CreateGroup();

                var createGroupResponse = AutoFixture.CreateResponse<CreateGroupResponse>();
                createGroupResponse.Item!.Id = group.Id;

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<CreateGroupResponse>(createGroupResponse));

                var getUsersResponse = AutoFixture.CreateResponse<UsersResponse>();
                getUsersResponse.Items = Array.Empty<UsersResponse.UserType>();
                getUsersResponse.Pagination = new() { PageNumber = 1, PageSize = int.MaxValue, TotalAvailable = getUsersResponse.Items.Length };

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UsersResponse>(getUsersResponse));

                var result = await GroupsApiClient.PublishAsync(group, Cancel);

                Assert.True(result.Success);

                await AssertCreateGroupRequestAsync(MockHttpClient.SentRequests[0], group);
                AssertAddUsersToGroup(MockHttpClient, group);
            }

            [Fact]
            public async Task Succeeds_when_group_exists()
            {
                SetupAddUsersToGroup();

                var existingGroup = CreateGroup();

                var createGroupResponse = new CreateGroupResponse
                {
                    Error = new Error
                    {
                        Code = GroupsApiClient.GROUP_NAME_CONFLICT_ERROR_CODE
                    }
                };

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<CreateGroupResponse>(HttpStatusCode.Conflict, createGroupResponse));

                var getUsersResponse = AutoFixture.CreateResponse<UsersResponse>();
                getUsersResponse.Items = Array.Empty<UsersResponse.UserType>();
                getUsersResponse.Pagination = new() { PageNumber = 1, PageSize = int.MaxValue, TotalAvailable = getUsersResponse.Items.Length };

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UsersResponse>(getUsersResponse));

                var getGroupResponse = AutoFixture.CreateResponse<GroupsResponse>();
                getGroupResponse.Items = new[]
                {
                    new GroupsResponse.GroupType
                    {
                        Id = existingGroup.Id,
                        Domain = new() { Name = existingGroup.Domain },
                        Name = existingGroup.Name
                    }
                };

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<GroupsResponse>(getGroupResponse));

                var result = await GroupsApiClient.PublishAsync(existingGroup, Cancel);

                Assert.True(result.Success);

                await AssertCreateGroupRequestAsync(MockHttpClient.SentRequests[0], existingGroup);
                AssertGetGroupRequest(MockHttpClient.SentRequests[1], existingGroup.Domain, existingGroup.Name);
                AssertAddUsersToGroup(MockHttpClient, existingGroup);
            }

            [Fact]
            public async Task Removes_extra_users()
            {
                SetupAddUsersToGroup();

                var existingGroup = CreateGroup();

                var createGroupResponse = new CreateGroupResponse
                {
                    Error = new Error
                    {
                        Code = GroupsApiClient.GROUP_NAME_CONFLICT_ERROR_CODE
                    }
                };

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<CreateGroupResponse>(HttpStatusCode.Conflict, createGroupResponse));

                var getUsersResponse = AutoFixture.CreateResponse<UsersResponse>();
                getUsersResponse.Items = CreateMany<UsersResponse.UserType>().ToArray();
                getUsersResponse.Pagination = new() { PageNumber = 1, PageSize = int.MaxValue, TotalAvailable = getUsersResponse.Items.Length };

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UsersResponse>(getUsersResponse));
                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.NoContent, new StringContent(string.Empty)));

                var getGroupResponse = AutoFixture.CreateResponse<GroupsResponse>();
                getGroupResponse.Items = new[]
                {
                    new GroupsResponse.GroupType
                    {
                        Id = existingGroup.Id,
                        Domain = new() { Name = existingGroup.Domain },
                        Name = existingGroup.Name
                    }
                };

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<GroupsResponse>(getGroupResponse));

                var result = await GroupsApiClient.PublishAsync(existingGroup, Cancel);

                Assert.True(result.Success);

                await AssertCreateGroupRequestAsync(MockHttpClient.SentRequests[0], existingGroup);
                AssertGetGroupRequest(MockHttpClient.SentRequests[1], existingGroup.Domain, existingGroup.Name);
                AssertAddUsersToGroup(MockHttpClient, existingGroup);
                AssertRemoveUsersFromGroup(MockHttpClient, existingGroup.Id, getUsersResponse.Items);
            }

            [Fact]
            public async Task Returns_failure()
            {
                var group = CreateGroup();

                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<CreateGroupResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await GroupsApiClient.PublishAsync(group, Cancel);

                Assert.False(result.Success);

                var error = Assert.Single(result.Errors);

                Assert.Same(exception, error);
                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/groups");
            }



            #region - Assert Helpers -

            private async Task AssertCreateGroupRequestAsync(HttpRequestMessage r, IGroup group)
            {
                r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/groups");

                r.AssertHttpMethod(HttpMethod.Post);

                var content = Assert.IsType<StringContent>(r.Content);

                var model = await HttpContentSerializer.Instance.DeserializeAsync<CreateLocalGroupRequest>(content, Cancel);

                Assert.NotNull(model);
                Assert.NotNull(model.Group);

                Assert.Equal(group.Name, model.Group.Name);
                Assert.Equal(group.SiteRole, model.Group.MinimumSiteRole);
            }

            private void AssertAddUsersToGroup(MockHttpClient http, IPublishableGroup publishedGroup)
            {
                var expectedUrl = $"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/groups/{publishedGroup.Id}/users";
                Assert.Equal(publishedGroup.Users.Count,
                    http.SentRequests.Count(r => r.Method == HttpMethod.Post && r.HasRelativeUri(expectedUrl)));
            }

            private void AssertRemoveUsersFromGroup(MockHttpClient http, Guid groupId, IEnumerable<UsersResponse.UserType> usersToRemove)
            {
                Assert.All(usersToRemove, u =>
                {
                    var expectedUrl = $"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/groups/{groupId}/users/{u.Id}";
                    Assert.Single(http.SentRequests, r => r.Method == HttpMethod.Delete && r.HasRelativeUri(expectedUrl));
                });
            }

            private void AssertGetGroupRequest(HttpRequestMessage r, string domain, string name)
            {
                r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/groups");

                var filters = $"domainName:eq:{domain},name:eq:{name}";

                r.AssertQuery("filter", filters);

                r.AssertHttpMethod(HttpMethod.Get);
            }

            #endregion
        }

        /// <summary>
        /// We check for the cancellation token 2 times in PublishAsync
        /// Once during the "Delete users from group" loop 
        /// Once during the "Add users to group" loop
        /// 
        /// There are 3 tests
        /// - Before the delete loop (in the create group call)
        /// - During the delete loop
        /// - During the add loop
        /// </summary>
        public class PublishAsync_Cancellation : GroupsApiClientTest
        {
            [Fact]
            public async Task Publish_cancel_after_create_group()
            {
                var existingGroup = CreateGroup();

                // Create group response
                // This tests that cancel works before we loop over users
                var createGroupResponse = AutoFixture.CreateResponse<CreateGroupResponse>();
                createGroupResponse.Item!.Id = existingGroup.Id;

                MockHttpClient.SetupResponse(
                    mockResponse: new MockHttpResponseMessage<CreateGroupResponse>(createGroupResponse),
                    request: null,
                    onRequestSent: (m) =>
                    {
                        CancelSource.Cancel();
                    });


                // Setup Get user response
                // This may not be needed because we have the create group mock
                var getUsersResponse = AutoFixture.CreateResponse<UsersResponse>();
                getUsersResponse.Items = CreateMany<UsersResponse.UserType>().ToArray();
                getUsersResponse.Pagination = new() { PageNumber = 1, PageSize = int.MaxValue, TotalAvailable = getUsersResponse.Items.Length };

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UsersResponse>(getUsersResponse));

                // Setup delete response
                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.NoContent));

                // Setup Get group user response
                var getGroupResponse = AutoFixture.CreateResponse<GroupsResponse>();
                getGroupResponse.Items = new[]
                {
                    new GroupsResponse.GroupType
                    {
                        Id = existingGroup.Id,
                        Domain = new() { Name = existingGroup.Domain },
                        Name = existingGroup.Name
                    }
                };
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<GroupsResponse>(getGroupResponse));

                // Setup Add user to group 
                var addResponse = AutoFixture.CreateResponse<AddUserResponse>();
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(addResponse));


                // Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => GroupsApiClient.PublishAsync(existingGroup, Cancel));
            }

            [Fact]
            public async Task Publish_cancel_during_delete_user_from_group()
            {
                var existingGroup = CreateGroup();

                // Create group response
                var createGroupResponse = AutoFixture.CreateResponse<CreateGroupResponse>();
                createGroupResponse.Item!.Id = existingGroup.Id;
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<CreateGroupResponse>(createGroupResponse));

                // Setup Get user response
                var getUsersResponse = AutoFixture.CreateResponse<UsersResponse>();
                getUsersResponse.Items = CreateMany<UsersResponse.UserType>().ToArray();
                getUsersResponse.Pagination = new() { PageNumber = 1, PageSize = int.MaxValue, TotalAvailable = getUsersResponse.Items.Length };

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UsersResponse>(getUsersResponse));

                // Setup delete response
                MockHttpClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MockHttpResponseMessage(HttpStatusCode.NoContent, new StringContent(string.Empty)).Object)
                .Callback(() =>
                {
                    CancelSource.Cancel();
                });


                // Setup Get group user response
                var getGroupResponse = AutoFixture.CreateResponse<GroupsResponse>();
                getGroupResponse.Items = new[]
                {
                    new GroupsResponse.GroupType
                    {
                        Id = existingGroup.Id,
                        Domain = new() { Name = existingGroup.Domain },
                        Name = existingGroup.Name
                    }
                };
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<GroupsResponse>(getGroupResponse));

                // Setup Add user to group 
                var addResponse = AutoFixture.CreateResponse<AddUserResponse>();
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(addResponse));


                // Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => GroupsApiClient.PublishAsync(existingGroup, Cancel));
            }

            [Fact]
            public async Task Publish_cancel_during_add_user_to_group()
            {
                var existingGroup = CreateGroup();

                // Create group response
                var createGroupResponse = AutoFixture.CreateResponse<CreateGroupResponse>();
                createGroupResponse.Item!.Id = existingGroup.Id;
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<CreateGroupResponse>(createGroupResponse));

                // Setup Get user response
                var getUsersResponse = AutoFixture.CreateResponse<UsersResponse>();
                getUsersResponse.Items = CreateMany<UsersResponse.UserType>().ToArray();
                getUsersResponse.Pagination = new() { PageNumber = 1, PageSize = int.MaxValue, TotalAvailable = getUsersResponse.Items.Length };

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UsersResponse>(getUsersResponse));


                // Setup delete response
                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.NoContent));

                // Setup Get group user response
                var getGroupResponse = AutoFixture.CreateResponse<GroupsResponse>();
                getGroupResponse.Items = new[]
                {
                    new GroupsResponse.GroupType
                    {
                        Id = existingGroup.Id,
                        Domain = new() { Name = existingGroup.Domain },
                        Name = existingGroup.Name
                    }
                };
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<GroupsResponse>(getGroupResponse));

                // Setup Add user to group 
                var addResponse = AutoFixture.CreateResponse<AddUserResponse>();

                MockHttpClient.SetupResponse(
                    mockResponse: new MockHttpResponseMessage<AddUserResponse>(addResponse),
                    request: null,
                    onRequestSent: (m) =>
                    {
                        CancelSource.Cancel();
                    });


                // Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => GroupsApiClient.PublishAsync(existingGroup, Cancel));
            }

            #region - Helpers -

            private IPublishableGroup CreateGroup(Action<Mock<IPublishableGroup>>? configure = null)
            {
                var groupUsers = CreateMany<IGroupUser>().ToImmutableArray();

                var mockGroup = Create<Mock<IPublishableGroup>>();
                mockGroup.SetupGet(x => x.Domain).Returns(Constants.LocalDomain);
                mockGroup.SetupGet(x => x.Users).Returns(groupUsers);

                configure?.Invoke(mockGroup);

                return mockGroup.Object;
            }

            #endregion
        }
    }
}
