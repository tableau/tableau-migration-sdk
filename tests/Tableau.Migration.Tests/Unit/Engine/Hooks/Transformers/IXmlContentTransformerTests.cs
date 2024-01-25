using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Moq;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Hooks.Transformers;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers
{
    public class IXmlContentTransformerTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            public class TestImplementation : IXmlContentTransformer<TestFileContentType>
            {
                public virtual async Task ExecuteAsync(TestFileContentType ctx, XDocument xml, CancellationToken cancel)
                {
                    await Task.CompletedTask;
                }

                public Func<TestFileContentType, bool> NeedsXmlTransformingFilter = _ => true;

                public bool NeedsXmlTransforming(TestFileContentType ctx) => NeedsXmlTransformingFilter(ctx);
            }

            [Fact]
            public async Task FiltersItemsAsync()
            {
                var mockFile = Freeze<Mock<IContentFileHandle>>();
                var ctx = Create<TestFileContentType>();

                var transformer = new TestImplementation();
                transformer.NeedsXmlTransformingFilter = _ => false;

                var result = await ((IXmlContentTransformer<TestFileContentType>)transformer).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);
                mockFile.Verify(x => x.GetXmlStreamAsync(Cancel), Times.Never);
            }

            [Fact]
            public async Task GetsOrReadsXmlAsync()
            {
                var mockFile = Freeze<Mock<IContentFileHandle>>();
                var ctx = Create<TestFileContentType>();

                var mockXmlStream = Freeze<Mock<ITableauFileXmlStream>>();
                var xml = Freeze<XDocument>();

                var mockTransformer = new Mock<TestImplementation> { CallBase = true };

                var result = await ((IXmlContentTransformer<TestFileContentType>)mockTransformer.Object).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                mockFile.Verify(x => x.GetXmlStreamAsync(Cancel), Times.Once);
                mockXmlStream.Verify(x => x.GetXmlAsync(Cancel), Times.Once);

                mockTransformer.Verify(x => x.ExecuteAsync(ctx, xml, Cancel), Times.Once);
            }
        }
    }
}
