using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class SignInRequestTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes()
            {
                var request = AutoFixture.Create<SignInRequest>();

                Assert.NotNull(request.Credentials);
                Assert.NotNull(request.Credentials.Site);

                var serialized = Serializer.SerializeToXml(request);

                Assert.NotNull(serialized);

                var expected = $@"
<tsRequest>
    <credentials personalAccessTokenName=""{request.Credentials.PersonalAccessTokenName}"" personalAccessTokenSecret=""{request.Credentials.PersonalAccessTokenSecret}"">
	    <site contentUrl=""{request.Credentials.Site.ContentUrl}"" />
    </credentials>
</tsRequest>";

                AssertXmlEqual(expected, serialized);
            }
        }
    }
}