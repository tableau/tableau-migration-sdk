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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpContentRequestBuilderTests
    {
        private class TestRequestObject
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public override bool Equals(object? obj)
            {
                return obj is TestRequestObject testObject && Id.Equals(testObject.Id);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Id);
            }
        }

        internal interface ITestHttpContentRequestBuilder : IHttpContentRequestBuilder<ITestHttpContentRequestBuilder>
        { }

        internal class TestHttpContentRequestBuilder : HttpContentRequestBuilder<TestHttpContentRequestBuilder, ITestHttpContentRequestBuilder>, ITestHttpContentRequestBuilder
        {
            internal override HttpMethod Method { get; } = HttpMethod.Post;

            public TestHttpContentRequestBuilder(Uri uri, IHttpClient httpClient, IHttpContentSerializer serializer)
                : base(uri, httpClient, serializer)
            { }
        }

        public abstract class HttpContentRequestBuilderTest : AutoFixtureTestBase
        {
            protected readonly Mock<IHttpClient> MockHttpClient = new();
            protected readonly Mock<IHttpContentSerializer> MockSerializer = new();

            protected readonly HttpContent DefaultContent = new MultipartContent();

            internal readonly TestHttpContentRequestBuilder Builder;

            public HttpContentRequestBuilderTest()
            {
                Builder = new(TestConstants.LocalhostUri, MockHttpClient.Object, MockSerializer.Object);
            }
        }

        public class WithContent : HttpContentRequestBuilderTest
        {
            [Fact]
            public void Sets_content()
            {
                var builder = Builder.WithContent(DefaultContent);

                Assert.IsType<TestHttpContentRequestBuilder>(builder);
                Assert.Same(DefaultContent, builder.Request.Content);
            }
        }

        public class WithContent_and_content_type_and_serializer : HttpContentRequestBuilderTest
        {
            [Fact]
            public void Sets_content()
            {
                var content = new TestRequestObject();

                var serialized = new StringContent("test");

                var contentType = new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Plain);

                MockSerializer.Setup(s => s.Serialize(content, contentType)).Returns(serialized);

                var builder = Builder.WithContent(content, contentType);

                Assert.IsType<TestHttpContentRequestBuilder>(builder);
                Assert.Contains(contentType, builder.Request.Headers.Accept);
                Assert.Same(serialized, builder.Request.Content);

                MockSerializer.Verify(s => s.Serialize(content, contentType), Times.Once);
            }
        }

        public class WithJsonContent : HttpContentRequestBuilderTest
        {
            [Fact]
            public void Sets_content()
            {
                var content = new TestRequestObject();

                var serialized = new StringContent("test");

                MockSerializer.Setup(s => s.Serialize(content, MediaTypes.Json)).Returns(serialized);

                var builder = Builder.WithJsonContent(content);

                Assert.IsType<TestHttpContentRequestBuilder>(builder);
                Assert.Contains(MediaTypes.Json, builder.Request.Headers.Accept);
                Assert.Same(serialized, builder.Request.Content);

                MockSerializer.Verify(s => s.Serialize(content, MediaTypes.Json), Times.Once);
            }

            [Fact]
            public async Task Sets_string_content()
            {
                var content = @"{ ""id"": ""1234"" }";

                var builder = Builder.WithJsonContent(content);

                Assert.IsType<TestHttpContentRequestBuilder>(builder);
                Assert.Contains(MediaTypes.Json, builder.Request.Headers.Accept);

                var stringContent = Assert.IsType<StringContent>(builder.Request.Content);

                Assert.Equal(content, await stringContent.ReadAsStringAsync());
            }
        }

        public class WithXmlContent : HttpContentRequestBuilderTest
        {
            [Fact]
            public void Sets_content()
            {
                var content = new TestRequestObject();

                var serialized = new StringContent("test");

                MockSerializer.Setup(s => s.Serialize(content, MediaTypes.Xml)).Returns(serialized);

                var builder = Builder.WithXmlContent(content);

                Assert.IsType<TestHttpContentRequestBuilder>(builder);
                Assert.Contains(MediaTypes.Xml, builder.Request.Headers.Accept);
                Assert.Same(serialized, builder.Request.Content);

                MockSerializer.Verify(s => s.Serialize(content, MediaTypes.Xml), Times.Once);
            }

            [Fact]
            public async Task Sets_string_content()
            {
                var content = @"<xml><test/></xml>";

                var builder = Builder.WithXmlContent(content);

                Assert.IsType<TestHttpContentRequestBuilder>(builder);
                Assert.Contains(MediaTypes.Xml, builder.Request.Headers.Accept);

                var stringContent = Assert.IsType<StringContent>(builder.Request.Content);

                Assert.Equal(content, await stringContent.ReadAsStringAsync());
            }
        }
    }
}
