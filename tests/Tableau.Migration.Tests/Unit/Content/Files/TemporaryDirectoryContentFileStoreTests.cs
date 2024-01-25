using System.IO;
using System.IO.Abstractions;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class TemporaryDirectoryContentFileStoreTests
    {
        public class TestFileStore : TemporaryDirectoryContentFileStore
        {
            public string PublicBaseStorePath => BaseStorePath;

            public TestFileStore(IFileSystem fileSystem, IContentFilePathResolver pathResolver, IConfigReader configReader)
                : base(fileSystem, pathResolver, configReader)
            { }
        }

        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void MakesRandomSubDirectory()
            {
                var rootPath = Create<string>();

                var config = Freeze<MigrationSdkOptions>();
                config.Files.RootPath = rootPath;

                var fs = Create<TestFileStore>();

                var subDir = Path.GetRelativePath(rootPath, fs.PublicBaseStorePath);
                Assert.NotEmpty(subDir);
            }
        }
    }
}
