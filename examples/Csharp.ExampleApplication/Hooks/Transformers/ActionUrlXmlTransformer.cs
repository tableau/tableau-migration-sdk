using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Transformers;

namespace Csharp.ExampleApplication.Hooks.Transformers
{
    #region class

    public class ActionUrlXmlTransformer : XmlContentTransformerBase<IPublishableWorkbook>
    {
        protected override bool NeedsXmlTransforming(IPublishableWorkbook ctx)
        {
            /*
             * Returning false prevents TransformAsync from running.
             * Implementing this method potentially allows workbooks to migrate without 
             * loading the file into memory, improving migration speed.
             */
            return true;
        }

        public override Task TransformAsync(IPublishableWorkbook ctx, XDocument xml, CancellationToken cancel)
        {
            // Changes to the XML are saved back to the workbook file before publishing.
            foreach (var actionLink in xml.XPathSelectElements("//actions/*/link"))
            {
                actionLink.SetAttributeValue("expression", actionLink.Attribute("expression")?.Value?.Replace("127.0.0.1", "testserver"));
            }

            return Task.CompletedTask;
        }
    }

    #endregion
}
