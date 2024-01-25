using System.Threading.Tasks;
using Tableau.Migration.Api;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class AuthenticationTokenProviderTests
    {
        public abstract class AuthenticationTokenProviderTest : AutoFixtureTestBase
        {
            internal readonly AuthenticationTokenProvider Provider = new();
        }

        public class RequestRefreshAsync : AuthenticationTokenProviderTest
        {
            [Fact]
            public async Task Handles_null_event()
            {
                // Does not throw
                await Provider.RequestRefreshAsync(default);
            }

            [Fact]
            public async Task Calls_refresh()
            {
                var count = 0;

                Provider.RefreshRequestedAsync += _ =>
                {
                    count++;
                    return Task.CompletedTask;
                };

                await Provider.RequestRefreshAsync(default);

                Assert.Equal(1, count);
            }
        }

        public class Set : AuthenticationTokenProviderTest
        {
            [Fact]
            public void Sets_token()
            {
                var token = Create<string>();

                Provider.Set(token);

                Assert.Equal(token, Provider.Token);
            }
        }

        public class Clear : AuthenticationTokenProviderTest
        {
            [Fact]
            public void Clears_token()
            {
                var token = Create<string>();

                Provider.Set(token);

                Assert.Equal(token, Provider.Token);

                Provider.Clear();

                Assert.Null(Provider.Token);
            }
        }
    }
}
