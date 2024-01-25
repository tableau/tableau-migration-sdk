using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Transformers
{
    /// <summary>
    /// Base implementation for an object that can transform content of a specific content type
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc/></typeparam>
    public abstract class ContentTransformerBase<TPublish> : IContentTransformer<TPublish>
    {
        /// <inheritdoc />
        public abstract Task<TPublish?> ExecuteAsync(TPublish itemToTransform, CancellationToken cancel);
    }
}
