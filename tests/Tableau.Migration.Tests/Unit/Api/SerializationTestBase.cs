using System.Xml.Linq;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public abstract class SerializationTestBase : AutoFixtureTestBase
    {
        internal readonly ITableauSerializer Serializer = TableauSerializer.Instance;

        internal static void AssertXmlEqual(string expectedXml, string actualXml)
        {
            Assert.True(XNode.DeepEquals(XElement.Parse(expectedXml), XElement.Parse(actualXml)),
                $"{actualXml}\n does not equal \n{expectedXml}");
        }

        protected static void AssertNotNullOrEmpty(string? value)
        {
            Assert.NotNull(value);
            Assert.NotEmpty(value);
        }

        protected static void AssertNotDefault<T>(T value)
        {
            Assert.NotEqual(default(T), value);
        }
    }
}
