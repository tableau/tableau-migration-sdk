using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Mappings.Default
{
    public class CallbackAuthenticationTypeDomainMappingTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            private readonly CancellationToken _cancel = new();

            [Fact]
            public async Task MapsDomainAsync()
            {
                var callback = (ContentMappingContext<IUsernameContent> ctx, CancellationToken cancel)
                    => Task.FromResult("myDomain");

                var mapper = new CallbackAuthenticationTypeDomainMapping(callback);

                var ctx = Create<ContentMappingContext<IUser>>();
                var result = await mapper.ExecuteAsync(ctx, _cancel);

                Assert.NotNull(result);
                Assert.NotSame(ctx, result);
                Assert.Same(ctx.ContentItem, result.ContentItem);

                var expectedLoc = ContentLocation.ForUsername("myDomain", ctx.MappedLocation.Name);
                Assert.Equal(expectedLoc, result.MappedLocation);
            }

            [Fact]
            public async Task CallbackReturnsNullAsync()
            {
                var callback = (ContentMappingContext<IUsernameContent> ctx, CancellationToken cancel)
                    => Task.FromResult((string?)null);

                var mapper = new CallbackAuthenticationTypeDomainMapping(callback);

                var ctx = Create<ContentMappingContext<IUser>>();
                var result = await mapper.ExecuteAsync(ctx, _cancel);

                Assert.NotNull(result);
                Assert.Same(ctx, result);
            }
        }
    }
}
