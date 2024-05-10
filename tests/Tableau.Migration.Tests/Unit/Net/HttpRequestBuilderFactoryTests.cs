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
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpRequestBuilderFactoryTests
    {
        public abstract class HttpRequestBuilderFactoryTest : AutoFixtureTestBase
        {
            protected readonly Mock<IHttpClient> MockHttpClient = new();
            protected readonly Mock<IHttpContentSerializer> MockSerializer = new();
            protected readonly Mock<IRequestBuilder> MockUriBuilder = new();

            internal readonly HttpRequestBuilderFactory Factory;

            public HttpRequestBuilderFactoryTest()
            {
                Factory = new HttpRequestBuilderFactory(MockHttpClient.Object, MockSerializer.Object);
            }
        }

        public class CreateDeleteRequest : HttpRequestBuilderFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var builder = Factory.CreateDeleteRequest(Create<Uri>());
                Assert.IsType<HttpDeleteRequestBuilder>(builder);
            }
        }

        public class CreateGetRequest : HttpRequestBuilderFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var builder = Factory.CreateGetRequest(Create<Uri>());
                Assert.IsType<HttpGetRequestBuilder>(builder);
            }
        }

        public class CreatePatchRequest : HttpRequestBuilderFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var builder = Factory.CreatePatchRequest(Create<Uri>());
                Assert.IsType<HttpPatchRequestBuilder>(builder);
            }
        }

        public class CreatePostRequest : HttpRequestBuilderFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var builder = Factory.CreatePostRequest(Create<Uri>());
                Assert.IsType<HttpPostRequestBuilder>(builder);
            }
        }

        public class CreatePutRequest : HttpRequestBuilderFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var builder = Factory.CreatePutRequest(Create<Uri>());
                Assert.IsType<HttpPutRequestBuilder>(builder);
            }
        }
    }
}
