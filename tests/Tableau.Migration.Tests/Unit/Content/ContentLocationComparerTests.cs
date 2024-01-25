using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class ContentLocationComparerTests
    {
        public class Instance
        {
            [Fact]
            public void IsSingleton()
            {
                var instance1 = ContentLocationComparer<TestContentType>.Instance;
                var instance2 = ContentLocationComparer<TestContentType>.Instance;

                Assert.Same(instance1, instance2);
            }
        }

        public class Compare : AutoFixtureTestBase
        {
            [Fact]
            public void BothNull()
            {
                TestContentType? a = null, b = null;

                var result = ContentLocationComparer<TestContentType>.Instance.Compare(a, b);

                Assert.Equal(0, result);
            }

            [Fact]
            public void SingleNull()
            {
                TestContentType? a = null, b = Create<TestContentType>();

                var result = ContentLocationComparer<TestContentType>.Instance.Compare(a, b);

                Assert.Equal(-1, result);

                result = ContentLocationComparer<TestContentType>.Instance.Compare(b, a);

                Assert.Equal(1, result);
            }

            [Fact]
            public void CompareByLocation()
            {
                TestContentType a = Create<TestContentType>(), b = Create<TestContentType>();

                var result = ContentLocationComparer<TestContentType>.Instance.Compare(a, b);

                Assert.Equal(a.Location.CompareTo(b.Location), result);
            }
        }
    }
}
