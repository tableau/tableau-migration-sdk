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
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Tests.Unit.Api.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class DataSourcesApiClientTests
    {
        public abstract class DataSourcesApiClientTest : PermissionsApiClientTestBase<IDataSourcesApiClient>
        {
            public DataSourcesApiClientTest()
            {
                MockConfigReader
                    .Setup(x => x.Get<IDataSource>())
                    .Returns(new ContentTypesOptions());
            }
            internal DataSourcesApiClient DataSourcesApiClient => GetApiClient<DataSourcesApiClient>();
        }

        #region - List -

        public class ListClient : PagedListApiClientTestBase<IDataSourcesApiClient, IDataSource, DataSourcesResponse>
        { }

        public class PageAccessor : ApiPageAccessorTestBase<IDataSourcesApiClient, IDataSource, DataSourcesResponse>
        { }

        #endregion

        #region - Get -

        public class GetDataSourceAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<DataSourceResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var dataSourceId = Guid.NewGuid();

                var result = await ApiClient.GetDataSourceAsync(dataSourceId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/datasources/{dataSourceId}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<DataSourceResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var dataSourceId = Guid.NewGuid();
                var connections = CreateMany<IConnection>().ToImmutableArray();

                var result = await ApiClient.GetDataSourceAsync(dataSourceId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/datasources/{dataSourceId}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var dsResponse = AutoFixture.CreateResponse<DataSourceResponse>();
                dsResponse.Item!.Project = Create<DataSourceResponse.DataSourceType.ProjectType>();
                dsResponse.Item!.Owner = Create<DataSourceResponse.DataSourceType.OwnerType>();

                var project = Create<IContentReference>();
                MockProjectFinder.Setup(x => x.FindByIdAsync(dsResponse.Item.Project.Id, Cancel))
                    .ReturnsAsync(project);

                var owner = Create<IContentReference>();
                MockUserFinder.Setup(x => x.FindByIdAsync(dsResponse.Item.Owner.Id, Cancel))
                    .ReturnsAsync(owner);

                var mockResponse = new MockHttpResponseMessage<DataSourceResponse>(dsResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var dataSourceId = Guid.NewGuid();

                var result = await ApiClient.GetDataSourceAsync(
                    dataSourceId,
                    Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/datasources/{dataSourceId}");

                Assert.Same(project, ((IContainerContent)result.Value).Container);
                Assert.Same(owner, result.Value.Owner);
            }
        }

        #endregion

        #region - Download -

        public class DownloadDataSourceAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var dataSourceId = Guid.NewGuid();

                var result = await ApiClient.DownloadDataSourceAsync(dataSourceId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/datasources/{dataSourceId}/content?includeExtract=True");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var dataSourceId = Guid.NewGuid();

                var result = await ApiClient.DownloadDataSourceAsync(dataSourceId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/datasources/{dataSourceId}/content?includeExtract=True");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var content = new ByteArrayContent(Constants.DefaultEncoding.GetBytes("hi2u"));

                var mockResponse = new MockHttpResponseMessage(content);
                MockHttpClient.SetupResponse(mockResponse);

                var dataSourceId = Guid.NewGuid();

                var result = await ApiClient.DownloadDataSourceAsync(dataSourceId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/datasources/{dataSourceId}/content?includeExtract=True");
            }
        }

        #endregion

        #region - Update -

        public class UpdateDataSourceAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<UpdateDataSourceResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var dataSourceId = Guid.NewGuid();

                var result = await ApiClient.UpdateDataSourceAsync(dataSourceId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/datasources/{dataSourceId}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<UpdateDataSourceResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var dataSourceId = Guid.NewGuid();

                var result = await ApiClient.UpdateDataSourceAsync(dataSourceId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/datasources/{dataSourceId}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var dsResponse = AutoFixture.CreateResponse<UpdateDataSourceResponse>();
                dsResponse.Item!.Project = Create<UpdateDataSourceResponse.DataSourceType.ProjectType>();
                dsResponse.Item!.Owner = Create<UpdateDataSourceResponse.DataSourceType.OwnerType>();
                dsResponse.Item!.Job = Create<UpdateDataSourceResponse.DataSourceType.JobType>();

                var mockResponse = new MockHttpResponseMessage<UpdateDataSourceResponse>(dsResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var dataSourceId = Guid.NewGuid();

                var result = await ApiClient.UpdateDataSourceAsync(dataSourceId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/datasources/{dataSourceId}");
            }
        }

        #endregion

        #region - Connections -
        public class ListConnectionsAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task SuccessAsync()
            {
                var dataSourceId = Guid.NewGuid();
                var connectionsResponse = AutoFixture.CreateResponse<ConnectionsResponse>();

                var mockResponse = new MockHttpResponseMessage<ConnectionsResponse>(connectionsResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.GetConnectionsAsync(dataSourceId, Cancel);

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
