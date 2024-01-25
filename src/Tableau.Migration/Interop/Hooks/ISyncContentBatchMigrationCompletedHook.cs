using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Migrators.Batch;

namespace Tableau.Migration.Interop.Hooks
{
    /// <summary>
    /// Interface representing a hook called synchronously when a <see cref="IContentBatchMigrator{TContent}"/> completes.
    /// </summary>
    public interface ISyncContentBatchMigrationCompletedHook<TContent>
        : ISyncMigrationHook<IContentBatchMigrationResult<TContent>>, IContentBatchMigrationCompletedHook<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Executes a content batch migration completed callback.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous hook.</param>
        /// <returns>
        /// The context, 
        /// potentially modified to pass on to the next hook or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        new IContentBatchMigrationResult<TContent>? Execute(IContentBatchMigrationResult<TContent> ctx);
    }
}
