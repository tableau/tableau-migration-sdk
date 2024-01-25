using System.Collections.Immutable;
using System.Threading;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Migrators.Batch;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators.Batch
{
    public class ContentMigrationBatchTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var items = CreateMany<ContentMigrationItem<TestContentType>>().ToImmutableArray();
                var cancel = new CancellationToken();

                var b = new ContentMigrationBatch<TestContentType, TestPublishType>(items, cancel);

                Assert.Equal(items, b.Items);
                Assert.Empty(b.ItemResults);
                Assert.Empty(b.PublishItems);
                Assert.False(b.BatchCancelSource.IsCancellationRequested);
            }

            [Fact]
            public void LinksCancellationToken()
            {
                var items = CreateMany<ContentMigrationItem<TestContentType>>().ToImmutableArray();
                var migrationCancelSource = new CancellationTokenSource();

                var b = new ContentMigrationBatch<TestContentType, TestPublishType>(items, migrationCancelSource.Token);

                migrationCancelSource.Cancel();
                Assert.True(b.BatchCancelSource.IsCancellationRequested);
            }

            [Fact]
            public void CancelingBatchDoesNotCancelMigration()
            {
                var items = CreateMany<ContentMigrationItem<TestContentType>>().ToImmutableArray();
                var migrationCancelSource = new CancellationTokenSource();

                var b = new ContentMigrationBatch<TestContentType, TestPublishType>(items, migrationCancelSource.Token);

                b.BatchCancelSource.Cancel();
                Assert.True(b.BatchCancelSource.IsCancellationRequested);
                Assert.False(migrationCancelSource.IsCancellationRequested);
            }
        }
    }
}
