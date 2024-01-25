using Tableau.Migration.TestComponents.JsonConverters.JsonObjects;

namespace Tableau.Migration.TestComponents.Tests.JsonConverter.JsonObjects
{
    public class TestJsonContentReference : AutoFixtureTestBase
    {
        [Fact]
        public void AsContentReferenceStub()
        {
            var input = Create<JsonContentReference>();

            var output = input.AsContentReferenceStub();

            Assert.NotNull(output);

            Assert.Equal(Guid.Parse(input.Id!), output.Id);
            Assert.Equal(input.ContentUrl, output.ContentUrl);
            Assert.Equal(input.Location!.AsContentLocation(), output.Location);
            Assert.Equal(input.Name, output.Name);
        }

        [Fact]
        public void BadDeserialization_NullId()
        {
            var input = Create<JsonContentReference>();
            input.Id = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentReferenceStub());
        }

        [Fact]
        public void BadDeserialization_NullContentUrl()
        {
            var input = Create<JsonContentReference>();
            input.ContentUrl = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentReferenceStub());
        }

        [Fact]
        public void BadDeserialization_NullLocation()
        {
            var input = Create<JsonContentReference>();
            input.Location = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentReferenceStub());
        }

        [Fact]
        public void BadDeserialization_NullName()
        {
            var input = Create<JsonContentReference>();
            input.Name = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentReferenceStub());
        }
    }
}
