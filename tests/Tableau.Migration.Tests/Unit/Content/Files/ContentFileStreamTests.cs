using System;
using System.IO;
using System.Threading.Tasks;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class ContentFileStreamTests
    {
        public class TestStream : MemoryStream
        {
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

        public class Ctor
        {
            [Fact]
            public async void Initializes()
            {
                var stream = new MemoryStream();
                await using var fileStream = new ContentFileStream(stream);

                Assert.Same(stream, fileStream.Content);
            }
        }

        public class DisposeAsync
        {
            [Fact]
            public async void DisposesStream()
            {
                var stream = new TestStream();
                var fileStream = new ContentFileStream(stream);

                await fileStream.DisposeAsync();

                Assert.True(stream.Disposed);
            }
        }
    }
}
