using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Tests.Unit.Api;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class TableauSerializerTests
    {
        public class DeserializeAsync : SerializationTestBase
        {
            [Fact]
            public void Deserializes_xml()
            {
                var error = Create<Error>();

                var xml = $@"
<tsResponse>
    <error code=""{error.Code}"">
        <summary>{error.Summary}</summary>
        <detail>{error.Detail}</detail>
    </error>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<TestTableauServerResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.NotNull(deserialized.Error);

                Assert.Equal(error.Code, deserialized.Error.Code);
                Assert.Equal(error.Summary, deserialized.Error.Summary);
                Assert.Equal(error.Detail, deserialized.Error.Detail);
            }

            [Fact]
            public void Deserializes_json()
            {
                var error = Create<Error>();

                var json = $@"
{{
    ""error"": {{
        ""summary"": ""{error.Summary}"",
        ""detail"": ""{error.Detail}"",
        ""code"": ""{error.Code}""
    }}
}}";

                var deserialized = Serializer.DeserializeFromJson<TestTableauServerResponse>(json);

                Assert.NotNull(deserialized);
                Assert.NotNull(deserialized.Error);

                Assert.Equal(error.Code, deserialized.Error.Code);
                Assert.Equal(error.Summary, deserialized.Error.Summary);
                Assert.Equal(error.Detail, deserialized.Error.Detail);
            }
        }
    }
}
