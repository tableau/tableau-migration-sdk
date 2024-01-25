using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Migrators.Batch;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators.Batch
{
    public class ItemPublishContentBatchMigratorTests
    {
        public class MigratePreparedItemAsync : ParallelContentBatchMigratorBatchTestBase<TestContentType, TestPublishType>
        {
            private readonly Mock<IDestinationEndpoint> _mockDestination;
            private readonly ItemPublishContentBatchMigrator<TestContentType, TestPublishType> _migrator;

            public MigratePreparedItemAsync()
            {
                _mockDestination = Freeze<Mock<IDestinationEndpoint>>();
                _mockDestination.Setup(x => x.PublishAsync<TestPublishType, TestContentType>(It.IsAny<TestPublishType>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => Result<TestContentType>.Succeeded(Create<TestContentType>()));

                _migrator = Create<ItemPublishContentBatchMigrator<TestContentType, TestPublishType>>();
            }

            [Fact]
            public async Task PublishesEachItemAsync()
            {
                var result = await _migrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();

                _mockDestination.Verify(x => x.PublishAsync<TestPublishType, TestContentType>(It.IsAny<TestPublishType>(), It.IsAny<CancellationToken>()), Times.Exactly(Items.Length));

                Assert.All(MockManifestEntries, e => e.Verify(x => x.DestinationFound(It.IsAny<IContentReference>()), Times.Once));
                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetMigrated(), Times.Once));
            }

            [Fact]
            public async Task PublishingFailsAsync()
            {
                var errors = new Exception[] { new(), new() };
                _mockDestination.Setup(x => x.PublishAsync<TestPublishType, TestContentType>(It.IsAny<TestPublishType>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => Result<TestContentType>.Failed(errors));

                var result = await _migrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();

                _mockDestination.Verify(x => x.PublishAsync<TestPublishType, TestContentType>(It.IsAny<TestPublishType>(), It.IsAny<CancellationToken>()), Times.Exactly(Items.Length));

                Assert.All(MockManifestEntries, e => e.Verify(x => x.DestinationFound(It.IsAny<IContentReference>()), Times.Never));
                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetMigrated(), Times.Never));
                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetFailed((IEnumerable<Exception>)errors), Times.Once));
            }
        }
    }
}
