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
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class TableauFileJsonStreamTests
    {
        public class TableauFileJsonStreamTest : AutoFixtureTestBase
        { }

        public class Ctor : TableauFileJsonStreamTest
        {
            [Fact]
            public void RequiresSeekableStream()
            {
                var stream = new MemoryStream(Array.Empty<byte>(), writable: false);
                Assert.Throws<ArgumentException>(() => new TableauFileJsonStream(stream, Cancel));
            }

            [Fact]
            public async Task InitializesAsync()
            {
                var stream = new MemoryStream();
                await using var jsonStream = new TableauFileJsonStream(stream, Cancel);

                Assert.Same(stream, jsonStream.JsonContent);
            }
        }

        public class GetJsonAsync : TableauFileJsonStreamTest
        {
            [Fact]
            public async Task GetsOrCreatesAsync()
            {
                var stream = new MemoryStream();
                var jsonBytes = Constants.DefaultEncoding.GetBytes("{\"test\": \"value\"}");
                stream.Write(jsonBytes);
                stream.Seek(0, SeekOrigin.Begin);

                await using var jsonStream = new TableauFileJsonStream(stream, Cancel);

                var jsonNode1 = await jsonStream.GetJsonAsync(Cancel);
                var jsonNode2 = await jsonStream.GetJsonAsync(Cancel);

                Assert.Same(jsonNode1, jsonNode2);
            }
        }

        public class DisposeAsync : TableauFileJsonStreamTest
        {
            [Fact]
            public async Task SavesJsonAsync()
            {
                var stream = new MemoryStream();
                var originalJson = "{\"test\": \"original\"}";
                var jsonBytes = Constants.DefaultEncoding.GetBytes(originalJson);
                stream.Write(jsonBytes);
                stream.Seek(0, SeekOrigin.Begin);

                await using (var jsonStream = new TableauFileJsonStream(stream, Cancel, leaveOpen: true))
                {
                    var json = await jsonStream.GetJsonAsync(Cancel);
                    json["test"] = "changed";
                }

                stream.Seek(0, SeekOrigin.Begin);
                using var resultJson = await JsonDocument.ParseAsync(stream);
                Assert.True(resultJson.RootElement.TryGetProperty("test", out var resultProp));
                Assert.Equal("changed", resultProp.GetString());
            }

            [Fact]
            public async Task ClosesStreamAsync()
            {
                var mockStream = new Mock<MemoryStream>() { CallBase = true };
                var stream = mockStream.Object;

                await using (var jsonStream = new TableauFileJsonStream(stream, Cancel, leaveOpen: false))
                { }

                mockStream.Verify(x => x.DisposeAsync(), Times.Once);
            }
        }
    }
}

