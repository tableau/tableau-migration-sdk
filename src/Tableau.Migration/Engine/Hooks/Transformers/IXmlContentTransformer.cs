using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Transformers
{
    /// <summary>
    /// Interface for a content transformer that manipulates the Tableau XML file of a content item. 
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    public interface IXmlContentTransformer<TPublish> : IContentTransformer<TPublish>
        where TPublish : IFileContent
    {
        /// <summary>
        /// Finds whether the content item needs any XML changes, 
        /// returning false prevents file IO from occurring.
        /// </summary>
        /// <param name="ctx">The content item to inspect.</param>
        /// <returns>Whether or not the content item needs XML changes.</returns>
        bool NeedsXmlTransforming(TPublish ctx);

        /// <summary>
        /// Transforms the XML of the content item.
        /// </summary>
        /// <param name="ctx">The content item being transformed.</param>
        /// <param name="xml">
        /// The XML of the content item to transform.
        /// Any changes made to the XML are persisted back to the file before publishing.
        /// </param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>A task to await.</returns>
        Task ExecuteAsync(TPublish ctx, XDocument xml, CancellationToken cancel);

        /// <inheritdoc />
        async Task<TPublish?> IMigrationHook<TPublish>.ExecuteAsync(TPublish ctx, CancellationToken cancel)
        {
            if (!NeedsXmlTransforming(ctx))
            {
                return ctx;
            }

            //We expect the item preparer to finalize/dispose the file stream.
            var xmlStream = await ctx.File.GetXmlStreamAsync(cancel).ConfigureAwait(false);
            var xml = await xmlStream.GetXmlAsync(cancel).ConfigureAwait(false);

            await ExecuteAsync(ctx, xml, cancel).ConfigureAwait(false);

            return ctx;
        }
    }
}
