using Tableau.Migration.Engine.Hooks.PostPublish;

namespace Tableau.Migration.Interop.Hooks.PostPublish
{
    /// <summary>
    /// Interface representing a synchronous hook called when a migration attempt for bulk content items completes.
    /// </summary>
    /// <typeparam name="TSource"><inheritdoc/></typeparam>
    public interface ISyncBulkPostPublishHook<TSource>
        : ISyncMigrationHook<BulkPostPublishContext<TSource>>, IBulkPostPublishHook<TSource>
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
        new BulkPostPublishContext<TSource>? Execute(BulkPostPublishContext<TSource> ctx);
    }
}
