using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class SignInResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expected = Create<SignInResponse>();

                Assert.NotNull(expected.Item);
                Assert.NotNull(expected.Item.Site);
                Assert.NotNull(expected.Item.User);

                var xml = $@"
<tsResponse xmlns=""http://tableau.com/api""
	xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
	xsi:schemaLocation=""http://tableau.com/api
	http://tableau.com/api/ts-api-3.4.xsd"">
		<credentials token=""{expected.Item.Token}"">
			<site id=""{expected.Item.Site.Id}"" contentUrl=""{expected.Item.Site.ContentUrl}""/>
			<user id=""{expected.Item.User.Id}""/>
		</credentials>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<SignInResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                Assert.NotNull(deserialized.Item);
                Assert.NotNull(deserialized.Item.Site);
                Assert.NotNull(deserialized.Item.User);

                Assert.Equal(expected.Item.Token, deserialized.Item.Token);
                Assert.Equal(expected.Item.User.Id, deserialized.Item.User.Id);
                Assert.Equal(expected.Item.Site.Id, deserialized.Item.Site.Id);
                Assert.Equal(expected.Item.Site.ContentUrl, deserialized.Item.Site.ContentUrl);
            }
        }
    }
}
