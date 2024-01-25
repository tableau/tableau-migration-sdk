using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters
{
    public class ContentLocationInPathFilterTests : AutoFixtureTestBase
    {
        public ContentLocationInPathFilterTests() { }


        [Fact]
        public async Task Filter()
        {
            //Setup
            // Create mock data
            var users = CreateMany<ContentMigrationItem<IUser>>().ToImmutableArray();
            var usersCount = users.Length;

            // Choose one of the items and create a filter from it
            var user1 = users.FirstOrDefault();
            Assert.NotNull(user1);
            var filter1 = new ContentLocationInPathFilter<IUser>(user1.SourceItem.Location.Path);


            // Act- Filter with the chosen filter
            var filterResult = await filter1.ExecuteAsync(users, new CancellationToken());

            // Verify - The filter returns only the item that matches the filter.
            Assert.NotNull(filterResult);
            Assert.Single(filterResult);
            Assert.Same(user1, filterResult.First());
            Assert.Equal(usersCount, users.Length);
        }
    }
}
