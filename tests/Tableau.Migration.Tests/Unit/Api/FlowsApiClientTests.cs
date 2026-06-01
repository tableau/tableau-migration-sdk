//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Tests.Unit.Api.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class FlowsApiClientTests
    {
        public abstract class FlowsApiClientTest : PermissionsApiClientTestBase<IFlowsApiClient>
        {
            public FlowsApiClientTest()
            {
                MockConfigReader
                    .Setup(x => x.Get<IFlow>())
                    .Returns(new ContentTypesOptions());
            }

            internal FlowsApiClient FlowsApiClient => GetApiClient<FlowsApiClient>();
        }

        #region - List -

        public class ListClient : NameSearchApiClientTestBase<IFlowsApiClient, IFlow, FlowsResponse>
        { }

        public class PageAccessor : ApiFilteredPageAccessorTestBase<IFlowsApiClient, IFlow, FlowsResponse>
        { }

        #endregion

        #region - Get -

        public class GetByIdAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<FlowResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();

                var result = await ((IReadApiClient<IFlowDetails>)ApiClient).GetByIdAsync(flowId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{flowId}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<FlowResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();

                var result = await ((IReadApiClient<IFlowDetails>)ApiClient).GetByIdAsync(flowId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{flowId}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var flowResponse = AutoFixture.CreateResponse<FlowResponse>();
                flowResponse.Item!.Project = Create<FlowResponse.FlowType.ProjectType>();
                flowResponse.Item!.Owner = Create<FlowResponse.FlowType.OwnerType>();
                
                // Add flow output steps
                flowResponse.FlowOutputSteps = new[]
                {
                    new FlowResponse.FlowOutputStepType { Id = Guid.NewGuid(), Name = "Output Step 1" },
                    new FlowResponse.FlowOutputStepType { Id = Guid.NewGuid(), Name = "Output Step 2" }
                };

                var project = Create<IContentReference>();
                MockProjectFinder.Setup(x => x.FindByIdAsync(flowResponse.Item.Project.Id, Cancel))
                    .ReturnsAsync(project);

                var owner = Create<IContentReference>();
                MockUserFinder.Setup(x => x.FindByIdAsync(flowResponse.Item.Owner.Id, Cancel))
                    .ReturnsAsync(owner);

                var mockResponse = new MockHttpResponseMessage<FlowResponse>(flowResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();

                var result = await ((IReadApiClient<IFlowDetails>)ApiClient).GetByIdAsync(flowId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);
                Assert.Equal(2, result.Value.FlowOutputSteps.Count);
                Assert.Equal("Output Step 1", result.Value.FlowOutputSteps[0].Name);
                Assert.Equal("Output Step 2", result.Value.FlowOutputSteps[1].Name);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{flowId}");

                Assert.Same(project, ((IContainerContent)result.Value).Container);
                Assert.Same(owner, result.Value.Owner);
            }
        }

        #endregion

        #region - GetPageAsync -

        public class GetPageAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<FlowsResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetPageAsync(1, 1, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<FlowsResponse>(HttpStatusCode.Forbidden, null);
                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetPageAsync(1, 1, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var flowResponse = AutoFixture.CreateResponse<FlowsResponse>();
                flowResponse.Items = CreateMany<FlowsResponse.FlowType>(5).ToArray();

                foreach (var item in flowResponse.Items)
                {
                    item.Project = Create<FlowsResponse.FlowType.ProjectType>();
                    item.Owner = Create<FlowsResponse.FlowType.OwnerType>();

                    var project = Create<IContentReference>();
                    MockProjectFinder.Setup(x => x.FindByIdAsync(item.Project.Id, Cancel))
                        .ReturnsAsync(project);

                    var owner = Create<IContentReference>();
                    MockUserFinder.Setup(x => x.FindByIdAsync(item.Owner.Id, Cancel))
                        .ReturnsAsync(owner);
                }

                var mockResponse = new MockHttpResponseMessage<FlowsResponse>(flowResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetPageAsync(1, 1, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);
                Assert.Equal(5, result.Value.Count);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            }

            [Fact]
            public async Task SuccessWithPersonalSpaceAsync()
            {
                var flowResponse = AutoFixture.CreateResponse<FlowsResponse>();
                flowResponse.Items = CreateMany<FlowsResponse.FlowType>(3).ToArray();

                // First flow has project (normal)
                var project0 = Create<FlowsResponse.FlowType.ProjectType>();
                var owner0 = Create<FlowsResponse.FlowType.OwnerType>();
                flowResponse.Items[0].Project = project0;
                flowResponse.Items[0].Owner = owner0;
                
                var project = Create<IContentReference>();
                MockProjectFinder.Setup(x => x.FindByIdAsync(project0.Id, Cancel))
                    .ReturnsAsync(project);

                var owner = Create<IContentReference>();
                MockUserFinder.Setup(x => x.FindByIdAsync(owner0.Id, Cancel))
                    .ReturnsAsync(owner);

                // Second flow has null project (personal space - should be skipped)
                flowResponse.Items[1].Project = null;
                flowResponse.Items[1].Owner = Create<FlowsResponse.FlowType.OwnerType>();

                // Third flow has project (normal)
                var project2Type = Create<FlowsResponse.FlowType.ProjectType>();
                var owner2Type = Create<FlowsResponse.FlowType.OwnerType>();
                flowResponse.Items[2].Project = project2Type;
                flowResponse.Items[2].Owner = owner2Type;
                
                var project2 = Create<IContentReference>();
                MockProjectFinder.Setup(x => x.FindByIdAsync(project2Type.Id, Cancel))
                    .ReturnsAsync(project2);

                var owner2 = Create<IContentReference>();
                MockUserFinder.Setup(x => x.FindByIdAsync(owner2Type.Id, Cancel))
                    .ReturnsAsync(owner2);

                var mockResponse = new MockHttpResponseMessage<FlowsResponse>(flowResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetPageAsync(1, 1, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);
                Assert.Equal(2, result.Value.Count); // Only 2 flows, personal space flow skipped

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            }
        }

        #endregion

        #region - Download -

        public class DownloadFlowAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();

                var result = await ApiClient.DownloadFlowAsync(flowId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/flows/{flowId}/{RestUrlKeywords.Content}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();

                var result = await ApiClient.DownloadFlowAsync(flowId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/flows/{flowId}/{RestUrlKeywords.Content}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var content = new ByteArrayContent(Constants.DefaultEncoding.GetBytes("hi2u"));

                var mockResponse = new MockHttpResponseMessage(content);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();

                var result = await ApiClient.DownloadFlowAsync(flowId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/flows/{flowId}/{RestUrlKeywords.Content}");
            }
        }

        #endregion

        // Note: PullAsync and PublishAsync are tested through integration tests in FlowPublisherTests.cs
        // as they involve complex interactions with IContentFileStore and IFlowPublisher.

        #region - Update -

        public class UpdateFlowAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<UpdateFlowResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();
                var newProjectId = Guid.NewGuid();

                var result = await ApiClient.UpdateFlowAsync(flowId, Cancel, newProjectId: newProjectId);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{flowId}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var updateResponse = AutoFixture.CreateResponse<UpdateFlowResponse>();
                updateResponse.Item!.Project = new UpdateFlowResponse.FlowType.ProjectType
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Project"
                };
                updateResponse.Item!.Owner = new UpdateFlowResponse.FlowType.OwnerType
                {
                    Id = Guid.NewGuid()
                };

                var mockResponse = new MockHttpResponseMessage<UpdateFlowResponse>(updateResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();
                var newProjectId = Guid.NewGuid();
                var newOwnerId = Guid.NewGuid();

                var result = await ApiClient.UpdateFlowAsync(flowId, Cancel, newProjectId, newOwnerId);

                result.AssertSuccess();
                Assert.NotNull(result.Value);
                Assert.Equal(updateResponse.Item.Id, result.Value.Id);
                Assert.Equal(updateResponse.Item.Project.Id, result.Value.ProjectId);
                Assert.Equal(updateResponse.Item.Owner.Id, result.Value.OwnerId);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{flowId}");
            }
        }

        #endregion

        #region - Connections -

        public class GetConnectionsAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<ConnectionsResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();

                var result = await ApiClient.GetConnectionsAsync(flowId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{flowId}/{RestUrlKeywords.Connections}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var connectionsResponse = new ConnectionsResponse
                {
                    Items = new[]
                    {
                        new ConnectionsResponse.ConnectionType
                        {
                            Id = Guid.NewGuid(),
                            Type = "sqlserver",
                            ServerAddress = "server1",
                            ConnectionUsername = "user1",
                            EmbedPassword = "true"
                        },
                        new ConnectionsResponse.ConnectionType
                        {
                            Id = Guid.NewGuid(),
                            Type = "postgres",
                            ServerAddress = "server2",
                            ConnectionUsername = "user2",
                            EmbedPassword = "false"
                        }
                    }
                };

                var mockResponse = new MockHttpResponseMessage<ConnectionsResponse>(connectionsResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();

                var result = await ApiClient.GetConnectionsAsync(flowId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);
                Assert.Equal(2, result.Value.Count);
                Assert.Equal("sqlserver", result.Value[0].Type);
                Assert.True(result.Value[0].EmbedPassword);
                Assert.Equal("postgres", result.Value[1].Type);
                Assert.False(result.Value[1].EmbedPassword);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{flowId}/{RestUrlKeywords.Connections}");
            }
        }

        public class UpdateConnectionAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<ConnectionResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();
                var connectionId = Guid.NewGuid();
                var options = new Mock<IUpdateConnectionOptions>().Object;

                var result = await ApiClient.UpdateConnectionAsync(flowId, connectionId, options, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{flowId}/{RestUrlKeywords.Connections}/{connectionId}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var connectionResponse = new ConnectionResponse
                {
                    Item = new ConnectionResponse.ConnectionType
                    {
                        Id = Guid.NewGuid(),
                        Type = "sqlserver",
                        ServerAddress = "updatedServer",
                        ConnectionUsername = "updatedUser",
                        EmbedPassword = "true"
                    }
                };

                var mockResponse = new MockHttpResponseMessage<ConnectionResponse>(connectionResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();
                var connectionId = Guid.NewGuid();
                
                var mockOptions = new Mock<IUpdateConnectionOptions>();
                mockOptions.Setup(o => o.ServerAddress).Returns("updatedServer");
                mockOptions.Setup(o => o.ServerPort).Returns("1433");
                mockOptions.Setup(o => o.ConnectionUsername).Returns("updatedUser");
                mockOptions.Setup(o => o.Password).Returns("newPassword");
                mockOptions.Setup(o => o.EmbedPassword).Returns(true);

                var result = await ApiClient.UpdateConnectionAsync(flowId, connectionId, mockOptions.Object, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);
                Assert.Equal(connectionResponse.Item.Id, result.Value.Id);
                Assert.Equal("updatedServer", result.Value.ServerAddress);
                Assert.True(result.Value.EmbedPassword);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{flowId}/{RestUrlKeywords.Connections}/{connectionId}");
            }
        }

        #endregion

        #region - Ownership -

        public class ChangeOwnerAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task CallsUpdateFlowAsync()
            {
                var updateResponse = AutoFixture.CreateResponse<UpdateFlowResponse>();
                updateResponse.Item!.Project = new UpdateFlowResponse.FlowType.ProjectType
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Project"
                };
                updateResponse.Item!.Owner = new UpdateFlowResponse.FlowType.OwnerType
                {
                    Id = Guid.NewGuid()
                };

                var mockResponse = new MockHttpResponseMessage<UpdateFlowResponse>(updateResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var flowId = Guid.NewGuid();
                var newOwnerId = Guid.NewGuid();

                var result = await ApiClient.ChangeOwnerAsync(flowId, newOwnerId, Cancel);

                result.AssertSuccess();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{flowId}");
            }
        }

        #endregion
    }
}
