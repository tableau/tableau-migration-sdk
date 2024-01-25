using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class NameOfTests
    {
        public class Build
        {
            [Fact]
            public void Builds()
            {
                var response = new SignInResponse();

                var actual = NameOf.Build(() => response);

                var expected = "response";

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void BuildsChain()
            {
                var response = new SignInResponse();

                var actual = NameOf.Build(() => response.Item!.Site!.ContentUrl);

                var expected = "response.Item.Site.ContentUrl";

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void BuildsChainForValueType()
            {
                var response = new SignInResponse();

                var actual = NameOf.Build(() => response.Item!.Site!.Id);

                var expected = "response.Item.Site.Id";

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void DoesNotThrowOnNonMemberExpression()
            {
                Assert.Equal(string.Empty, NameOf.Build(() => 1 + 1));
            }
        }
    }
}
