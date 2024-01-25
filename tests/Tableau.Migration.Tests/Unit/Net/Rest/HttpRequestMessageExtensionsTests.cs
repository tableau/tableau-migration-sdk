using System.Net.Http;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest
{
    public class HttpRequestMessageExtensionsTests
    {
        public abstract class HttpRequestMessageExtensionsTest : AutoFixtureTestBase
        { }

        public class SetRestAuthenticationToken : HttpRequestMessageExtensionsTest
        {
            [Fact]
            public void Overwrites_existing_token()
            {
                var oldToken = Create<string>();
                var newToken = Create<string>();

                var request = new HttpRequestMessage(HttpMethod.Get, TestConstants.LocalhostUri);

                request.Headers.TryAddWithoutValidation(RestHeaders.AuthenticationToken, oldToken);

                request.AssertHeaderExists(RestHeaders.AuthenticationToken);

                request.SetRestAuthenticationToken(newToken);

                request.AssertSingleHeaderValue(RestHeaders.AuthenticationToken, newToken);
            }

            [Fact]
            public void Sets_token()
            {
                var token = Create<string>();

                var request = new HttpRequestMessage(HttpMethod.Get, TestConstants.LocalhostUri);

                request.SetRestAuthenticationToken(token);

                request.AssertSingleHeaderValue(RestHeaders.AuthenticationToken, token);
            }
        }
    }
}
