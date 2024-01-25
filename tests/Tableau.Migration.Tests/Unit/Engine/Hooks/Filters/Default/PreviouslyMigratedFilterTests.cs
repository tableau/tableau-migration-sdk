using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Options;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public class PreviouslyMigratedFilterTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            private IMigrationManifest? PreviousManifest;

            private PreviouslyMigratedFilterOptions _options = new();

            public ExecuteAsync()
            {
                PreviousManifest = Create<IMigrationManifest>();

                var mockInput = Freeze<Mock<IMigrationInput>>();
                mockInput.SetupGet(x => x.PreviousManifest).Returns(() => PreviousManifest);

                var mockOptionsProvider = Freeze<Mock<IMigrationPlanOptionsProvider<PreviouslyMigratedFilterOptions>>>();
                mockOptionsProvider.Setup(x => x.Get()).Returns(() => _options);
            }

            [Fact]
            public async Task DisabledNoPreviousManifestAsync()
            {
                PreviousManifest = null;

                var allItems = CreateMany<ContentMigrationItem<TestContentType>>().ToImmutableList();

                var filter = Create<PreviouslyMigratedFilter<TestContentType>>();
                var results = await filter.ExecuteAsync(allItems, Cancel);

                Assert.Same(allItems, results);
            }

            [Fact]
            public async Task DisabledManuallyAsync()
            {
                _options = new() { Disabled = true };

                var allItems = CreateMany<ContentMigrationItem<TestContentType>>().ToImmutableList();

                var filter = Create<PreviouslyMigratedFilter<TestContentType>>();
                var results = await filter.ExecuteAsync(allItems, Cancel);

                Assert.Same(allItems, results);
            }

            [Fact]
            public async Task FiltersPreviouslyMigratedAsync()
            {
                var allItems = CreateMany<ContentMigrationItem<TestContentType>>().ToImmutableList();

                var filter = Create<PreviouslyMigratedFilter<TestContentType>>();
                var results = await filter.ExecuteAsync(allItems, Cancel);

                Assert.NotSame(allItems, results);
                Assert.NotNull(results);
                Assert.NotEqual(allItems.Count, results.Count());
                Assert.All(results, i => Assert.False(i.ManifestEntry.HasMigrated));
            }
        }
    }
}
