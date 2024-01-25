using Tableau.Migration.Config;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Config
{
    public class MigrationSdkOptionsTests
    {
        public class BatchSize
        {
            [Fact]
            public void FallsBackToDefault()
            {
                var opts = new MigrationSdkOptions();
                Assert.Equal(MigrationSdkOptions.Defaults.BATCH_SIZE, opts.BatchSize);
            }

            [Fact]
            public void CustomizedValue()
            {
                var opts = new MigrationSdkOptions
                {
                    BatchSize = 47
                };
                Assert.Equal(47, opts.BatchSize);
            }
        }

        public class MigrationParallelism
        {
            [Fact]
            public void FallsBackToDefault()
            {
                var opts = new MigrationSdkOptions();
                Assert.Equal(MigrationSdkOptions.Defaults.MIGRATION_PARALLELISM, opts.MigrationParallelism);
            }

            [Fact]
            public void CustomizedValue()
            {
                var opts = new MigrationSdkOptions
                {
                    MigrationParallelism = 47
                };
                Assert.Equal(47, opts.MigrationParallelism);
            }
        }
    }
}
