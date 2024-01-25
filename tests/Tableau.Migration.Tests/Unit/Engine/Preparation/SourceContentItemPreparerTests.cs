using System.Threading.Tasks;
using Tableau.Migration.Engine.Preparation;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Preparation
{
    public class SourceContentItemPreparerTests
    {
        public class PullAsync : ContentItemPreparerTestBase<TestContentType>
        {
            [Fact]
            public async Task PullsSourceItemAsync()
            {
                var preparer = Create<SourceContentItemPreparer<TestContentType>>();
                var result = await preparer.PrepareAsync(Item, Cancel);

                result.AssertSuccess();
                Assert.Same(Item.SourceItem, result.Value);
            }
        }
    }
}
