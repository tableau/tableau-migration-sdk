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
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Moq;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class TableauFileXmlStreamTests
    {
        public class TableauFileXmlStreamTest : AutoFixtureTestBase
        { }

        public class Ctor : TableauFileXmlStreamTest
        {
            [Fact]
            public void RequiresSeekableStream()
            {
                var stream = new MemoryStream(Array.Empty<byte>(), writable: false);
                Assert.Throws<ArgumentException>(() => new TableauFileXmlStream(stream, Cancel));
            }

            [Fact]
            public async Task InitializesAsync()
            {
                var stream = new MemoryStream();
                await using var xmlStream = new TableauFileXmlStream(stream, Cancel);

                Assert.Same(stream, xmlStream.XmlContent);
            }
        }

        public class GetXmlAsync : TableauFileXmlStreamTest
        {
            [Fact]
            public async Task GetsOrCreatesAsync()
            {
                var stream = new MemoryStream();
                stream.Write(Constants.DefaultEncoding.GetBytes("<workbook />"));
                stream.Seek(0, SeekOrigin.Begin);

                await using var xmlStream = new TableauFileXmlStream(stream, Cancel);

                var xmlDoc1 = await xmlStream.GetXmlAsync(Cancel);
                var xmlDoc2 = await xmlStream.GetXmlAsync(Cancel);

                Assert.Same(xmlDoc1, xmlDoc2);
            }
        }

        public class DisposeAsync : TableauFileXmlStreamTest
        {
            [Fact]
            public async Task SavesXmlAsync()
            {
                var stream = new MemoryStream();
                stream.Write(Constants.DefaultEncoding.GetBytes("<workbook />"));
                stream.Seek(0, SeekOrigin.Begin);

                await using (var xmlStream = new TableauFileXmlStream(stream, Cancel, leaveOpen: true))
                {
                    var xml = await xmlStream.GetXmlAsync(Cancel);
                    xml.Root!.SetAttributeValue("test", "changed");
                }

                stream.Seek(0, SeekOrigin.Begin);
                var resultXml = await XDocument.LoadAsync(stream, LoadOptions.None, Cancel);
                Assert.Equal("changed", resultXml.Root!.Attribute("test")!.Value);
            }

            [Fact]
            public async Task ClosesStreamAsync()
            {
                var mockStream = new Mock<MemoryStream>() { CallBase = true };
                var stream = mockStream.Object;

                await using (var xmlStream = new TableauFileXmlStream(stream, Cancel, leaveOpen: false))
                { }

                mockStream.Verify(x => x.DisposeAsync(), Times.Once);
            }
        }
    }
}
