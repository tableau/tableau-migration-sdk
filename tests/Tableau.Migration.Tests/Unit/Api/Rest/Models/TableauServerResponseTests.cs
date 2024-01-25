using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class TableauServerResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes_empty_response()
            {
                var xml = "<tsResponse/>";

                var deserialized = Serializer.DeserializeFromXml<TestTableauServerResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
            }

            [Fact]
            public void Deserializes_error()
            {
                var xml = $@"
<tsResponse>
    <error code=""{Create<int>()}"">
        <summary>{Create<string>()}</summary>
        <detail>{Create<string>()}</detail>
    </error>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<TestTableauServerResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.NotNull(deserialized.Error);
            }
        }
    }
}