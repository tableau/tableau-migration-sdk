using System.Threading.Tasks;
using System.Xml.Linq;
using Moq;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Interop.Hooks.Transformers;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Interop.Hooks.Transformers
{
    public class ISyncXmlContentTransformerTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            public class TestImplementation : ISyncXmlContentTransformer<TestFileContentType>
            {
                public virtual void Execute(TestFileContentType ctx, XDocument xml) { }

                public virtual bool NeedsXmlTransforming(TestFileContentType ctx) => true;
            }

            [Fact]
            public async Task CallsExecuteAsync()
            {
                var mockTransformer = new Mock<TestImplementation>()
                {
                    CallBase = true
                };

                var mockFile = Freeze<Mock<IContentFileHandle>>();
                var ctx = Create<TestFileContentType>();

                var mockXmlStream = Freeze<Mock<ITableauFileXmlStream>>();
                var xml = Freeze<XDocument>();

                var result = await ((IMigrationHook<TestFileContentType>)mockTransformer.Object).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                mockFile.Verify(x => x.GetXmlStreamAsync(Cancel), Times.Once);
                mockXmlStream.Verify(x => x.GetXmlAsync(Cancel), Times.Once);

                mockTransformer.Verify(x => x.Execute(ctx, xml), Times.Once);
            }
        }
    }
}
