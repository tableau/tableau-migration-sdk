using System.Collections.Immutable;
using System.Xml.Linq;
using Tableau.Migration.Content.Files.Xml;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files.Xml
{
    public class XElementExtensionsTests
    {
        public class GetFeatureFlaggedAttributes
        {
            [Fact]
            public void FFSAware()
            {
                var el = XElement.Parse("<el _.fcp.Flag.true...test='yes' _.fcp.Flag.false...test='no' test='maybe' />");

                var testAttrs = el.GetFeatureFlaggedAttributes("test")
                    .ToImmutableArray();

                Assert.Equal(3, testAttrs.Length);
            }
        }
    }
}
