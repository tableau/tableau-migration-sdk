using System.Threading;
using System.Xml.Linq;
using Moq;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class MockXmlFileHandle : Mock<IContentFileHandle>
    {
        public XDocument Xml { get; set; }

        public Mock<ITableauFileXmlStream> MockXmlStream { get; set; }

        public MockXmlFileHandle()
            : base()
        {
            Xml = new();

            MockXmlStream = new();
            MockXmlStream.Setup(x => x.GetXmlAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Xml);

            Setup(x => x.GetXmlStreamAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => MockXmlStream.Object);
        }
    }
}
