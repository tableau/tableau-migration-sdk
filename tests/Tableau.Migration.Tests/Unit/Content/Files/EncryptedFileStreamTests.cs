using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class EncryptedFileStreamTests
    {
        public class TestStream : CryptoStream
        {
            public TestStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode)
                : base(stream, transform, mode)
            { }

            public bool Disposed { get; private set; }

            protected override void Dispose(bool disposing)
            {
                Disposed = true;
                base.Dispose(disposing);
            }

            public override ValueTask DisposeAsync()
            {
                Disposed = true;
                GC.SuppressFinalize(this);
                return base.DisposeAsync();
            }
        }

        public class DisposeAsync
        {
            [Fact]
            public async Task TaskDisposesAllOwnedItemsAsync()
            {
                var mockInnerStream = new Mock<IContentFileStream>();

                var mockAes = new Mock<IDisposable>();
                var mockTransform = new Mock<ICryptoTransform>();

                var s = new TestStream(new MemoryStream(), mockTransform.Object, CryptoStreamMode.Read);

                var encryptedStream = new EncryptedFileStream(mockInnerStream.Object, mockAes.Object, mockTransform.Object, s);

                await encryptedStream.DisposeAsync();

                Assert.True(s.Disposed);
                mockTransform.Verify(x => x.Dispose(), Times.Once);
                mockAes.Verify(x => x.Dispose(), Times.Once);
                mockInnerStream.Verify(x => x.DisposeAsync(), Times.Once);
            }
        }
    }
}
