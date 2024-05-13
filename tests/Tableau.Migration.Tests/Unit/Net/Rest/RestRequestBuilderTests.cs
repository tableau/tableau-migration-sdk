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
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Net.Rest.Fields;
using Tableau.Migration.Net.Rest.Filtering;
using Tableau.Migration.Net.Rest.Paging;
using Tableau.Migration.Net.Rest.Sorting;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest
{
    public class RestRequestBuilderTests
    {
        public abstract class RestRequestBuilderTest : AutoFixtureTestBase
        {
            protected readonly Mock<IHttpClient> MockHttpClient = new();
            internal readonly HttpRequestBuilderFactory HttpRequestBuilderFactory;

            protected readonly Mock<IQueryStringBuilder> MockQuery = new();
            protected readonly Mock<IFieldBuilder> MockFields = new();
            protected readonly Mock<IFilterBuilder> MockFilters = new();
            protected readonly Mock<ISortBuilder> MockSorts = new();
            protected readonly Mock<IPageBuilder> MockPaging = new();

            protected readonly Uri DefaultUri = TestConstants.LocalhostUri;
            protected readonly string DefaultPath;

            internal readonly RestRequestBuilder Builder;

            public RestRequestBuilderTest()
            {
                MockQuery.SetupGet(q => q.IsEmpty).Returns(true);

                DefaultPath = Create<string>();

                HttpRequestBuilderFactory = new(MockHttpClient.Object, HttpContentSerializer.Instance);

                Builder = new(
                    DefaultUri,
                    DefaultPath,
                    HttpRequestBuilderFactory,
                    MockQuery.Object,
                    MockFields.Object,
                    MockFilters.Object,
                    MockSorts.Object,
                    MockPaging.Object);
            }
        }

        public class Build : RestRequestBuilderTest
        {
            [Fact]
            public void Sets_ApiVersion()
            {
                var version = Create<string>();

                Builder.WithApiVersion(version);

                var uri = Builder.BuildUri();

                Assert.Equal($"/api/{version}/{DefaultPath}", uri.AbsolutePath);
            }

            [Fact]
            public void Uses_path_ApiVersion()
            {
                var version1 = Create<string>();
                var version2 = Create<string>();

                var builder = new RestRequestBuilder(DefaultUri, $"/api/{version1}", HttpRequestBuilderFactory);

                builder.WithApiVersion(version2);

                var uri = builder.BuildUri();

                Assert.Equal($"/api/{version1}", uri.AbsolutePath);
            }

            [Fact]
            public void Sets_SiteId()
            {
                var siteId = Create<Guid>();

                Builder.WithSiteId(siteId);

                var uri = Builder.BuildUri();

                Assert.Equal($"/sites/{siteId.ToUrlSegment()}/{DefaultPath}", uri.AbsolutePath);
            }

            [Fact]
            public void Uses_path_SiteId()
            {
                var siteId1 = Create<Guid>();
                var siteId2 = Create<Guid>();

                var builder = new RestRequestBuilder(DefaultUri, $"/sites/{siteId1}", HttpRequestBuilderFactory);

                builder.WithSiteId(siteId2);

                var uri = builder.BuildUri();

                Assert.Equal($"/sites/{siteId1}", uri.AbsolutePath);
            }

            [Fact]
            public void Builds_path_in_correct_order()
            {
                var version = Create<string>();
                var siteId = Create<Guid>();

                Builder.WithSiteId(siteId);
                Builder.WithApiVersion(version);

                var uri = Builder.BuildUri();

                Assert.Equal($"/api/{version}/sites/{siteId}/{DefaultPath}", uri.AbsolutePath);
            }

            [Fact]
            public void Sets_fields()
            {
                var uri = Builder.BuildUri();

                MockFields.Verify(s => s.AppendQueryString(MockQuery.Object), Times.Once);
            }

            [Fact]
            public void Sets_filter()
            {
                var uri = Builder.BuildUri();

                MockFilters.Verify(s => s.AppendQueryString(MockQuery.Object), Times.Once);
            }

            [Fact]
            public void Sets_sort()
            {
                var uri = Builder.BuildUri();

                MockSorts.Verify(s => s.AppendQueryString(MockQuery.Object), Times.Once);
            }

            [Fact]
            public void Sets_page()
            {
                var uri = Builder.BuildUri();

                MockPaging.Verify(s => s.AppendQueryString(MockQuery.Object), Times.Once);
            }

            [Fact]
            public void Sets_query()
            {
                var query = $"query={Create<string>()}";

                MockQuery.SetupGet(q => q.IsEmpty).Returns(false);
                MockQuery.Setup(q => q.Build()).Returns(query);

                var uri = Builder.BuildUri();

                MockQuery.Verify(s => s.Build(), Times.Once);

                Assert.Equal($"?{query}", uri.Query);
            }


            public class ForDeleteRequest : RestRequestBuilderTest
            {
                [Fact]
                public void Creates()
                {
                    var builder = Builder.ForDeleteRequest();
                    Assert.IsType<HttpDeleteRequestBuilder>(builder);
                }
            }

            public class ForGetRequest : RestRequestBuilderTest
            {
                [Fact]
                public void Creates()
                {
                    var builder = Builder.ForGetRequest();
                    Assert.IsType<HttpGetRequestBuilder>(builder);
                }
            }

            public class ForPatchRequest : RestRequestBuilderTest
            {
                [Fact]
                public void Creates()
                {
                    var builder = Builder.ForPatchRequest();
                    Assert.IsType<HttpPatchRequestBuilder>(builder);
                }
            }

            public class ForPostRequest : RestRequestBuilderTest
            {
                [Fact]
                public void Creates()
                {
                    var builder = Builder.ForPostRequest();
                    Assert.IsType<HttpPostRequestBuilder>(builder);
                }
            }

            public class ForPutRequest : RestRequestBuilderTest
            {
                [Fact]
                public void Creates()
                {
                    var builder = Builder.ForPutRequest();
                    Assert.IsType<HttpPutRequestBuilder>(builder);
                }
            }
        }
    }
}
