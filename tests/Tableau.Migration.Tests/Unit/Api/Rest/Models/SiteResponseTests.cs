using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class SiteResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expected = Create<SiteResponse>();

                Assert.NotNull(expected.Item);

                var xml = $@"
<tsResponse>
    <site id=""{expected.Item.Id}"" name=""{expected.Item.Name}"" contentUrl=""{expected.Item.ContentUrl}"" />
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<SiteResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.NotNull(deserialized.Item);

                Assert.Equal(expected.Item.Id, deserialized.Item.Id);
                Assert.Equal(expected.Item.Name, deserialized.Item.Name);
                Assert.Equal(expected.Item.ContentUrl, deserialized.Item.ContentUrl);
            }
        }
    }
}