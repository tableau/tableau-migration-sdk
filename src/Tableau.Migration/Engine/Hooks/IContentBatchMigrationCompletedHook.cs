using Tableau.Migration.Engine.Migrators.Batch;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Interface representing a hook called when a <see cref="IContentBatchMigrator{TContent}"/> completes.
    /// </summary>
    public interface IContentBatchMigrationCompletedHook<TContent> : IMigrationHook<IContentBatchMigrationResult<TContent>>
        where TContent : IContentReference
    { }
}
