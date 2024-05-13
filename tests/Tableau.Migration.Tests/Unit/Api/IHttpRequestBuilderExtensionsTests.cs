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

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class IHttpRequestBuilderExtensionsTests
    {
        public abstract class IHttpRequestBuilderExtensionsTest : AutoFixtureTestBase
        {
            protected readonly Mock<IHttpRequestBuilder> MockRequestBuilder = new();

            protected readonly Mock<IHttpResponseMessage> MockResponse = new();
            protected readonly Mock<HttpHeaders> MockHeaders = new();

            public IHttpRequestBuilderExtensionsTest()
            {
                MockRequestBuilder.Setup(x => x.SendAsync(HttpCompletionOption.ResponseHeadersRead, Cancel))
                    .ReturnsAsync(MockResponse.Object);

                MockResponse.SetupGet(x => x.Headers).Returns(MockHeaders.Object);
            }
        }

        public class DownloadAsync : IHttpRequestBuilderExtensionsTest
        {
            [Fact]
            public async Task SendsAndDownloadsAsync()
            {
                var content = new ByteArrayContent(Constants.DefaultEncoding.GetBytes("test content"));
                MockResponse.Setup(x => x.Content).Returns(content);

                var result = await MockRequestBuilder.Object
                    .DownloadAsync(Cancel);

                MockRequestBuilder.Verify(x => x.SendAsync(HttpCompletionOption.ResponseHeadersRead, Cancel), Times.Once);

                result.AssertSuccess();
            }
        }
    }
}
