using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class EncryptedFileHandleTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void CopiesInnerValues()
            {
                var mockStore = Freeze<Mock<IContentFileStore>>();
                var mockInner = Freeze<Mock<ContentFileHandle>>();

                var handle = new EncryptedFileHandle(mockStore.Object, mockInner.Object);
                Assert.Equal(mockInner.Object.Path, handle.Path);
                Assert.Equal(mockInner.Object.OriginalFileName, handle.OriginalFileName);
            }
        }

        public class DisposeAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task DisposesInnerStreamAsync()
            {
                var mockStore = Freeze<Mock<IContentFileStore>>();
                var mockInner = Freeze<Mock<ContentFileHandle>>();

                var handle = new EncryptedFileHandle(mockStore.Object, mockInner.Object);

                await handle.DisposeAsync();

                mockInner.Verify(x => x.DisposeAsync(), Times.Once);
            }
        }
    }
}
