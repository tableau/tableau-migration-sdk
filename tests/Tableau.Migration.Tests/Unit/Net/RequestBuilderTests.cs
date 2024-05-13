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
using System.Linq;
using Moq;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class RequestBuilderTests
    {
        public abstract class RequestBuilderTest : AutoFixtureTestBase
        {
            protected readonly Mock<IHttpRequestBuilderFactory> MockHttpRequestBuilderFactory = new();
            protected readonly Mock<IQueryStringBuilder> MockQuery = new();

            protected readonly Uri DefaultUri = new("http://localhost/");

            internal readonly RequestBuilder Builder;

            public RequestBuilderTest()
            {
                MockQuery.SetupGet(q => q.IsEmpty).Returns(true);

                Builder = new(DefaultUri, Create<string>(), MockHttpRequestBuilderFactory.Object, MockQuery.Object);
            }
        }

        public class BuildUri : RequestBuilderTest
        {
            [Fact]
            public void Sets_Path()
            {
                var path = Create<string>();

                var builder = new RequestBuilder(DefaultUri, path, MockHttpRequestBuilderFactory.Object, MockQuery.Object);

                var uri = builder.BuildUri();

                Assert.Equal($"/{path}", uri.AbsolutePath);
            }

            [Fact]
            public void Handles_multiple_paths()
            {
                var paths = CreateMany<string>(2).ToList();

                var builder = new RequestBuilder(DefaultUri, String.Join("/", paths), MockHttpRequestBuilderFactory.Object, MockQuery.Object);

                var uri = builder.BuildUri();

                Assert.Equal($"/{paths[0]}/{paths[1]}", uri.AbsolutePath);
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
        }

        public class WithQuery : RequestBuilderTest
        {
            [Fact]
            public void Adds_query()
            {
                var key = Create<string>();
                var value = Create<string>();

                Builder.WithQuery(key, value);

                MockQuery.Verify(q => q.AddOrUpdate(key, value), Times.Once);
            }
        }
    }
}
