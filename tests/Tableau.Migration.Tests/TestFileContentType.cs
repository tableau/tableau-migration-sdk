using System;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Tests.Unit.Content.Files;

namespace Tableau.Migration.Tests
{
    public class TestFileContentType : TestPublishType, IFileContent
    {
        public bool IsDisposed { get; private set; }

        public TestFileContentType()
            : this(new MockXmlFileHandle().Object)
        { }

        public TestFileContentType(IContentFileHandle file)
        {
            File = file;
        }

        public IContentFileHandle File { get; set; }

        public async ValueTask DisposeAsync()
        {
            await File.DisposeAsync();
            IsDisposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
