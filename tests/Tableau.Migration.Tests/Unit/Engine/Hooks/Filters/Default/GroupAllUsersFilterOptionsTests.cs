using Tableau.Migration.Engine.Hooks.Filters.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public class GroupAllUsersFilterOptionsTests
    {
        public abstract class GroupAllUsersFilterOptionsTest : AutoFixtureTestBase
        { }

        public class Ctor : GroupAllUsersFilterOptionsTest
        {
            [Fact]
            public void Initializes()
            {
                var options = new GroupAllUsersFilterOptions();

                Assert.NotNull(options.AllUsersGroupNames);
                Assert.Empty(options.AllUsersGroupNames);
            }
        }

        public class AllUsersGroupNames : GroupAllUsersFilterOptionsTest
        {
            [Fact]
            public void Gets_values()
            {
                var options = new GroupAllUsersFilterOptions();

                var groupName = Create<string>();

                options.AllUsersGroupNames.Add(groupName);

                Assert.Equal(groupName, Assert.Single(options.AllUsersGroupNames));
            }
        }
    }
}
