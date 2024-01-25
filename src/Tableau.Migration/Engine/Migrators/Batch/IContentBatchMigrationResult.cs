using System.Collections.Immutable;

namespace Tableau.Migration.Engine.Migrators.Batch
{
    /// <summary>
    /// <see cref="IResult"/> object for a migration action.
    /// </summary>
    public interface IContentBatchMigrationResult<TContent> : IResult
        where TContent : IContentReference
    {
        /// <summary>
        /// Gets whether or not to migrate the next batch, if any.
        /// </summary>
        bool PerformNextBatch { get; }

        /// <summary>
        /// Gets the migration result of each item in the batch, in the order they finished.
        /// </summary>
        IImmutableList<IContentItemMigrationResult<TContent>> ItemResults { get; }

        /// <summary>
        /// Creates a new <see cref="IContentBatchMigrationResult{TContent}"/> object while modifying the <see cref="PerformNextBatch"/> value.
        /// </summary>
        /// <param name="performNextBatch">Whether or not to migrate the next batch.</param>
        /// <returns>The new <see cref="IContentBatchMigrationResult{TContent}"/> object.</returns>
        IContentBatchMigrationResult<TContent> ForNextBatch(bool performNextBatch);
    }
}
