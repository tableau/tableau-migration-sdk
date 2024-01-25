using System.Collections.Immutable;
using System.Xml.Linq;
using Tableau.Migration.Content.Files.Xml;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files.Xml
{
    public class XContainerExtensionsTests
    {
        public class GetFeatureFlaggedDescendants
        {
            [Fact]
            public void GetsDescendants()
            {
                var el = XElement.Parse("<el><test /><container><test /><test /></container></el>");

                var testEls = el.GetFeatureFlaggedDescendants("test")
                    .ToImmutableArray();

                Assert.Equal(3, testEls.Length);
            }

            [Fact]
            public void FFSAware()
            {
                var el = XElement.Parse("<el><_.fcp.Flag.true...test /><_.fcp.Flag.false...test /><test /></el>");

                var testEls = el.GetFeatureFlaggedDescendants("test")
                    .ToImmutableArray();

                Assert.Equal(3, testEls.Length);
            }
        }
    }
}
