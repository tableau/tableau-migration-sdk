using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Mappings
{
    public class ContentMappingBaseTests : AutoFixtureTestBase
    {
        internal class StubUserMapping : ContentMappingBase<IUser>
        {
            private readonly ContentMappingContext<IUser> _searchLocation;
            private readonly ContentMappingContext<IUser> _replaceLocation;

            public StubUserMapping(ContentMappingContext<IUser> searchLocation, ContentMappingContext<IUser> replaceLocation)
            {
                _searchLocation = searchLocation;
                _replaceLocation = replaceLocation;
            }

            public override Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
            {
                if (ctx == _searchLocation)
                {
                    return _replaceLocation.ToTask();
                }

                return ctx.ToTask();
            }
        }

        [Fact]
        public async Task Map()
        {
            // Arrange
            // Create mock data

            var mappedLoc = Create<ContentMappingContext<IUser>>();
            var unmappedLoc = Create<ContentMappingContext<IUser>>();
            var replaceLoc = Create<ContentMappingContext<IUser>>();

            var mapping = new StubUserMapping(mappedLoc, replaceLoc);

            // Act
            var mappedResult = await mapping.ExecuteAsync(mappedLoc, default);
            var unmappedResult = await mapping.ExecuteAsync(unmappedLoc, default);

            // Asserts
            Assert.Equal(replaceLoc, mappedResult);
            Assert.Equal(unmappedLoc, unmappedResult);
        }
    }
}
