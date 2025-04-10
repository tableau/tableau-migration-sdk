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
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Tableau.Migration.Api;
using Tableau.Migration.Api.EmbeddedCredentials;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.EmbeddedCredentials
{
    public sealed class EmbeddedCredentialsApiClientTests
    {
        public abstract class EmbeddedCredentialsApiClientTest : ApiTestBase
        {
            protected IEmbeddedCredentialsApiClient ApiClient { get; }

            protected Guid SiteId { get; } = Guid.NewGuid();

            public EmbeddedCredentialsApiClientTest()
            {
                MockSessionProvider.SetupGet(p => p.SiteId).Returns(() => SiteId);

                var wbApiClient = Dependencies.CreateClient<IWorkbooksApiClient>();
                ApiClient = new EmbeddedCredentialsApiClient(
                    Dependencies.RestRequestBuilderFactory,
                    Dependencies.MockLoggerFactory.Object,
                    Dependencies.MockSharedResourcesLocalizer.Object,
                    new(wbApiClient.UrlPrefix),
                    Dependencies.Serializer);
            }
        }
        #region - RetrieveKeychainAsync -

        public sealed class RetrieveKeychainAsync : EmbeddedCredentialsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<RetrieveKeychainResponse>(HttpStatusCode.InternalServerError);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var id = Guid.NewGuid();
                var options = Create<IDestinationSiteInfo>();

                var result = await ApiClient.RetrieveKeychainAsync(id, options, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri(
                    $"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{id}/retrievekeychain");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var retrieveKeychainResponse = AutoFixture.CreateResponse<RetrieveKeychainResponse>();
                var mockResponse = new MockHttpResponseMessage<RetrieveKeychainResponse>(
                    retrieveKeychainResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var id = Guid.NewGuid();
                var options = Create<IDestinationSiteInfo>();

                var result = await ApiClient.RetrieveKeychainAsync(id, options, Cancel);

                result.AssertSuccess();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri(
                    $"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{id}/retrievekeychain");

                var embeddedCredKeychain = result.Value;
                Assert.NotNull(embeddedCredKeychain);
                Assert.NotEmpty(embeddedCredKeychain.EncryptedKeychains);
                Assert.NotEmpty(embeddedCredKeychain.AssociatedUserIds);
            }

            [Fact]
            public async Task Success_with_empty_users_Async()
            {
                var retrieveKeychainResponse = AutoFixture
                    .Build<RetrieveKeychainResponse>()
                    .Without(r => r.Error)
                    .With(r => r.AssociatedUserLuidList, () => [])
                    .Create();

                var mockResponse = new MockHttpResponseMessage<RetrieveKeychainResponse>(
                    retrieveKeychainResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var id = Guid.NewGuid();
                var options = Create<IDestinationSiteInfo>();

                var result = await ApiClient.RetrieveKeychainAsync(id, options, Cancel);

                result.AssertSuccess();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri(
                    $"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{id}/retrievekeychain");

                var embeddedCredKeychain = result.Value;
                Assert.NotNull(embeddedCredKeychain);
                Assert.NotEmpty(embeddedCredKeychain.EncryptedKeychains);
                Assert.Empty(embeddedCredKeychain.AssociatedUserIds);
            }
        }

        #endregion

        #region - ApplyKeychainAsync -

        public sealed class ApplyKeychainAsync : EmbeddedCredentialsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.InternalServerError);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var id = Guid.NewGuid();
                var options = Create<IApplyKeychainOptions>();

                var result = await ApiClient.ApplyKeychainAsync(id, options, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{id}/applykeychain");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.OK);
                MockHttpClient.SetupResponse(mockResponse);

                var id = Guid.NewGuid();
                var options = Create<IApplyKeychainOptions>();

                var result = await ApiClient.ApplyKeychainAsync(id, options, Cancel);

                result.AssertSuccess();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/workbooks/{id}/applykeychain");
            }
        }

        #endregion
    }
}
