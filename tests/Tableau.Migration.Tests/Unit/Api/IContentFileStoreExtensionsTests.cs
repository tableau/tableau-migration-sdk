using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Tests.Unit.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class IContentFileStoreExtensionsTests
    {
        public class CreateAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task CreatesFromFileDownloadAsync()
            {
                var cancel = new CancellationToken();
                var fs = new MemoryContentFileStore();

                var fileText = "text";
                await using (var fileDownload = new FileDownload("fileName", new MemoryStream(Encoding.UTF8.GetBytes(fileText))))
                {
                    var file = await fs.CreateAsync(new object(), fileDownload, cancel);

                    Assert.Equal(fileDownload.Filename, file.OriginalFileName);

                    await using (var readStream = await file.OpenReadAsync(cancel))
                    using (var reader = new StreamReader(readStream.Content, Encoding.UTF8))
                    {
                        Assert.Equal(fileText, reader.ReadToEnd());
                    }
                }
            }
        }
    }
}
