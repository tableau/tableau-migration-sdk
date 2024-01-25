using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters
{
    public class ContentFilterBaseTests
    {
        public class TestFilter : ContentFilterBase<TestContentType>
        {
            public bool PublicDisabled
            {
                get => base.Disabled;
                set => base.Disabled = value;
            }

            public Func<ContentMigrationItem<TestContentType>, bool> FilterCallback { get; set; } = i => true;

            public override bool ShouldMigrate(ContentMigrationItem<TestContentType> item)
                => FilterCallback(item);
        }

        public class ExecuteAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task NoAllocationOnDisabledAsync()
            {
                var filter = new TestFilter
                {
                    PublicDisabled = true
                };

                var allItems = CreateMany<ContentMigrationItem<TestContentType>>();

                var results = await filter.ExecuteAsync(allItems, Cancel);

                Assert.Same(allItems, results);
            }

            [Fact]
            public async Task AppliesFilterAsync()
            {
                var allItems = CreateMany<ContentMigrationItem<TestContentType>>().ToImmutableList();
                var filter = new TestFilter
                {
                    FilterCallback = i => allItems.IndexOf(i) % 2 == 0
                };

                var results = await filter.ExecuteAsync(allItems, Cancel);

                Assert.NotSame(allItems, results);
                Assert.Equal(allItems.Where(filter.FilterCallback), results);
            }
        }
    }
}
