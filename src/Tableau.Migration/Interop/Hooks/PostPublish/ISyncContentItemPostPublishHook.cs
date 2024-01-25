using Tableau.Migration.Engine.Hooks.PostPublish;

namespace Tableau.Migration.Interop.Hooks.PostPublish
{
    /// <summary>
    /// Interface representing a synchronous hook called when a migration attempt for a single content item completes.
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    /// <typeparam name="TResult"><inheritdoc /></typeparam>
    public interface ISyncContentItemPostPublishHook<TPublish, TResult>
        : ISyncMigrationHook<ContentItemPostPublishContext<TPublish, TResult>>, IContentItemPostPublishHook<TPublish, TResult>
    {
        /// <summary>
        /// Executes a hook callback.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous hook.</param>
        /// <returns>
        /// The context, 
        /// potentially modified to pass on to the next hook or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        new ContentItemPostPublishContext<TPublish, TResult>? Execute(ContentItemPostPublishContext<TPublish, TResult> ctx);
    }
}
