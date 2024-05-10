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
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Tags
{
    public class TagsApiClientTests
    {
        public class TagsApiClientTest : ApiTestBase
        {
            protected ITagsApiClient ApiClient { get; }

            protected Guid SiteId { get; } = Guid.NewGuid();

            public TagsApiClientTest()
            {
                MockSessionProvider.SetupGet(p => p.SiteId).Returns(() => SiteId);

                var wbApiClient = Dependencies.CreateClient<IWorkbooksApiClient>();
                ApiClient = new TagsApiClient(Dependencies.RestRequestBuilderFactory, Dependencies.MockLoggerFactory.Object,
                    Dependencies.MockSharedResourcesLocalizer.Object, new(wbApiClient.UrlPrefix), Dependencies.Serializer);
            }

            protected static ImmutableArray<ITag> ToTags(params string[] labels)
                => labels.Select(label => (ITag)new Tag(label)).ToImmutableArray();
        }

        #region - AddTagsAsync -

        public class AddTagsAsync : TagsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<AddTagsResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var itemId = Guid.NewGuid();
                var tags = ToTags("tag1", "Tag2");

                var result = await ApiClient.AddTagsAsync(itemId, tags, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{itemId}/tags");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<AddTagsResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var itemId = Guid.NewGuid();
                var tags = ToTags("tag1", "Tag2");

                var result = await ApiClient.AddTagsAsync(itemId, tags, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{itemId}/tags");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var tagsResponse = AutoFixture.CreateResponse<AddTagsResponse>();

                var mockResponse = new MockHttpResponseMessage<AddTagsResponse>(tagsResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var itemId = Guid.NewGuid();
                var tags = ToTags("tag1", "Tag2");

                var result = await ApiClient.AddTagsAsync(itemId, tags, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{itemId}/tags");
            }
        }

        #endregion

        #region - RemoveTagsAsync -

        public class RemoveTagsAsync : TagsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var itemId = Guid.NewGuid();
                var tags = ToTags("tag1", "Tag2");

                var result = await ApiClient.RemoveTagsAsync(itemId, tags, Cancel);

                result.AssertFailure();

                Assert.Equal(2, result.Errors.Count);
                mockResponse.Verify(x => x.EnsureSuccessStatusCode(), Times.Exactly(2));

                Assert.Equal(2, MockHttpClient.SentRequests.Count);
                Assert.Contains(MockHttpClient.SentRequests,
                    r => r.HasRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{itemId}/tags/tag1"));
                Assert.Contains(MockHttpClient.SentRequests,
                    r => r.HasRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{itemId}/tags/Tag2"));
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<EmptyTableauServerResponse>(HttpStatusCode.NotFound,
                    new EmptyTableauServerResponse(new Error()));
                MockHttpClient.SetupResponse(mockResponse);

                var itemId = Guid.NewGuid();
                var tags = ToTags("tag1", "Tag2");

                var result = await ApiClient.RemoveTagsAsync(itemId, tags, Cancel);

                result.AssertFailure();

                Assert.Equal(2, result.Errors.Count);

                Assert.Equal(2, MockHttpClient.SentRequests.Count);
                Assert.Contains(MockHttpClient.SentRequests,
                    r => r.HasRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{itemId}/tags/tag1"));
                Assert.Contains(MockHttpClient.SentRequests,
                    r => r.HasRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{itemId}/tags/Tag2"));
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.NoContent);
                MockHttpClient.SetupResponse(mockResponse);

                var itemId = Guid.NewGuid();
                var tags = ToTags("tag1", "Tag2");

                var result = await ApiClient.RemoveTagsAsync(itemId, tags, Cancel);

                result.AssertSuccess();

                Assert.Equal(2, MockHttpClient.SentRequests.Count);
                Assert.Contains(MockHttpClient.SentRequests,
                    r => r.HasRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{itemId}/tags/tag1"));
                Assert.Contains(MockHttpClient.SentRequests,
                    r => r.HasRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{itemId}/tags/Tag2"));
            }
        }

        #endregion

        #region - UpdateTagsAsync -

        public class UpdateTagsAsync : TagsApiClientTest
        {
            [Fact]
            public async Task AddsAndRemovesTagsAsync()
            {
                var addTagsResponse = AutoFixture.CreateResponse<AddTagsResponse>();
                addTagsResponse.Items = ToTags("tag1", "Tag2", "tag3", "tag4")
                    .Select(t => new AddTagsResponse.TagType(t))
                    .ToArray();

                var mockAddResponse = new MockHttpResponseMessage<AddTagsResponse>(addTagsResponse);
                var mockRemoveResponse = new MockHttpResponseMessage(HttpStatusCode.NoContent);
                MockHttpClient.SetupResponse(mockAddResponse);
                MockHttpClient.SetupResponse(mockRemoveResponse);

                var itemId = Guid.NewGuid();
                var expectedTags = ToTags("tag1", "Tag2");

                var result = await ApiClient.UpdateTagsAsync(itemId, expectedTags, Cancel);

                result.AssertSuccess();

                Assert.Equal(3, MockHttpClient.SentRequests.Count);
                Assert.Contains(MockHttpClient.SentRequests,
                    r => r.HasRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{itemId}/tags"));
                Assert.Contains(MockHttpClient.SentRequests,
                    r => r.HasRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{itemId}/tags/tag3"));
                Assert.Contains(MockHttpClient.SentRequests,
                    r => r.HasRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{itemId}/tags/tag4"));
            }
        }

        #endregion

    }
}
