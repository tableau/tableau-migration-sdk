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
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest
{
    public class RestRequestBuilderFactoryTests
    {
        public abstract class RestRequestBuilderFactoryTest : AutoFixtureTestBase
        {
            protected readonly Mock<IRequestBuilderFactoryInput> MockInput = new();
            protected readonly Mock<IServerSessionProvider> MockSessionProvider = new();
            protected readonly Mock<IHttpRequestBuilderFactory> MockHttpRequestBuilderFactory = new();

            protected readonly Uri DefaultUri = TestConstants.LocalhostUri;
            protected readonly string DefaultPath;

            internal readonly RestRequestBuilderFactory Factory;

            public RestRequestBuilderFactoryTest()
            {
                MockInput.SetupGet(i => i.ServerUri).Returns(DefaultUri);
                MockInput.SetupGet(i => i.IsInitialized).Returns(true);

                DefaultPath = Create<string>();
                Factory = new(MockInput.Object, MockSessionProvider.Object, MockHttpRequestBuilderFactory.Object);
            }

            internal static string? GetApiVersion(RestRequestBuilder builder) => builder.GetFieldValue("_apiVersion") as string;

            internal static string? GetSiteId(RestRequestBuilder builder) => builder.GetFieldValue("_siteId") as string;

            internal static Uri? GetBaseUri(RestRequestBuilder builder) => builder.GetFieldValue(typeof(RequestBuilderBase<RestRequestBuilder>), "_baseUri") as Uri;

            internal static string? GetPath(RestRequestBuilder builder) => builder.GetFieldValue(typeof(RequestBuilderBase<RestRequestBuilder>), "_path") as string;
        }

        public class CreateUri : RestRequestBuilderFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var builder = Factory.CreateUri(DefaultPath);

                Assert.NotNull(builder);
                var restUriBuilder = Assert.IsType<RestRequestBuilder>(builder);

                Assert.Equal(TestConstants.LocalhostUri, GetBaseUri(restUriBuilder));
            }

            [Fact]
            public void Sets_default_Api_version_from_session()
            {
                var version = Create<TableauServerVersion>();

                MockSessionProvider.SetupGet(p => p.Version).Returns(version);

                var builder = Factory.CreateUri(DefaultPath);

                Assert.NotNull(builder);
                var restUriBuilder = Assert.IsType<RestRequestBuilder>(builder);

                Assert.Equal(version.RestApiVersion, GetApiVersion(restUriBuilder));
                Assert.NotEqual(ApiClient.EXPERIMENTAL_API_VERSION, GetApiVersion(restUriBuilder));
            }

            [Fact]
            public void Sets_experimental_api_version()
            {
                var version = Create<TableauServerVersion>();

                MockSessionProvider.SetupGet(p => p.Version).Returns(version);

                var builder = Factory.CreateUri(DefaultPath, useExperimental: true);

                Assert.NotNull(builder);
                var restUriBuilder = Assert.IsType<RestRequestBuilder>(builder);

                Assert.Equal(ApiClient.EXPERIMENTAL_API_VERSION, GetApiVersion(restUriBuilder));
            }

            [Fact]
            public void Sets_default_site_ID_from_session()
            {
                var siteId = Create<Guid>();

                MockSessionProvider.SetupGet(p => p.SiteId).Returns(siteId);

                var builder = Factory.CreateUri(DefaultPath);

                Assert.NotNull(builder);
                var restUriBuilder = Assert.IsType<RestRequestBuilder>(builder);

                Assert.Equal(siteId.ToUrlSegment(), GetSiteId(restUriBuilder));
            }
        }
    }
}
