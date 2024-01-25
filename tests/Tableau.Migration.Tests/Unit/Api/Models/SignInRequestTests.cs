using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class SignInRequestTests
    {
        public class SignInRequestTest : AutoFixtureTestBase
        { }

        public class Ctor : SignInRequestTest
        {
            [Fact]
            public void Sets_values()
            {
                var tokenName = Create<string>();
                var token = Create<string>();
                var contentUrl = Create<string>();

                var request = new SignInRequest(tokenName, token, contentUrl);

                Assert.NotNull(request.Credentials?.Site);

                Assert.Equal(tokenName, request.Credentials.PersonalAccessTokenName);
                Assert.Equal(token, request.Credentials.PersonalAccessTokenSecret);
                Assert.Equal(contentUrl, request.Credentials.Site.ContentUrl);
            }
        }
    }
}
