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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Tests.Unit.Api.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class WorkbooksApiClientTests
    {
        public abstract class WorkbooksApiClientTest : PermissionsApiClientTestBase<IWorkbooksApiClient>
        {
            public WorkbooksApiClientTest()
            {
                MockConfigReader
                    .Setup(x => x.Get<IWorkbook>())
                    .Returns(new ContentTypesOptions());
            }
            internal WorkbooksApiClient WorkbooksApiClient => GetApiClient<WorkbooksApiClient>();
        }

        #region - List -

        public class ListClient : PagedListApiClientTestBase<IWorkbooksApiClient, IWorkbook, WorkbooksResponse>
        { }

        public class PageAccessor : ApiPageAccessorTestBase<IWorkbooksApiClient, IWorkbook, WorkbooksResponse>
        { }

        #endregion

        #region - Get -

        public class GetWorkbookAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<WorkbookResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var workbookId = Guid.NewGuid();

                var result = await ApiClient.GetWorkbookAsync(workbookId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{workbookId}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<WorkbookResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var workbookId = Guid.NewGuid();

                var result = await ApiClient.GetWorkbookAsync(workbookId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{workbookId}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var wbResponse = AutoFixture.CreateResponse<WorkbookResponse>();
                wbResponse.Item!.Project = Create<WorkbookResponse.WorkbookType.ProjectType>();
                wbResponse.Item!.Owner = Create<WorkbookResponse.WorkbookType.OwnerType>();

                var project = Create<IContentReference>();
                MockProjectFinder.Setup(x => x.FindByIdAsync(wbResponse.Item.Project.Id, Cancel))
                    .ReturnsAsync(project);

                var owner = Create<IContentReference>();
                MockUserFinder.Setup(x => x.FindByIdAsync(wbResponse.Item.Owner.Id, Cancel))
                    .ReturnsAsync(owner);


                MockViewsApiClient.Setup(x => x.Permissions.GetPermissionsAsync(It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(new Mock<IResult<IPermissions>>().Object);

                var mockResponse = new MockHttpResponseMessage<WorkbookResponse>(wbResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var workbookId = Guid.NewGuid();

                var result = await ApiClient.GetWorkbookAsync(workbookId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}/{workbookId}");

                Assert.Same(project, ((IContainerContent)result.Value).Container);
                Assert.Same(owner, result.Value.Owner);
            }
        }

        #endregion

        #region - GetAll -

        public class GetAllWorkbooksAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<WorkbooksResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetAllWorkbooksAsync(1, 1, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<WorkbooksResponse>(HttpStatusCode.Forbidden, null);
                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetAllWorkbooksAsync(1, 1, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var wbResponse = AutoFixture.CreateResponse<WorkbooksResponse>();
                wbResponse.Items = CreateMany<WorkbooksResponse.WorkbookType>(5).ToArray();

                var mockResponse = new MockHttpResponseMessage<WorkbooksResponse>(wbResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetAllWorkbooksAsync(1, 1, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/{UrlPrefix}");
            }
        }

        #endregion

        #region - Download -

        public class DownloadWorkbookAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var workbookId = Guid.NewGuid();

                var result = await ApiClient.DownloadWorkbookAsync(workbookId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{workbookId}/content?includeExtract=True");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var workbookId = Guid.NewGuid();

                var result = await ApiClient.DownloadWorkbookAsync(workbookId, Cancel);

                result.AssertFailure();


                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{workbookId}/content?includeExtract=True");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var content = new ByteArrayContent(Constants.DefaultEncoding.GetBytes("hi2u"));

                var mockResponse = new MockHttpResponseMessage(content);
                MockHttpClient.SetupResponse(mockResponse);

                var workbookId = Guid.NewGuid();

                var result = await ApiClient.DownloadWorkbookAsync(workbookId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{workbookId}/content?includeExtract=True");
            }
        }

        #endregion

        #region - Publish -

        public class PublishAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task Succeeds()
            {
                var mockFileStream = new Mock<Stream>();

                var mockContentFileStream = new Mock<IContentFileStream>();
                mockContentFileStream.SetupGet(s => s.Content).Returns(mockFileStream.Object);

                var mockContentFileHandle = new Mock<IContentFileHandle>();
                mockContentFileHandle.Setup(h => h.OpenReadAsync(Cancel)).ReturnsAsync(mockContentFileStream.Object);

                var mockPublishableWorkbook = Create<Mock<IPublishableWorkbook>>();
                mockPublishableWorkbook.SetupGet(wb => wb.File).Returns(mockContentFileHandle.Object);

                var publisherResult = Result<IWorkbookDetails>.Succeeded(Create<IWorkbookDetails>());

                MockWorkbookPublisher.Setup(p => p.PublishAsync(It.IsAny<IPublishWorkbookOptions>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(publisherResult);

                var result = await ApiClient.PublishAsync(mockPublishableWorkbook.Object, Cancel);

                result.AssertSuccess();

                MockWorkbookPublisher.Verify(p =>
                    p.PublishAsync(
                        It.Is<IPublishWorkbookOptions>(o => o.Name == mockPublishableWorkbook.Object.Name && o.File == mockFileStream.Object),
                        Cancel),
                    Times.Once);
            }
        }

        public class PublishWorkbookAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task Succeeds()
            {
                var mockOptions = new Mock<IPublishWorkbookOptions>();

                var publisherResult = Result<IWorkbookDetails>.Succeeded(Create<IWorkbookDetails>());

                MockWorkbookPublisher.Setup(p => p.PublishAsync(It.IsAny<IPublishWorkbookOptions>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(publisherResult);

                var result = await WorkbooksApiClient.PublishWorkbookAsync(mockOptions.Object, Cancel);

                Assert.Same(publisherResult, result);

                MockWorkbookPublisher.Verify(p => p.PublishAsync(mockOptions.Object, Cancel), Times.Once);
            }
        }

        #endregion

        #region - Update -

        public class UpdateWorkbookAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<UpdateWorkbookResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var workbookId = Guid.NewGuid();

                var result = await ApiClient.UpdateWorkbookAsync(workbookId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{workbookId}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<UpdateWorkbookResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var workbookId = Guid.NewGuid();

                var result = await ApiClient.UpdateWorkbookAsync(workbookId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{workbookId}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var wbResponse = AutoFixture.CreateResponse<UpdateWorkbookResponse>();

                var mockResponse = new MockHttpResponseMessage<UpdateWorkbookResponse>(wbResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var workbookId = Guid.NewGuid();

                var result = await ApiClient.UpdateWorkbookAsync(workbookId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{workbookId}");
            }
        }

        #endregion

        #region - Connections -
        public class ListConnectionsAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task SuccessAsync()
            {
                var workbookId = Guid.NewGuid();
                var connectionsResponse = AutoFixture.CreateResponse<ConnectionsResponse>();

                var mockResponse = new MockHttpResponseMessage<ConnectionsResponse>(connectionsResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetConnectionsAsync(workbookId, Cancel);

                result.AssertSuccess();
                var connections = result.Value;
                Assert.NotNull(connections);

                var expectedConnections = connectionsResponse.Items.Select(c => new Connection(c)).ToImmutableList();

                Assert.NotNull(expectedConnections);
                Assert.Equal(connections.Count, expectedConnections.Count);
            }
        }
        #endregion    
    }
}
