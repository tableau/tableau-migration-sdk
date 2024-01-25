using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Transformers;

namespace Tableau.Migration.Interop.Hooks.Transformers
{
    /// <summary>
    /// Interface for a content transformer that can synchronously manipulate the Tableau XML file of a content item. 
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    public interface ISyncXmlContentTransformer<TPublish> : IXmlContentTransformer<TPublish>
        where TPublish : IFileContent
    {
        /// <summary>
        /// Finds whether the content item needs any XML changes, 
        /// returning false prevents file IO from occurring.
        /// </summary>
        /// <param name="ctx">The content item to inspect.</param>
        /// <returns>Whether or not the content item needs XML changes.</returns>
        new bool NeedsXmlTransforming(TPublish ctx);

        /// <inheritdoc />
        bool IXmlContentTransformer<TPublish>.NeedsXmlTransforming(TPublish ctx) => NeedsXmlTransforming(ctx);

        /// <summary>
        /// Transforms the XML of the content item.
        /// </summary>
        /// <param name="ctx">The content item being transformed.</param>
        /// <param name="xml">
        /// The XML of the content item to transform.
        /// Any changes made to the XML are persisted back to the file before publishing.
        /// </param>
        void Execute(TPublish ctx, XDocument xml);

        /// <inheritdoc />
        Task IXmlContentTransformer<TPublish>.ExecuteAsync(TPublish ctx, XDocument xml, CancellationToken cancel)
            => Task.Run(() => Execute(ctx, xml), cancel);
    }
}
