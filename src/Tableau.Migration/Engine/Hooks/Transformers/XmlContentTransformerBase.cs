using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Transformers
{
    /// <summary>
    /// Abstract base class for transformers that manipulate the Tableau XML file of a content item. 
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    public abstract class XmlContentTransformerBase<TPublish> : IXmlContentTransformer<TPublish>
        where TPublish : IFileContent
    {
        /// <summary>
        /// Finds whether the content item needs any XML changes, 
        /// returning false prevents file IO from occurring.
        /// </summary>
        /// <param name="ctx">The content item to inspect.</param>
        /// <returns>Whether or not the content item needs XML changes.</returns>
        protected virtual bool NeedsXmlTransforming(TPublish ctx) => true;

        bool IXmlContentTransformer<TPublish>.NeedsXmlTransforming(TPublish ctx) => NeedsXmlTransforming(ctx);

        /// <inheritdoc />
        public abstract Task ExecuteAsync(TPublish ctx, XDocument xml, CancellationToken cancel);
    }
}
