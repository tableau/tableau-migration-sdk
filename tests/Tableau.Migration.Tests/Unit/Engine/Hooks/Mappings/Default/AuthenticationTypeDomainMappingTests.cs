using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Tableau.Migration.Engine.Options;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Mappings.Default
{
    public class AuthenticationTypeDomainMappingTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task MapsUserDomainAsync()
            {
                var mockOptions = Create<Mock<IMigrationPlanOptionsProvider<AuthenticationTypeDomainMappingOptions>>>();
                mockOptions.Setup(x => x.Get()).Returns(new AuthenticationTypeDomainMappingOptions
                {
                    UserDomain = "userDomain"
                });

                var mapping = new AuthenticationTypeDomainMapping(mockOptions.Object);

                var ctx = Create<ContentMappingContext<IUser>>();
                var result = await mapping.ExecuteAsync(ctx, new());

                var expectedLoc = ContentLocation.ForUsername("userDomain", ctx.MappedLocation.Name);

                Assert.NotNull(result);
                Assert.NotSame(ctx, result);
                Assert.Same(ctx.ContentItem, result.ContentItem);
                Assert.Equal(expectedLoc, result.MappedLocation);
            }

            [Fact]
            public async Task MapsGroupDomainAsync()
            {
                var mockOptions = Create<Mock<IMigrationPlanOptionsProvider<AuthenticationTypeDomainMappingOptions>>>();
                mockOptions.Setup(x => x.Get()).Returns(new AuthenticationTypeDomainMappingOptions
                {
                    GroupDomain = "groupDomain"
                });

                var mapping = new AuthenticationTypeDomainMapping(mockOptions.Object);

                var ctx = Create<ContentMappingContext<IGroup>>();
                var result = await mapping.ExecuteAsync(ctx, new());

                var expectedLoc = ContentLocation.ForUsername("groupDomain", ctx.MappedLocation.Name);

                Assert.NotNull(result);
                Assert.NotSame(ctx, result);
                Assert.Same(ctx.ContentItem, result.ContentItem);
                Assert.Equal(expectedLoc, result.MappedLocation);
            }
        }
    }
}
