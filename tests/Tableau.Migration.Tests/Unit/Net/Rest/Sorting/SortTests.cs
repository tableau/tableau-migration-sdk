using System.ComponentModel;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Net.Rest.Sorting;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest.Sorting
{
    public class SortTests
    {
        public abstract class SortTest : AutoFixtureTestBase
        { }

        public class Ctor
        {
            public class With_Field_and_ListSortDirection : SortTest
            {
                [Fact]
                public void Sets_Field()
                {
                    var field = Create<string>();

                    var sort = new Sort(field, Create<ListSortDirection>());

                    Assert.Equal(field, sort.Field);
                }

                [Fact]
                public void Sets_Direction()
                {
                    var direction = Create<ListSortDirection>();

                    var sort = new Sort(Create<string>(), direction);

                    Assert.Equal(direction, sort.Direction);
                }

                [Fact]
                public void Sets_Expression()
                {
                    var sort = new Sort(Create<string>(), Create<bool>());

                    Assert.Equal($"{sort.Field}:{sort.Direction.GetQueryStringValue()}", sort.Expression);
                }
            }

            public class With_Field_and_bool : SortTest
            {
                [Fact]
                public void Sets_Field()
                {
                    var field = Create<string>();

                    var sort = new Sort(field, Create<bool>());

                    Assert.Equal(field, sort.Field);
                }

                [Fact]
                public void Sets_Ascending_when_true()
                {
                    var sort = new Sort(Create<string>(), true);

                    Assert.Equal(ListSortDirection.Ascending, sort.Direction);
                }

                [Fact]
                public void Sets_Descending_when_false()
                {
                    var sort = new Sort(Create<string>(), false);

                    Assert.Equal(ListSortDirection.Descending, sort.Direction);
                }

                [Fact]
                public void Sets_Expression()
                {
                    var sort = new Sort(Create<string>(), Create<bool>());

                    Assert.Equal($"{sort.Field}:{sort.Direction.GetQueryStringValue()}", sort.Expression);
                }
            }
        }
    }
}
