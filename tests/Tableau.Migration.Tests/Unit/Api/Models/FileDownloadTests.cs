using System.IO;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Models;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class FileDownloadTests
    {
        public class DisposeAsync
        {
            [Fact]
            public async Task DisposesStreamAsync()
            {
                var mockStream = new Mock<Stream>();

                var d = new FileDownload(null, mockStream.Object);

                await using (d)
                { }

                mockStream.Verify(x => x.DisposeAsync(), Times.Once);
            }
        }
    }
}
