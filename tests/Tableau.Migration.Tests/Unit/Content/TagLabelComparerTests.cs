using System;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class TagLabelComparerTests
    {
        public class EqualsMethod : AutoFixtureTestBase
        {
            [Fact]
            public void LabelsEqual()
            {
                var label = Create<string>();

                var x = new Tag(label);
                var y = new Tag(label);

                Assert.True(TagLabelComparer.Instance.Equals(x, y));
            }

            [Fact]
            public void CaseSensitive()
            {
                var x = new Tag("tag");
                var y = new Tag("Tag");

                Assert.False(TagLabelComparer.Instance.Equals(x, y));
            }
        }

        public class GetHashCodeMethod : AutoFixtureTestBase
        {
            [Fact]
            public void LabelsEqual()
            {
                var label = Create<string>();

                var tag = new Tag(label);

                Assert.Equal(label.GetHashCode(StringComparison.Ordinal), TagLabelComparer.Instance.GetHashCode(tag));
            }
        }
    }
}
