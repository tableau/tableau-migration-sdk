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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public sealed class GroupSetsApiClientTests
    {
        public abstract class GroupSetsApiClientTest : ApiClientTestBase<IGroupSetsApiClient>
        {
            public GroupSetsApiClientTest()
            {
                MockGroupFinder.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), Cancel))
                    .ReturnsAsync((Guid id, CancellationToken c) =>
                    {
                        var mockResult = Create<Mock<IContentReference>>();
                        mockResult.SetupGet(x => x.Id).Returns(id);
                        return mockResult.Object;
                    });
            }
        }

        #region - AddGroupToGroupSetAsync -

        public sealed class AddGroupToGroupSetAsync : GroupSetsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();
                SetupExceptionResponse(exception);

                var result = await ApiClient.AddGroupToGroupSetAsync(Guid.NewGuid(), Guid.NewGuid(), Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Put);
                });
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var contentId = Guid.NewGuid();

                var result = await ApiClient.AddGroupToGroupSetAsync(Guid.NewGuid(), Guid.NewGuid(), Cancel);

                result.AssertFailure();
                Assert.Single(result.Errors);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Put);
                });
            }

            [Fact]
            public async Task SuccessAsync()
            {
                SetupSuccessResponse();

                var result = await ApiClient.AddGroupToGroupSetAsync(Guid.NewGuid(), Guid.NewGuid(), Cancel);

                result.AssertSuccess();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Put);
                });
            }
        }

        #endregion

        #region - RemoveGroupFromGroupSetAsync -

        public sealed class RemoveGroupFromGroupSetAsync : GroupSetsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();
                SetupExceptionResponse(exception);

                var result = await ApiClient.RemoveGroupFromGroupSetAsync(Guid.NewGuid(), Guid.NewGuid(), Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                });
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var contentId = Guid.NewGuid();

                var result = await ApiClient.RemoveGroupFromGroupSetAsync(Guid.NewGuid(), Guid.NewGuid(), Cancel);

                result.AssertFailure();
                Assert.Single(result.Errors);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                });
            }

            [Fact]
            public async Task SuccessAsync()
            {
                SetupSuccessResponse(statusCode: HttpStatusCode.NoContent);

                var result = await ApiClient.RemoveGroupFromGroupSetAsync(Guid.NewGuid(), Guid.NewGuid(), Cancel);

                result.AssertSuccess();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                });
            }
        }

        #endregion

        #region - List -

        public class ListClient : PagedListApiClientTestBase<IGroupSetsApiClient, IGroupSet, GroupSetsResponse>
        { }

        public class PageAccessor : ApiPageAccessorTestBase<IGroupSetsApiClient, IGroupSet, GroupSetsResponse>
        { }

        #endregion

        #region - GetByIdAsync -

        public sealed class GetByIdAsync : GroupSetsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();
                SetupExceptionResponse<GroupSetResponse>(exception);

                var contentId = Guid.NewGuid();

                var result = await ApiClient.GetByIdAsync(contentId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                });
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                SetupErrorResponse<GroupSetResponse>();

                var contentId = Guid.NewGuid();

                var result = await ApiClient.GetByIdAsync(contentId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                });
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var groupSetResponse = AutoFixture.CreateResponse<GroupSetResponse>();
                groupSetResponse.Item!.Groups = CreateMany<GroupSetResponse.GroupSetType.GroupType>().ToArray();

                var groups = new List<IContentReference>();
                foreach(var groupRef in groupSetResponse.Item.Groups)
                {
                    var g = Create<IContentReference>();
                    groups.Add(g);
                    MockGroupFinder.Setup(x => x.FindByIdAsync(groupRef.Id, Cancel))
                        .ReturnsAsync(g);
                }

                SetupSuccessResponse(groupSetResponse);

                var contentId = Guid.NewGuid();

                var result = await ApiClient.GetByIdAsync(contentId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                });

                var pulledGroupSet = Assert.IsType<PublishableGroupSet>(result.Value);

                Assert.Equal(groups, pulledGroupSet.Groups);
            }
        }

        #endregion

        #region - PullAsync -

        public sealed class PullAsync : GroupSetsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();
                SetupExceptionResponse<GroupSetResponse>(exception);

                var groupSet = Create<IGroupSet>();

                var result = await ApiClient.PullAsync(groupSet, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                });
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                SetupErrorResponse<GroupSetResponse>();

                var groupSet = Create<IGroupSet>();

                var result = await ApiClient.PullAsync(groupSet, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                });
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var groupSetResponse = AutoFixture.CreateResponse<GroupSetResponse>();
                groupSetResponse.Item!.Groups = CreateMany<GroupSetResponse.GroupSetType.GroupType>().ToArray();

                var groups = new List<IContentReference>();
                foreach (var groupRef in groupSetResponse.Item.Groups)
                {
                    var g = Create<IContentReference>();
                    groups.Add(g);
                    MockGroupFinder.Setup(x => x.FindByIdAsync(groupRef.Id, Cancel))
                        .ReturnsAsync(g);
                }

                SetupSuccessResponse(groupSetResponse);

                var groupSet = Create<IGroupSet>();

                var result = await ApiClient.PullAsync(groupSet, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                });

                Assert.Equal(groups, result.Value.Groups);
            }
        }

        #endregion

        #region - PublishAsync -

        public sealed class PublishAsync : GroupSetsApiClientTest
        {
            private bool OverwriteGroupSetGroupsEnabled { get; set; }

            public PublishAsync()
            {
                MockConfigReader.Setup(x => x.Get<IGroupSet>()).Returns(() =>
                {
                    return new()
                    {
                        OverwriteGroupSetGroupsEnabled = OverwriteGroupSetGroupsEnabled
                    };
                });
            }

            private void SetupCreateDuplicateGroupSetError()
                => SetupErrorResponse<CreateGroupSetResponse>(e => e.Code = RestErrorCodes.GROUP_SET_NAME_CONFLICT_ERROR_CODE);

            private Action<HttpRequestMessage> AssertCreateGroupSet() => r =>
            {
                r.AssertHttpMethod(HttpMethod.Post);
                AssertSiteUri(r, RestUrlKeywords.GroupSets);
            };

            private Action<HttpRequestMessage> AssertGetExistingGroupSet() => r =>
            {
                r.AssertHttpMethod(HttpMethod.Get);
                AssertSiteUri(r, RestUrlKeywords.GroupSets);
            };

            private Action<HttpRequestMessage> AssertPullExistingGroupSet(Guid groupSetId) => r =>
            {
                r.AssertHttpMethod(HttpMethod.Get);
                AssertSiteUri(r, $"{RestUrlKeywords.GroupSets}/{groupSetId}");
            };

            private Action<HttpRequestMessage> AssertAddGroup(Guid groupSetId, Guid groupId) => r =>
            {
                r.AssertHttpMethod(HttpMethod.Put);
                AssertSiteUri(r, $"{RestUrlKeywords.GroupSets}/{groupSetId}/{RestUrlKeywords.Groups}/{groupId}");
            };

            private Action<HttpRequestMessage> AssertRemoveGroup(Guid groupSetId, Guid groupId) => r =>
            {
                r.AssertHttpMethod(HttpMethod.Delete);
                AssertSiteUri(r, $"{RestUrlKeywords.GroupSets}/{groupSetId}/{RestUrlKeywords.Groups}/{groupId}");
            };

            [Fact]
            public async Task CreatesNewGroupSetAsync()
            {
                var createResponse = AutoFixture.CreateResponse<CreateGroupSetResponse>();
                var newGroupId = createResponse.Item!.Id;

                SetupSuccessResponse();
                SetupSuccessResponse(createResponse);

                var groupSet = Create<IPublishableGroupSet>();
                var result = await ApiClient.PublishAsync(groupSet, Cancel);

                result.AssertSuccess();

                var requestAsserts = groupSet.Groups.Select(g => AssertAddGroup(newGroupId, g.Id))
                    .Prepend(AssertCreateGroupSet());

                MockHttpClient.AssertAllRequests(requestAsserts);
            }

            [Fact]
            public async Task CreateGroupSetFailsAsync()
            {
                SetupErrorResponse<CreateGroupSetResponse>();

                var groupSet = Create<IPublishableGroupSet>();
                var result = await ApiClient.PublishAsync(groupSet, Cancel);

                result.AssertFailure();
                MockHttpClient.AssertSingleRequest(AssertCreateGroupSet());
            }

            [Fact]
            public async Task OverwriteExistingGroupSetFailsAsync()
            {
                SetupCreateDuplicateGroupSetError();
                SetupErrorResponse<GroupSetsResponse>();

                var groupSet = Create<IPublishableGroupSet>();
                var result = await ApiClient.PublishAsync(groupSet, Cancel);

                result.AssertFailure();
                MockHttpClient.AssertAllRequests([AssertCreateGroupSet(), AssertGetExistingGroupSet()]);
            }

            [Fact]
            public async Task OverwriteNoExistingGroupSetFoundAsync()
            {
                SetupCreateDuplicateGroupSetError();

                var existingGroupSetsResponse = AutoFixture.CreateResponse<GroupSetsResponse>();
                existingGroupSetsResponse.Items = Array.Empty<GroupSetsResponse.GroupSetType>();
                SetupSuccessResponse(existingGroupSetsResponse);

                var groupSet = Create<IPublishableGroupSet>();
                var result = await ApiClient.PublishAsync(groupSet, Cancel);

                result.AssertFailure();
                MockHttpClient.AssertAllRequests([AssertCreateGroupSet(), AssertGetExistingGroupSet()]);
            }

            [Fact]
            public async Task OverwriteMultipleExistingGroupSetFoundAsync()
            {
                SetupSuccessResponse<CreateGroupSetResponse>();
                SetupCreateDuplicateGroupSetError();

                var existingGroupSetsResponse = AutoFixture.CreateResponse<GroupSetsResponse>();
                existingGroupSetsResponse.Items = CreateMany<GroupSetsResponse.GroupSetType>(2).ToArray();
                SetupSuccessResponse(existingGroupSetsResponse);

                var groupSet = Create<IPublishableGroupSet>();
                var result = await ApiClient.PublishAsync(groupSet, Cancel);

                result.AssertFailure();
                MockHttpClient.AssertAllRequests([AssertCreateGroupSet(), AssertGetExistingGroupSet()]);
            }

            [Fact]
            public async Task OverwritePullExistingFailsAsync()
            {
                SetupSuccessResponse<CreateGroupSetResponse>();
                SetupCreateDuplicateGroupSetError();

                var existingGroupSet = Create<GroupSetsResponse.GroupSetType>();
                var existingGroupSetsResponse = AutoFixture.CreateResponse<GroupSetsResponse>();
                existingGroupSetsResponse.Items = [existingGroupSet];
                SetupSuccessResponse(existingGroupSetsResponse);

                SetupErrorResponse<GroupSetResponse>();

                var groupSet = Create<IPublishableGroupSet>();
                var result = await ApiClient.PublishAsync(groupSet, Cancel);

                result.AssertFailure();
                MockHttpClient.AssertAllRequests([AssertCreateGroupSet(), AssertGetExistingGroupSet(), AssertPullExistingGroupSet(existingGroupSet.Id)]);
            }

            [Fact]
            public async Task OverwritesExistingGroupSetAsync()
            {
                OverwriteGroupSetGroupsEnabled = true;
                var groupSet = Create<IPublishableGroupSet>();

                SetupSuccessResponse();
                SetupCreateDuplicateGroupSetError();

                var existingGroupSet = Create<GroupSetResponse.GroupSetType>();
                existingGroupSet.Groups = CreateMany<GroupSetResponse.GroupSetType.GroupType>(5).ToArray();

                var existingGroupSetList = Create<GroupSetsResponse.GroupSetType>();
                existingGroupSetList.Id = existingGroupSet.Id;

                var existingGroupSetsResponse = AutoFixture.CreateResponse<GroupSetsResponse>();
                existingGroupSetsResponse.Items = [existingGroupSetList];
                SetupSuccessResponse(existingGroupSetsResponse);

                var existingPullResponse = AutoFixture.CreateResponse<GroupSetResponse>();
                existingPullResponse.Item = existingGroupSet;
                SetupSuccessResponse(existingPullResponse);

                SetupSuccessResponse();

                var result = await ApiClient.PublishAsync(groupSet, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                Assert.Equal(existingGroupSet.Id, result.Value.Id);
                Assert.Equal(groupSet.Groups.Count, result.Value.Groups.Count);

                var requestAsserts = new[] { AssertCreateGroupSet(), AssertGetExistingGroupSet(), AssertPullExistingGroupSet(existingGroupSet.Id) }
                    .Concat(existingGroupSet.Groups.ToDictionary(g => g.Id, g => g).Keys.Select(id => AssertRemoveGroup(existingGroupSet.Id, id)))
                    .Concat(groupSet.Groups.Select(g => AssertAddGroup(existingGroupSet.Id, g.Id)));

                MockHttpClient.AssertAllRequests(requestAsserts);
            }

            [Fact]
            public async Task AppendsExistingGroupSetAsync()
            {
                OverwriteGroupSetGroupsEnabled = false;
                var groupSet = Create<IPublishableGroupSet>();

                SetupSuccessResponse();
                SetupCreateDuplicateGroupSetError();

                var existingGroupSet = Create<GroupSetResponse.GroupSetType>();
                existingGroupSet.Groups = CreateMany<GroupSetResponse.GroupSetType.GroupType>().ToArray();

                var existingGroupSetList = Create<GroupSetsResponse.GroupSetType>();
                existingGroupSetList.Id = existingGroupSet.Id;

                var existingGroupSetsResponse = AutoFixture.CreateResponse<GroupSetsResponse>();
                existingGroupSetsResponse.Items = [existingGroupSetList];
                SetupSuccessResponse(existingGroupSetsResponse);

                var existingPullResponse = AutoFixture.CreateResponse<GroupSetResponse>();
                existingPullResponse.Item = existingGroupSet;
                SetupSuccessResponse(existingPullResponse);

                SetupSuccessResponse();

                var result = await ApiClient.PublishAsync(groupSet, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                Assert.Equal(existingGroupSet.Id, result.Value.Id);
                Assert.Equal(groupSet.Groups.Count + existingGroupSet.Groups.Length, result.Value.Groups.Count);

                var requestAsserts = new[] { AssertCreateGroupSet(), AssertGetExistingGroupSet(), AssertPullExistingGroupSet(existingGroupSet.Id) }
                    .Concat(groupSet.Groups.Select(g => AssertAddGroup(existingGroupSet.Id, g.Id)));

                MockHttpClient.AssertAllRequests(requestAsserts);
            }
        }

        #endregion
    }
}
