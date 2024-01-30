// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Permissions;
using Xunit;
using Xunit.Sdk;

namespace Tableau.Migration.Tests.Unit.Api.Permissions
{
    public class DefaultPermissionsApiClientTests
    {
        public abstract class DefaultPermissionsApiClientTest : AutoFixtureTestBase
        {
            internal const string CustomContentTypeUrlSegment = "custom";

            internal readonly Mock<IPermissionsApiClientFactory> MockPermissionsClientFactory = new();

            protected readonly DefaultPermissionsContentTypeOptions Options;

            protected readonly Dictionary<string, Mock<IPermissionsApiClient>> MockPermissionsClients;

            internal readonly DefaultPermissionsApiClient DefaultPermissionsClient;

            public DefaultPermissionsApiClientTest()
            {
                Options = new DefaultPermissionsContentTypeOptions(new[] { CustomContentTypeUrlSegment });

                MockPermissionsClients = Options.UrlSegments
                    .ToDictionary(s => s, _ => new Mock<IPermissionsApiClient>(), StringComparer.OrdinalIgnoreCase);

                MockPermissionsClientFactory
                    .Setup(f => f.Create(It.IsAny<IPermissionsUriBuilder>()))
                    .Returns<IPermissionsUriBuilder>(b =>
                        MockPermissionsClients[b.Suffix.Split('/').Last()].Object);

                DefaultPermissionsClient = new DefaultPermissionsApiClient(
                    MockPermissionsClientFactory.Object,
                    Options);
            }
        }

        public class CreatePermissionsAsync : DefaultPermissionsApiClientTest
        {
            [Theory]
            [DefaultPermissionsContentTypeUrlSegmentData]
            public async Task Calls_inner_client(string contentTypeUrlSegment)
            {
                var projectId = Create<Guid>();
                var permissions = Create<IPermissions>();

                var mockResult = new Mock<IResult<IPermissions>>();

                MockPermissionsClients[contentTypeUrlSegment]
                    .Setup(c => c.CreatePermissionsAsync(projectId, permissions, Cancel))
                    .ReturnsAsync(mockResult.Object);

                var result = await DefaultPermissionsClient.CreatePermissionsAsync(contentTypeUrlSegment, projectId, permissions, Cancel);

                Assert.Same(mockResult.Object, result);

                MockPermissionsClients[contentTypeUrlSegment].VerifyAll();
                MockPermissionsClients[contentTypeUrlSegment].VerifyNoOtherCalls();
            }
        }

        public class DeleteAllPermissionsAsync : DefaultPermissionsApiClientTest
        {
            [Theory]
            [DefaultPermissionsContentTypeUrlSegmentData]
            public async Task Calls_inner_client(string contentTypeUrlSegment)
            {
                var projectId = Create<Guid>();
                var permissions = Create<IPermissions>();

                var mockResult = new Mock<IResult>();

                MockPermissionsClients[contentTypeUrlSegment]
                    .Setup(c => c.DeleteAllPermissionsAsync(projectId, permissions, Cancel))
                    .ReturnsAsync(mockResult.Object);

                var result = await DefaultPermissionsClient.DeleteAllPermissionsAsync(contentTypeUrlSegment, projectId, permissions, Cancel);

                Assert.Same(mockResult.Object, result);

                MockPermissionsClients[contentTypeUrlSegment].VerifyAll();
                MockPermissionsClients[contentTypeUrlSegment].VerifyNoOtherCalls();
            }
        }

        public class DeleteCapabilityAsync : DefaultPermissionsApiClientTest
        {
            [Theory]
            [DefaultPermissionsContentTypeUrlSegmentData]
            public async Task Calls_inner_client(string contentTypeUrlSegment)
            {
                var projectId = Create<Guid>();
                var granteeId = Create<Guid>();
                var granteeType = Create<GranteeType>();
                var capability = Create<ICapability>();

                var mockResult = new Mock<IResult>();

                MockPermissionsClients[contentTypeUrlSegment]
                    .Setup(c => c.DeleteCapabilityAsync(projectId, granteeId, granteeType, capability, Cancel))
                    .ReturnsAsync(mockResult.Object);

                var result = await DefaultPermissionsClient.DeleteCapabilityAsync(contentTypeUrlSegment, projectId, granteeId, granteeType, capability, Cancel);

                Assert.Same(mockResult.Object, result);

                MockPermissionsClients[contentTypeUrlSegment].VerifyAll();
                MockPermissionsClients[contentTypeUrlSegment].VerifyNoOtherCalls();
            }
        }

        public class GetAllPermissionsAsync : DefaultPermissionsApiClientTest
        {
            [Fact]
            public async Task Calls_inner_clients()
            {
                var projectId = Create<Guid>();

                var resultsByContentType = new Dictionary<string, IResult<IPermissions>>();

                foreach (var contentTypeUrlSegment in Options.UrlSegments)
                {
                    var contentTypeResult = Result<IPermissions>.Succeeded(Create<IPermissions>());

                    MockPermissionsClients[contentTypeUrlSegment]
                        .Setup(c => c.GetPermissionsAsync(projectId, Cancel))
                        .ReturnsAsync(contentTypeResult);
                }

                var result = await DefaultPermissionsClient.GetAllPermissionsAsync(projectId, Cancel);

                Assert.True(result.Success);

                var granteeCapabilityResults = result.Value.Values.SelectMany(v => v.GranteeCapabilities);

                foreach (var resultByContentType in resultsByContentType.Select(r => r.Value))
                {
                    Assert.True(resultByContentType.Success);

                    foreach (var granteeCapability in resultByContentType.Value.GranteeCapabilities)
                    {
                        Assert.Contains(granteeCapability, granteeCapabilityResults);
                    }
                }

                foreach (var mockClient in MockPermissionsClients.Values)
                {
                    mockClient.VerifyAll();
                    mockClient.VerifyNoOtherCalls();
                }
            }
        }

        public class GetPermissionsAsync : DefaultPermissionsApiClientTest
        {
            [Theory]
            [DefaultPermissionsContentTypeUrlSegmentData]
            public async Task Calls_inner_client(string contentTypeUrlSegment)
            {
                var projectId = Create<Guid>();

                var mockResult = new Mock<IResult<IPermissions>>();

                MockPermissionsClients[contentTypeUrlSegment]
                    .Setup(c => c.GetPermissionsAsync(projectId, Cancel))
                    .ReturnsAsync(mockResult.Object);

                var result = await DefaultPermissionsClient.GetPermissionsAsync(contentTypeUrlSegment, projectId, Cancel);

                Assert.Same(mockResult.Object, result);

                MockPermissionsClients[contentTypeUrlSegment].VerifyAll();
                MockPermissionsClients[contentTypeUrlSegment].VerifyNoOtherCalls();
            }
        }

        private class DefaultPermissionsContentTypeUrlSegmentData : DataAttribute
        {
            public override IEnumerable<object[]?> GetData(MethodInfo testMethod)
            {
                var options = new DefaultPermissionsContentTypeOptions(new[] { DefaultPermissionsApiClientTest.CustomContentTypeUrlSegment });

                foreach (var value in options.UrlSegments)
                {
                    yield return new object[] { value };
                }
            }
        }
    }
}
