using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class ContentFileHandleTests
    {
        public class ContentFileHandleTest : AutoFixtureTestBase
        {
            protected readonly Mock<IContentFileStore> MockFileStore;
            protected readonly ContentFileHandle Handle;

            public ContentFileHandleTest()
            {
                MockFileStore = Freeze<Mock<IContentFileStore>>();
                Handle = Create<ContentFileHandle>();
            }
        }

        public class OpenReadAsync : ContentFileHandleTest
        {
            [Fact]
            public async Task OpensFromStoreAsync()
            {
                var storeStream = Freeze<IContentFileStream>();

                var stream = await Handle.OpenReadAsync(Cancel);

                Assert.Same(storeStream, stream);
                MockFileStore.Verify(x => x.OpenReadAsync(Handle, Cancel), Times.Once);
            }
        }

        public class OpenWriteAsync : ContentFileHandleTest
        {
            [Fact]
            public async Task OpensFromStoreAsync()
            {
                var storeStream = Freeze<IContentFileStream>();

                var stream = await Handle.OpenWriteAsync(Cancel);

                Assert.Same(storeStream, stream);
                MockFileStore.Verify(x => x.OpenWriteAsync(Handle, Cancel), Times.Once);
            }
        }

        public class GetXmlStreamAsync : ContentFileHandleTest
        {
            [Fact]
            public async Task OpensFromStoreAsync()
            {
                var editorStream = Freeze<ITableauFileXmlStream>();
                //var storeEditor = Freeze<ITableauFileEditor>();

                var stream = await Handle.GetXmlStreamAsync(Cancel);

                Assert.Same(editorStream, stream);
                MockFileStore.Verify(x => x.GetTableauFileEditorAsync(Handle, Cancel, null), Times.Once);
            }
        }

        public class DisposAsync : ContentFileHandleTest
        {
            [Fact]
            public async Task DeletesFromStoreAsync()
            {
                await Handle.DisposeAsync();

                MockFileStore.Verify(x => x.DeleteAsync(Handle, default), Times.Once);
            }

            [Fact]
            public async Task DisposeTwiceCallDeleteOnce()
            {
                await Handle.DisposeAsync();
                await Handle.DisposeAsync();

                MockFileStore.Verify(x => x.DeleteAsync(Handle, default), Times.Once);
            }
        }
    }
}
