using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class TableauFileEditorTests
    {
        public class TableauFileEditorTest : AutoFixtureTestBase
        {
            protected const string TEST_ENTRY_FILENAME = "test.twb";
            protected const string TEST_XML = "<workbook></workbook>";

            protected readonly Mock<IContentFileHandle> MockFile;

            protected readonly MemoryStream WrittenFileData;
            protected readonly Mock<IContentFileStream> MockWriteFileStream;

            public TableauFileEditorTest()
            {
                MockFile = Freeze<Mock<IContentFileHandle>>();

                WrittenFileData = new(); //Default ctor for resizable stream.
                MockWriteFileStream = CreateTestFileStream(WrittenFileData);

                MockFile.Setup(x => x.OpenWriteAsync(Cancel))
                    .ReturnsAsync((CancellationToken c) =>
                    {
                        WrittenFileData.Seek(0, SeekOrigin.Begin);
                        WrittenFileData.SetLength(0);
                        return MockWriteFileStream.Object;
                    });
            }

            protected Mock<IContentFileStream> CreateTestFileStream(MemoryStream content)
            {
                var mockFileStream = Create<Mock<IContentFileStream>>();
                mockFileStream.SetupGet(x => x.Content).Returns(() => content);
                return mockFileStream;
            }

            protected static MemoryStream CreateResizableMemoryStream(byte[] data)
            {
                var stream = new MemoryStream(); //Default ctor for resizable stream.
                stream.Write(data);
                stream.Seek(0, SeekOrigin.Begin);

                return stream;
            }

            protected TableauFileEditor CreateXmlFileEditor(string xml = TEST_XML)
            {
                var stream = TableauFileEditorTest.CreateResizableMemoryStream(Encoding.UTF8.GetBytes(xml));
                return new(MockFile.Object, stream, null, Cancel);
            }

            protected static byte[] BundleXmlIntoZipFile(string xml, string entryName = TEST_ENTRY_FILENAME)
            {
                var stream = new MemoryStream();

                using (var createZip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    var entry = createZip.CreateEntry(entryName);
                    entry.Open().Write(Encoding.UTF8.GetBytes(xml));
                }

                return stream.ToArray();
            }

            protected TableauFileEditor CreateZipArchiveEditor(string xml = TEST_XML)
            {
                var zipData = TableauFileEditorTest.BundleXmlIntoZipFile(xml);

                var stream = TableauFileEditorTest.CreateResizableMemoryStream(zipData);
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
                var stream = new MemoryStream();
                var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true);

                await using var editor = new TableauFileEditor(MockFile.Object, stream, zipArchive, Cancel);

                Assert.Same(stream, editor.Content);
                Assert.Same(zipArchive, editor.Archive);
            }

            [Fact]
            public async Task NullZipArchiveAsync()
            {
                await using var stream = new MemoryStream();

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
                var dataStream = TableauFileEditorTest.CreateResizableMemoryStream(Encoding.UTF8.GetBytes(TEST_XML));

                var mockFileStream = CreateTestFileStream(dataStream);

                MockFile.SetupGet(x => x.OriginalFileName).Returns(TEST_ENTRY_FILENAME);
                MockFile.Setup(x => x.OpenReadAsync(Cancel))
                    .ReturnsAsync(mockFileStream.Object);

                await using var editor = await TableauFileEditor.OpenAsync(MockFile.Object, Cancel);

                Assert.NotSame(dataStream, editor.Content);
                Assert.Equal(dataStream.ToArray(), editor.Content.ToArray());

                mockFileStream.Verify(x => x.DisposeAsync(), Times.Once);

                Assert.Equal(0, editor.Content.Position); //stream is ready to read.

                Assert.Null(editor.Archive);
            }

            [Fact]
            public async Task OpensZipFileAsync()
            {
                var data = TableauFileEditorTest.BundleXmlIntoZipFile(TEST_XML);
                var dataStream = TableauFileEditorTest.CreateResizableMemoryStream(data);

                var mockFileStream = CreateTestFileStream(dataStream);

                MockFile.SetupGet(x => x.OriginalFileName).Returns("test.twbx");
                MockFile.Setup(x => x.OpenReadAsync(Cancel))
                    .ReturnsAsync(mockFileStream.Object);

                await using var editor = await TableauFileEditor.OpenAsync(MockFile.Object, Cancel);

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
                var dataStream = TableauFileEditorTest.CreateResizableMemoryStream(Encoding.UTF8.GetBytes(TEST_XML));

                var mockFileStream = CreateTestFileStream(dataStream);

                MockFile.SetupGet(x => x.OriginalFileName).Returns(TEST_ENTRY_FILENAME);
                MockFile.Setup(x => x.OpenReadAsync(Cancel))
                    .ReturnsAsync(mockFileStream.Object);

                await using (var editor = await TableauFileEditor.OpenAsync(MockFile.Object, Cancel))
                {
                    await editor.GetXmlStream().XmlContent.WriteAsync(Encoding.UTF8.GetBytes(OUTPUT_XML));
                }

                Assert.Equal(OUTPUT_XML, Encoding.UTF8.GetString(WrittenFileData.ToArray()));

                MockFile.Verify(x => x.OpenWriteAsync(Cancel), Times.Once);
            }
            
            [Fact]
            public async Task PersistsXmlDisposeTwiceNoErrors()
            {
                const string OUTPUT_XML = "<workbook>changed</workbook>";
                var dataStream = TableauFileEditorTest.CreateResizableMemoryStream(Encoding.UTF8.GetBytes(TEST_XML));

                var mockFileStream = CreateTestFileStream(dataStream);

                MockFile.SetupGet(x => x.OriginalFileName).Returns(TEST_ENTRY_FILENAME);
                MockFile.Setup(x => x.OpenReadAsync(Cancel))
                    .ReturnsAsync(mockFileStream.Object);

                var editor = await TableauFileEditor.OpenAsync(MockFile.Object, Cancel);
                
                await editor.GetXmlStream().XmlContent.WriteAsync(Encoding.UTF8.GetBytes(OUTPUT_XML));
                await editor.DisposeAsync();
                await editor.DisposeAsync();

                Assert.Equal(OUTPUT_XML, Encoding.UTF8.GetString(WrittenFileData.ToArray()));

                MockFile.Verify(x => x.OpenWriteAsync(Cancel), Times.Once);
            }

            [Fact]
            public async Task PersistsZipAsync()
            {
                const string OUTPUT_XML = "<workbook>changed</workbook>";

                var data = TableauFileEditorTest.BundleXmlIntoZipFile(TEST_XML);
                var dataStream = TableauFileEditorTest.CreateResizableMemoryStream(data);

                var mockFileStream = CreateTestFileStream(dataStream);

                MockFile.SetupGet(x => x.OriginalFileName).Returns("test.twbx");
                MockFile.Setup(x => x.OpenReadAsync(Cancel))
                    .ReturnsAsync(mockFileStream.Object);

                await using (var editor = await TableauFileEditor.OpenAsync(MockFile.Object, Cancel))
                {
                    await editor.GetXmlStream().XmlContent.WriteAsync(Encoding.UTF8.GetBytes(OUTPUT_XML));
                }

                var outputZip = new ZipArchive(new MemoryStream(WrittenFileData.ToArray()), ZipArchiveMode.Read);
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

                var data = TableauFileEditorTest.BundleXmlIntoZipFile(TEST_XML);
                var dataStream = TableauFileEditorTest.CreateResizableMemoryStream(data);

                var mockFileStream = CreateTestFileStream(dataStream);

                MockFile.SetupGet(x => x.OriginalFileName).Returns("test.twbx");
                MockFile.Setup(x => x.OpenReadAsync(Cancel))
                    .ReturnsAsync(mockFileStream.Object);

                var editor = await TableauFileEditor.OpenAsync(MockFile.Object, Cancel);
                await editor.GetXmlStream().XmlContent.WriteAsync(Encoding.UTF8.GetBytes(OUTPUT_XML));
                await editor.DisposeAsync();
                await editor.DisposeAsync();

                var outputZip = new ZipArchive(new MemoryStream(WrittenFileData.ToArray()), ZipArchiveMode.Read);
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
