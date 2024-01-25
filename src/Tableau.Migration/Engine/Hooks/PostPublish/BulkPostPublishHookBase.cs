using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.PostPublish
{
    /// <summary>
    /// Base class for <see cref="IBulkPostPublishHook{TSource}"/> implementations.
    /// </summary>
    public abstract class BulkPostPublishHookBase<TSource> : IBulkPostPublishHook<TSource>
    {
        /// <inheritdoc/>
        public abstract Task<BulkPostPublishContext<TSource>?> ExecuteAsync(
            BulkPostPublishContext<TSource> ctx,
            CancellationToken cancel);
    }
}
