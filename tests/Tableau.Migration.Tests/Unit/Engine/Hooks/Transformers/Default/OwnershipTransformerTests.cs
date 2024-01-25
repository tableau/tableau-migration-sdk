using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class OwnershipTransformerTests
    {
        private class TestOwnershipType : TestContentType, IWithOwner
        {
            public IContentReference Owner { get; set; } = null!;
        }

        public class ExecuteAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task SetsMappedOwnerAsync()
            {
                var cancel = new CancellationToken();
                var ctx = Create<TestOwnershipType>();

                var mockUserTransformer = new Mock<IMappedUserTransformer>();

                var mappedRef = Create<IContentReference>();
                mockUserTransformer.Setup(x => x.ExecuteAsync(ctx.Owner, cancel))
                    .ReturnsAsync(mappedRef);

                var transformer = new OwnershipTransformer<TestOwnershipType>(mockUserTransformer.Object);

                var result = await transformer.ExecuteAsync(ctx, cancel);

                Assert.NotNull(result);
                Assert.Same(ctx, result);

                Assert.Same(mappedRef, result.Owner);
            }
        }
    }
}
