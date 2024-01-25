using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Mappings.Default
{
    public class AuthenticationTypeDomainMappingBaseTests
    {
        public class TestAuthenticationTypeDomainMapping : AuthenticationTypeDomainMappingBase
        {
            public int CallCount { get; private set; }

            protected override Task<ContentMappingContext<T>?> ExecuteAsync<T>(ContentMappingContext<T> context, CancellationToken cancel)
            {
                CallCount++;
                return context.ToTask();
            }
        }

        public class AuthenticationTypeDomainMappingBaseTest : AutoFixtureTestBase
        {
            protected readonly TestAuthenticationTypeDomainMapping Mapping;

            public AuthenticationTypeDomainMappingBaseTest()
            {
                Mapping = new();
            }
        }

        public class ExecuteAsync : AuthenticationTypeDomainMappingBaseTest
        {
            [Fact]
            public async Task WithUserAsync()
            {
                var ctx = Create<ContentMappingContext<IUser>>();

                var result = await Mapping.ExecuteAsync(ctx, Cancel);

                Assert.Equal(1, Mapping.CallCount);
                Assert.Same(result, ctx);
            }

            [Fact]
            public async Task WithGroupAsync()
            {
                var ctx = Create<ContentMappingContext<IGroup>>();

                var result = await Mapping.ExecuteAsync(ctx, Cancel);

                Assert.Equal(1, Mapping.CallCount);
                Assert.Same(result, ctx);
            }
        }
    }
}
