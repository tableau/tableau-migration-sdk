using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class CreateProjectResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expected = Create<CreateProjectResponse>();

                Assert.NotNull(expected.Item);

                var xml = $@"
<tsResponse xmlns=""http://tableau.com/api""
	xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
	xsi:schemaLocation=""http://tableau.com/api
	http://tableau.com/api/ts-api-3.4.xsd"">
		<project 
            id=""{expected.Item.Id}""
            parentProjectId=""{expected.Item.ParentProjectId}""
            name=""{expected.Item.Name}""
            description=""{expected.Item.Description}""
            contentPermissions=""{expected.Item.ContentPermissions}"" />
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<CreateProjectResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                Assert.NotNull(deserialized.Item);

                Assert.Equal(expected.Item.Id, deserialized.Item.Id);
                Assert.Equal(expected.Item.ParentProjectId, deserialized.Item.ParentProjectId);
                Assert.Equal(expected.Item.Name, deserialized.Item.Name);
                Assert.Equal(expected.Item.Description, deserialized.Item.Description);
                Assert.Equal(expected.Item.ContentPermissions, deserialized.Item.ContentPermissions);
            }
        }
    }
}
