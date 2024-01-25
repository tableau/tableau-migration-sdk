using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Preparation
{
    /// <summary>
    /// <see cref="IContentItemPreparer{TContent, TPublish}"/> implementation that publishes the source item as-is
    /// and does not require extra pulled information.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class SourceContentItemPreparer<TContent> : ContentItemPreparerBase<TContent, TContent>
        where TContent : class
    {
        /// <summary>
        /// Creates a new <see cref="SourceContentItemPreparer{TContent}"/>.
        /// </summary>
        /// <param name="transformerRunner"><inheritdoc /></param>
        /// <param name="pipeline"><inheritdoc /></param>
        public SourceContentItemPreparer(IContentTransformerRunner transformerRunner, IMigrationPipeline pipeline)
            : base(transformerRunner, pipeline)
        { }

        /// <inheritdoc />
        protected override Task<IResult<TContent>> PullAsync(ContentMigrationItem<TContent> item, CancellationToken cancel)
        {
            var result = Result<TContent>.Succeeded(item.SourceItem);
            return Task.FromResult<IResult<TContent>>(result);
        }
    }
}
