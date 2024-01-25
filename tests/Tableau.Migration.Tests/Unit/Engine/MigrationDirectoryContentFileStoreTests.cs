using System;
using System.IO;
using System.IO.Abstractions;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine
{
    public class MigrationDirectoryContentFileStoreTests
    {
        public class TestFileStore : MigrationDirectoryContentFileStore
        {
            public TestFileStore(IFileSystem fileSystem, IContentFilePathResolver pathResolver, IConfigReader configReader, IMigrationInput migrationInput)
                : base(fileSystem, pathResolver, configReader, migrationInput)
            { }

            public string PublicBaseStorePath => BaseStorePath;

        }

        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void UsesMigrationSubDirectory()
            {
                var rootPath = Create<string>();

                var config = Freeze<MigrationSdkOptions>();
                config.Files.RootPath = rootPath;

                var migrationId = Guid.NewGuid();
                var mockInput = Freeze<Mock<IMigrationInput>>();
                mockInput.SetupGet(x => x.MigrationId).Returns(migrationId);

                var fs = Create<TestFileStore>();

                Assert.Equal(Path.Combine(rootPath, $"migration-{migrationId:N}"), fs.PublicBaseStorePath);
            }
        }
    }
}
