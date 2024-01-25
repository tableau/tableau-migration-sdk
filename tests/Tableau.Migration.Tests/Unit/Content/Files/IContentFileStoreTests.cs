using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class IContentFileStoreTests
    {
        public class CreateAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task CreatesAndWritesAsync()
            {
                const string content = "test";
                const string path = "test.txt";
                const string originalFileName = "original.txt";
                var cancel = new CancellationToken();

                var mockFileStore = new Mock<IContentFileStore>
                {
                    CallBase = true
                };

                var handle = new ContentFileHandle(mockFileStore.Object, path, originalFileName);
                mockFileStore.Setup(x => x.Create(path, originalFileName)).Returns(handle);

                var writeStream = new MemoryStream();
                mockFileStore.Setup(x => x.OpenWriteAsync(handle, cancel))
                    .ReturnsAsync(new ContentFileStream(writeStream));

                using var initialStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

                var result = await mockFileStore.Object.CreateAsync(path, originalFileName, initialStream, cancel);

                mockFileStore.Verify(x => x.Create(path, originalFileName), Times.Once);
                mockFileStore.Verify(x => x.OpenWriteAsync(handle, cancel), Times.Once);

                Assert.Equal(content, Encoding.UTF8.GetString(writeStream.ToArray()));
            }

            [Fact]
            public async Task CreatesAndWritesContentItemAsync()
            {
                const string content = "test";
                const string originalFileName = "original.txt";
                var contentItem = new TestContentType();
                var cancel = new CancellationToken();

                var mockFileStore = new Mock<IContentFileStore>
                {
                    CallBase = true
                };

                var handle = new ContentFileHandle(mockFileStore.Object, "generatedPath", originalFileName);
                mockFileStore.Setup(x => x.Create(contentItem, originalFileName)).Returns(handle);

                var writeStream = new MemoryStream();
                mockFileStore.Setup(x => x.OpenWriteAsync(handle, cancel))
                    .ReturnsAsync(new ContentFileStream(writeStream));

                using var initialStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

                var result = await mockFileStore.Object.CreateAsync(contentItem, originalFileName, initialStream, cancel);

                mockFileStore.Verify(x => x.Create(contentItem, originalFileName), Times.Once);
                mockFileStore.Verify(x => x.OpenWriteAsync(handle, cancel), Times.Once);

                Assert.Equal(content, Encoding.UTF8.GetString(writeStream.ToArray()));
            }
        }
    }
}
