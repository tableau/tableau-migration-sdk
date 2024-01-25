using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Migrators.Batch
{
    /// <summary>
    /// Interface for an object that migrates a batch of content items.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IContentBatchMigrator<TContent>
        where TContent : class, IContentReference
    {
        /// <summary>
        /// Migrates a batch of content items.
        /// </summary>
        /// <param name="itemBatch">The batch of content items to migrate.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The results of the batch migration.</returns>
        Task<IContentBatchMigrationResult<TContent>> MigrateAsync(ImmutableArray<ContentMigrationItem<TContent>> itemBatch, CancellationToken cancel);
    }
}
