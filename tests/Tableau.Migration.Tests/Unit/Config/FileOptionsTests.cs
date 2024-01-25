using System.IO;
using Xunit;

using FileOptions = Tableau.Migration.Config.FileOptions;

namespace Tableau.Migration.Tests.Unit.Config
{
    public class FileOptionsTests
    {
        public class Disabled
        {
            [Fact]
            public void DefaultsToFalse()
            {
                Assert.False(FileOptions.Defaults.DISABLE_FILE_ENCRYPTION);
            }

            [Fact]
            public void FallsBackToDefault()
            {
                var opts = new FileOptions();
                Assert.Equal(FileOptions.Defaults.DISABLE_FILE_ENCRYPTION, opts.DisableFileEncryption);
            }

            [Fact]
            public void CustomizedValue()
            {
                var opts = new FileOptions
                {
                    DisableFileEncryption = true
                };
                Assert.True(opts.DisableFileEncryption);
            }
        }

        public class RootPath
        {
            [Fact]
            public void DefaultsToTempDir()
            {
                Assert.Equal(Path.GetTempPath(), FileOptions.Defaults.ROOT_PATH);
            }

            [Fact]
            public void FallsBackToDefault()
            {
                var opts = new FileOptions();
                Assert.Equal(FileOptions.Defaults.ROOT_PATH, opts.RootPath);
            }

            [Fact]
            public void CustomizedValue()
            {
                const string testPath = @"\\test";
                var opts = new FileOptions
                {
                    RootPath = testPath
                };
                Assert.Equal(testPath, opts.RootPath);
            }
        }

    }
}
