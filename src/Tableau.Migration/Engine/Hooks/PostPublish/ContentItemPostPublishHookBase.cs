using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.PostPublish
{
    /// <summary>
    /// Base class for <see cref="IContentItemPostPublishHook{TSource, TDestination}"/> implementations.
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    /// <typeparam name="TResult"><inheritdoc /></typeparam>
    public abstract class ContentItemPostPublishHookBase<TPublish, TResult> : IContentItemPostPublishHook<TPublish, TResult>
    {
        /// <inheritdoc/>
        public abstract Task<ContentItemPostPublishContext<TPublish, TResult>?> ExecuteAsync(
            ContentItemPostPublishContext<TPublish, TResult> ctx,
            CancellationToken cancel);
    }

    /// <summary>
    /// Base class for <see cref="IContentItemPostPublishHook{TSource, TDestination}"/> implementations.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public abstract class ContentItemPostPublishHookBase<TContent> : ContentItemPostPublishHookBase<TContent, TContent>
    { }
}
