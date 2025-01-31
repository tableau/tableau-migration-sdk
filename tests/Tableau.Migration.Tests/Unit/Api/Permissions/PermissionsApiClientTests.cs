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
using System.Net.Http;
using System.Threading.Tasks;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Permissions
{
    public class PermissionsApiClientTests
    {
        public abstract class PermissionsApiClientTest : SiteApiTestBase
        {
            protected const string ContentTypePrefix = "test";

            internal readonly PermissionsApiClient ApiClient;

            public PermissionsApiClientTest()
            {
                var permissionsUriBuilder = new PermissionsUriBuilder(ContentTypePrefix);
                ApiClient = new(RestRequestBuilderFactory, Serializer, permissionsUriBuilder, MockSharedResourcesLocalizer.Object);
            }
        }

        public class GetPermissionsAsync : PermissionsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                var id = Guid.NewGuid();

                SetupSuccessResponse<PermissionsResponse>();

                var result = await ApiClient.GetPermissionsAsync(id, Cancel);

                result.AssertSuccess();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    AssertSiteUri(r, $"test/{id}/permissions");
                    r.AssertHttpMethod(HttpMethod.Get);
                });
            }

            [Fact]
            public async Task ReponseError()
            {
                SetupErrorResponse();

                var result = await ApiClient.GetPermissionsAsync(Guid.NewGuid(), Cancel);

                result.AssertFailure();
            }

            [Fact]
            public async Task RequestException()
            {
                SetupExceptionResponse();

                var result = await ApiClient.GetPermissionsAsync(Guid.NewGuid(), Cancel);

                result.AssertFailure();
            }
        }

        public class CreatePermissionsAsync : PermissionsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                var id = Guid.NewGuid();
                var destinationPermissions = Create<IPermissions>();

                SetupSuccessResponse<PermissionsResponse>();

                var result = await ApiClient.CreatePermissionsAsync(id, destinationPermissions, Cancel);

                result.AssertSuccess();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    AssertSiteUri(r, $"test/{id}/permissions");
                    r.AssertHttpMethod(HttpMethod.Put);
                });
            }

            [Fact]
            public async Task SkipsNoGrantees()
            {
                var destinationPermissions = Create<IPermissions>();
                destinationPermissions.GranteeCapabilities = Array.Empty<IGranteeCapability>();

                var result = await ApiClient.CreatePermissionsAsync(Guid.NewGuid(), destinationPermissions, Cancel);

                Assert.True(result.Success);

                MockHttpClient.AssertNoRequests();
            }

            [Fact]
            public async Task ReponseError()
            {
                var destinationPermissions = Create<IPermissions>();

                SetupErrorResponse();

                var result = await ApiClient.CreatePermissionsAsync(Guid.NewGuid(), destinationPermissions, Cancel);

                result.AssertFailure();
            }

            [Fact]
            public async Task RequestException()
            {
                var destinationPermissions = Create<IPermissions>();

                SetupExceptionResponse();

                var result = await ApiClient.CreatePermissionsAsync(Guid.NewGuid(), destinationPermissions, Cancel);

                result.AssertFailure();
            }
        }

        public class DeleteCapabilityAsync : PermissionsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                SetupSuccessResponse();

                var id = Guid.NewGuid();
                var result = await ApiClient.DeleteCapabilityAsync(id, Guid.NewGuid(), GranteeType.User, Create<ICapability>(), Cancel);

                result.AssertSuccess();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                });
            }

            [Fact]
            public async Task ReponseError()
            {
                SetupErrorResponse();

                var result = await ApiClient.DeleteCapabilityAsync(Guid.NewGuid(), Guid.NewGuid(), GranteeType.User, Create<ICapability>(), Cancel);

                result.AssertFailure();
            }

            [Fact]
            public async Task RequestException()
            {
                SetupExceptionResponse();

                var result = await ApiClient.DeleteCapabilityAsync(Guid.NewGuid(), Guid.NewGuid(), GranteeType.User, Create<ICapability>(), Cancel);

                result.AssertFailure();
            }
        }

        public class UpdatePermissionsAsync : PermissionsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                var id = Guid.NewGuid();

                SetupSuccessResponse();

                var result = await ApiClient.UpdatePermissionsAsync(id, Create<IPermissions>(), Cancel);

                result.AssertSuccess();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    AssertSiteUri(r, $"test/{id}/permissions");
                    r.AssertHttpMethod(HttpMethod.Post);
                });
            }

            [Fact]
            public async Task ReponseError()
            {
                SetupErrorResponse();

                var result = await ApiClient.UpdatePermissionsAsync(Guid.NewGuid(), Create<IPermissions>(), Cancel);

                result.AssertFailure();
            }

            [Fact]
            public async Task RequestException()
            {
                SetupExceptionResponse();

                var result = await ApiClient.UpdatePermissionsAsync(Guid.NewGuid(), Create<IPermissions>(), Cancel);

                result.AssertFailure();
            }
        }
    }
}
