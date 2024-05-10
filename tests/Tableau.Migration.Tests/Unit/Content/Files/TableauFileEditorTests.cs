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

using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IO;
using Moq;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class TableauFileEditorTests
    {
        public class TableauFileEditorTest : AutoFixtureTestBase
        {
            protected const string TEST_ENTRY_FILENAME = "test.twb";
            protected const string TEST_XML = "<workbook></workbook>";

            protected readonly Mock<IContentFileHandle> MockFile;

            protected readonly RecyclableMemoryStream WrittenFileData;
            protected readonly Mock<IContentFileStream> MockWriteFileStream;

            protected readonly IMemoryStreamManager MemoryStreamManager = Migration.MemoryStreamManager.Instance;

            public TableauFileEditorTest()
            {
                Freeze(MemoryStreamManager);
                MockFile = Freeze<Mock<IContentFileHandle>>();

                WrittenFileData = MemoryStreamManager.GetStream();
                MockWriteFileStream = CreateTestFileStream(WrittenFileData);

                MockFile.Setup(x => x.OpenWriteAsync(Cancel))
                    .ReturnsAsync((CancellationToken c) =>
                    {
                        WrittenFileData.Seek(0, SeekOrigin.Begin);
                        WrittenFileData.SetLength(0);
                        return MockWriteFileStream.Object;
                    });
            }

            protected Mock<IContentFileStream> CreateTestFileStream(RecyclableMemoryStream content)
            {
                var mockFileStream = Create<Mock<IContentFileStream>>();
                mockFileStream.SetupGet(x => x.Content).Returns(() => content);
                return mockFileStream;
            }

            protected RecyclableMemoryStream CreateMemoryStream(byte[] data)
            {
                var stream = MemoryStreamManager.GetStream();
                stream.Write(data);
                stream.Seek(0, SeekOrigin.Begin);

                return stream;
            }

            protected RecyclableMemoryStream CreateMemoryStream(string data)
                => CreateMemoryStream(Constants.DefaultEncoding.GetBytes(data));

            protected TableauFileEditor CreateXmlFileEditor(string xml = TEST_XML)
            {
                var stream = CreateMemoryStream(xml);
                return new(MockFile.Object, stream, null, Cancel);
            }

            protected byte[] BundleXmlIntoZipFile(string xml, string entryName = TEST_ENTRY_FILENAME)
            {
                var stream = MemoryStreamManager.GetStream();

                using (var createZip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    var entry = createZip.CreateEntry(entryName);
                    entry.Open().Write(Constants.DefaultEncoding.GetBytes(xml));
                }

                return stream.ToArray();
            }

            protected TableauFileEditor CreateZipArchiveEditor(string xml = TEST_XML)
            {
                var zipData = BundleXmlIntoZipFile(xml);

                var stream = CreateMemoryStream(zipData);
                var zip = new ZipArchive(stream, ZipArchiveMode.Update, leaveOpen: true);

                return new(MockFile.Object, stream, zip, Cancel);
            }
        }

        #region - Ctor -

        public class Ctor : TableauFileEditorTest
        {
            [Fact]
            public async Task InitializeAsync()
            {
                var stream = MemoryStreamManager.GetStream();
                var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true);

                await using var editor = new TableauFileEditor(MockFile.Object, stream, zipArchive, Cancel);

                Assert.Same(stream, editor.Content);
                Assert.Same(zipArchive, editor.Archive);
            }

            [Fact]
            public async Task NullZipArchiveAsync()
            {
                await using var stream = MemoryStreamManager.GetStream();

                await using var editor = new TableauFileEditor(MockFile.Object, stream, null, Cancel);

                Assert.Same(stream, editor.Content);
                Assert.Null(editor.Archive);
            }
        }

        #endregion

        #region - IsXmlFile -

        public class IsXmlFile : AutoFixtureTestBase
        {
            [Theory]
            [InlineData("test.twb", true)]
            [InlineData("test.tds", true)]
            [InlineData("test.hyper", false)]
            [InlineData("test", false)]
            [InlineData("", false)]
            public void DetectsXmlEntriesByFileExtension(string fileName, bool isXml)
            {
                Assert.Equal(isXml, TableauFileEditor.IsXmlFile(fileName));
            }
        }

        #endregion

        #region - GetXmlStream -

        public class GetXmlStream : TableauFileEditorTest
        {
            [Fact]
            public async Task GetsOrCreatesAsync()
            {
                await using var editor = CreateXmlFileEditor();

                var stream1 = editor.GetXmlStream();
                var stream2 = editor.GetXmlStream();

                Assert.Same(stream1, stream2);
            }

            [Fact]
            public async Task NonZipReturnsFileStreamAsync()
            {
                await using var editor = CreateXmlFileEditor();

                var stream = editor.GetXmlStream();

                Assert.Same(editor.Content, stream.XmlContent);
            }

            [Fact]
            public async Task ZipReturnsXmlEntryFileStream()
            {
                await using var editor = CreateZipArchiveEditor();

                var stream = editor.GetXmlStream();

                using var reader = new StreamReader(stream.XmlContent, leaveOpen: true);
                var xml = await reader.ReadToEndAsync();

                Assert.NotSame(editor.Content, stream.XmlContent);
                Assert.Equal(TEST_XML, xml);
            }
        }

        #endregion

        #region - OpenAsync -

        public class OpenAsync : TableauFileEditorTest
        {
            [Fact]
            public async Task OpensXmlFileAsync()
            {
                var dataStream = CreateMemoryStream(TEST_XML);

                var mockFileStream = CreateTestFileStream(dataStream);

                MockFile.SetupGet(x => x.OriginalFileName).Returns(TEST_ENTRY_FILENAME);
                MockFile.Setup(x => x.OpenReadAsync(Cancel))
                    .ReturnsAsync(mockFileStream.Object);

                await using var editor = await TableauFileEditor.OpenAsync(MockFile.Object, MemoryStreamManager, Cancel);

                Assert.NotSame(dataStream, editor.Content);
                Assert.Equal(0, editor.Content.Position); //stream is ready to read.

                Assert.Equal(dataStream.ToArray(), editor.Content.ToArray());

                mockFileStream.Verify(x => x.DisposeAsync(), Times.Once);

                Assert.Null(editor.Archive);
            }

            [Fact]
            public async Task OpensZipFileAsync()
            {
                var data = BundleXmlIntoZipFile(TEST_XML);
                var dataStream = CreateMemoryStream(data);

                var mockFileStream = CreateTestFileStream(dataStream);

                MockFile.SetupGet(x => x.OriginalFileName).Returns("test.twbx");
                MockFile.Setup(x => x.OpenReadAsync(Cancel))
                    .ReturnsAsync(mockFileStream.Object);

                await using var editor = await TableauFileEditor.OpenAsync(MockFile.Object, MemoryStreamManager, Cancel);

                Assert.NotSame(dataStream, editor.Content);
                Assert.Equal(dataStream.ToArray(), editor.Content.ToArray());

                mockFileStream.Verify(x => x.DisposeAsync(), Times.Once);

                Assert.NotNull(editor.Archive);
                Assert.Equal(ZipArchiveMode.Update, editor.Archive.Mode);
            }
        }

        #endregion

        #region - DisposeAsync -

        public class DisposeAsync : TableauFileEditorTest
        {
            [Fact]
            public async Task PersistsXmlAsync()
            {
                const string OUTPUT_XML = "<workbook>changed</workbook>";
                var dataStream = CreateMemoryStream(TEST_XML);

                var mockFileStream = CreateTestFileStream(dataStream);

                MockFile.SetupGet(x => x.OriginalFileName).Returns(TEST_ENTRY_FILENAME);
                MockFile.Setup(x => x.OpenReadAsync(Cancel))
                    .ReturnsAsync(mockFileStream.Object);

                await using (var editor = await TableauFileEditor.OpenAsync(MockFile.Object, MemoryStreamManager, Cancel))
                {
                    await editor.GetXmlStream().XmlContent.WriteAsync(Constants.DefaultEncoding.GetBytes(OUTPUT_XML));
                }

                Assert.Equal(OUTPUT_XML, Constants.DefaultEncoding.GetString(WrittenFileData.ToArray()));

                MockFile.Verify(x => x.OpenWriteAsync(Cancel), Times.Once);
            }
            
            [Fact]
            public async Task PersistsXmlDisposeTwiceNoErrors()
            {
                const string OUTPUT_XML = "<workbook>changed</workbook>";
                var dataStream = CreateMemoryStream(TEST_XML);

                var mockFileStream = CreateTestFileStream(dataStream);

                MockFile.SetupGet(x => x.OriginalFileName).Returns(TEST_ENTRY_FILENAME);
                MockFile.Setup(x => x.OpenReadAsync(Cancel))
                    .ReturnsAsync(mockFileStream.Object);

                var editor = await TableauFileEditor.OpenAsync(MockFile.Object, MemoryStreamManager, Cancel);
                
                await editor.GetXmlStream().XmlContent.WriteAsync(Constants.DefaultEncoding.GetBytes(OUTPUT_XML));
                await editor.DisposeAsync();
                await editor.DisposeAsync();

                Assert.Equal(OUTPUT_XML, Constants.DefaultEncoding.GetString(WrittenFileData.ToArray()));

                MockFile.Verify(x => x.OpenWriteAsync(Cancel), Times.Once);
            }

            [Fact]
            public async Task PersistsZipAsync()
            {
                const string OUTPUT_XML = "<workbook>changed</workbook>";

                var data = BundleXmlIntoZipFile(TEST_XML);
                var dataStream = CreateMemoryStream(data);

                var mockFileStream = CreateTestFileStream(dataStream);

                MockFile.SetupGet(x => x.OriginalFileName).Returns("test.twbx");
                MockFile.Setup(x => x.OpenReadAsync(Cancel))
                    .ReturnsAsync(mockFileStream.Object);

                await using (var editor = await TableauFileEditor.OpenAsync(MockFile.Object, MemoryStreamManager, Cancel))
                {
                    await editor.GetXmlStream().XmlContent.WriteAsync(Constants.DefaultEncoding.GetBytes(OUTPUT_XML));
                }

                var outputZip = new ZipArchive(MemoryStreamManager.GetStream(WrittenFileData.ToArray()), ZipArchiveMode.Read);
                using (var entryStream = outputZip.Entries.Single(e => e.Name == TEST_ENTRY_FILENAME).Open())
                using (var streamReader = new StreamReader(entryStream))
                {
                    Assert.Equal(OUTPUT_XML, streamReader.ReadToEnd());
                }

                MockFile.Verify(x => x.OpenWriteAsync(Cancel), Times.Once);
            }

            [Fact]
            public async Task PersistsZipAsyncDisposeTwiceNoErrors()
            {
                const string OUTPUT_XML = "<workbook>changed</workbook>";

                var data = BundleXmlIntoZipFile(TEST_XML);
                var dataStream = CreateMemoryStream(data);

                var mockFileStream = CreateTestFileStream(dataStream);

                MockFile.SetupGet(x => x.OriginalFileName).Returns("test.twbx");
                MockFile.Setup(x => x.OpenReadAsync(Cancel))
                    .ReturnsAsync(mockFileStream.Object);

                var editor = await TableauFileEditor.OpenAsync(MockFile.Object, MemoryStreamManager, Cancel);
                await editor.GetXmlStream().XmlContent.WriteAsync(Constants.DefaultEncoding.GetBytes(OUTPUT_XML));
                await editor.DisposeAsync();
                await editor.DisposeAsync();

                var outputZip = new ZipArchive(MemoryStreamManager.GetStream(WrittenFileData.ToArray()), ZipArchiveMode.Read);
                using (var entryStream = outputZip.Entries.Single(e => e.Name == TEST_ENTRY_FILENAME).Open())
                using (var streamReader = new StreamReader(entryStream))
                {
                    Assert.Equal(OUTPUT_XML, streamReader.ReadToEnd());
                }

                MockFile.Verify(x => x.OpenWriteAsync(Cancel), Times.Once);
            }
        }

        #endregion
    }
}
