using Moq;
using Tableau.Migration.Config;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators.Batch
{
    public class ParallelContentBatchMigratorBatchTestBase<TContent, TPublish> : ContentBatchMigratorTestBase<TContent, TPublish>
        where TContent : class
        where TPublish : class
    {
        protected readonly Mock<IConfigReader> MockConfigReader;

        protected int TestConcurrency { get; set; } = 3;

        public ParallelContentBatchMigratorBatchTestBase()
        {
            MockConfigReader = Freeze<Mock<IConfigReader>>();
            MockConfigReader.Setup(x => x.Get())
                .Returns(() => new MigrationSdkOptions
                {
                    MigrationParallelism = TestConcurrency
                });
        }
    }
}
