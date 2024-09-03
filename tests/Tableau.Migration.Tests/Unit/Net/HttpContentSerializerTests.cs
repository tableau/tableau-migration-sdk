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
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpContentSerializerTests
    {
        public abstract class HttpContentSerializerTest : AutoFixtureTestBase
        {
            internal readonly Mock<HttpContentSerializer> MockSerializer = new(TableauSerializer.Instance)
            {
                CallBase = true
            };

            public readonly Mock<HttpContent> MockContent = new();
        }

        public class TryDeserializeErrorAsync : HttpContentSerializerTest
        {
            [Fact]
            public async Task Returns_null_when_no_error_found()
            {
                var tsResponse = new EmptyTableauServerResponse();

                Assert.Null(tsResponse.Error);

                var error = await MockSerializer.Object.TryDeserializeErrorAsync(MockContent.Object, Cancel);

                Assert.Null(error);
            }

            [Fact]
            public async Task Returns_error()
            {
                var tsResponse = new EmptyTableauServerResponse(new());

                var content = HttpContentSerializer.Instance.Serialize(tsResponse, MediaTypes.Xml)!;

                MockSerializer.Setup(s => s.DeserializeAsync<EmptyTableauServerResponse>(content, Cancel))
                    .ReturnsAsync(tsResponse);

                var error = await MockSerializer.Object.TryDeserializeErrorAsync(content, Cancel);

                Assert.NotNull(error);
                Assert.Same(tsResponse.Error, error);

                MockSerializer.VerifyAll();
            }

            [Fact]
            public async Task Returns_null_on_deserialization_error()
            {
                var error = await MockSerializer.Object.TryDeserializeErrorAsync(MockContent.Object, Cancel);

                Assert.Null(error);
            }
        }

        public sealed class DeserializeAsync : HttpContentSerializerTest
        {
            [Fact]
            public async Task ThrowsHtmlErrorWithContextAsync()
            {
                const string HTML = "<html><head></head><body><h1>I Have Error Information!</h1></body>";
                var htmlErrorContent = new StringContent(HTML, Encoding.UTF8, MediaTypeNames.Text.Html);

                var ex = await Assert.ThrowsAsync<FormatException>(() => MockSerializer.Object.DeserializeAsync<EmptyTableauServerResponse>(htmlErrorContent, Cancel));
                Assert.Contains(HTML, ex.Message);
            }
        }
    }
}
