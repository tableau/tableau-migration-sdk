using System;
using Tableau.Migration.Net.Rest;
using Xunit;


namespace Tableau.Migration.Tests.Unit.Net.Rest
{
    public class GuidExtensionsTests
    {
        public class GuidExtensionTest : AutoFixtureTestBase
        { }

        public class ToUrlSegment : GuidExtensionTest
        {
            [Fact]
            public void Uses_expected_format()
            {
                var guidValue = "e70e0c66-bd02-40e2-9f98-47511047b4a6";
                var guid = Guid.Parse(guidValue);

                Assert.Equal(guidValue, guid.ToUrlSegment());
            }
        }
    }
}
