using System.Collections.Immutable;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Preparation;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// Interface for an object that defines the steps and strategies needed to perform a migration of Tableau data.
    /// </summary>
    public interface IMigrationPipeline
    {
        /// <summary>
        /// Builds the ordered actions in the pipeline to execute.
        /// </summary>
        ImmutableArray<IMigrationAction> BuildActions();

        /// <summary>
        /// Gets a content type level migrator for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The content type level migrator.</returns>
        IContentMigrator<TContent> GetMigrator<TContent>()
            where TContent : class, IContentReference;

        /// <summary>
        /// Gets a batch level migrator for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The batch level migrator.</returns>
        IContentBatchMigrator<TContent> GetBatchMigrator<TContent>()
            where TContent : class, IContentReference;

        /// <summary>
        /// Gets a content item preparer for the given content and publish types.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <typeparam name="TPublish">The publish type.</typeparam>
        /// <returns>The content preparer.</returns>
        IContentItemPreparer<TContent, TPublish> GetItemPreparer<TContent, TPublish>()
            where TContent : class
            where TPublish : class;

        /// <summary>
        /// Gets the destination cache for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The destination cache.</returns>
        IContentReferenceCache CreateDestinationCache<TContent>()
            where TContent : IContentReference;

        /// <summary>
        /// Gets the destination content finder for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The destination content finder.</returns>
        IMappedContentReferenceFinder<TContent> CreateDestinationFinder<TContent>()
            where TContent : IContentReference;

        /// <summary>
        /// Gets the destination locked project cache.
        /// </summary>
        /// <returns>The destination locked project cache.</returns>
        ILockedProjectCache GetDestinationLockedProjectCache();
    }
}
