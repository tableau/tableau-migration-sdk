using System.Xml.Linq;
using Tableau.Migration.Content.Files.Xml;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files.Xml
{
    public class XFeatureFlagNameTests
    {
        public class Parse
        {
            [Theory]
            [InlineData("_.fcp.FeatureFlag.true...element", true)]
            [InlineData("BADPREFIX_.fcp.FeatureFlag.true...element", false)]
            [InlineData("element", false)]
            public void ParsesHasFeatureFlag(string ffsName, bool expectedValue)
            {
                var fn = XFeatureFlagName.Parse(ffsName);
                Assert.Equal(expectedValue, fn.HasFeatureFlag);
            }

            [Theory]
            [InlineData("_.fcp.FeatureFlag1.true...element", "FeatureFlag1")]
            [InlineData("element", "")]
            public void ParsesFeatureFlagName(string ffsName, string expectedValue)
            {
                var fn = XFeatureFlagName.Parse(ffsName);
                Assert.Equal(expectedValue, fn.FeatureFlagName);
            }

            [Theory]
            [InlineData("_.fcp.FeatureFlag.true...element", true)]
            [InlineData("_.fcp.FeatureFlag.TRUE...element", true)]
            [InlineData("_.fcp.FeatureFlag.True...element", true)]
            [InlineData("_.fcp.FeatureFlag.false...element", false)]
            [InlineData("_.fcp.FeatureFlag.FALSE...element", false)]
            [InlineData("_.fcp.FeatureFlag.False...element", false)]
            [InlineData("element", false)]
            public void ParsesFeatureFlagEnabled(string ffsName, bool expectedValue)
            {
                var fn = XFeatureFlagName.Parse(ffsName);
                Assert.Equal(expectedValue, fn.FeatureFlagEnabled);
            }

            [Theory]
            [InlineData("_.fcp.FeatureFlag.true...element", "element")]
            [InlineData("element", "element")]
            public void ParsesName(string ffsName, string expectedValue)
            {
                var fn = XFeatureFlagName.Parse(ffsName);
                Assert.Equal(expectedValue, fn.Name);
            }

            [Theory]
            [InlineData("", "_.fcp.FeatureFlag.true...element")]
            [InlineData("", "element")]
            [InlineData("ns", "element")]
            [InlineData("ns", "_.fcp.FeatureFlag.true...element")]
            public void FullNameSameAsInput(string nsName, string elName)
            {
                var xName = XName.Get(elName, nsName);
                var fn = XFeatureFlagName.Parse(xName);
                Assert.Same(xName, fn.FullName);
            }
        }
    }
}
