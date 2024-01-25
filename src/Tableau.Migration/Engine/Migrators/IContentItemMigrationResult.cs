using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Migrators
{
    /// <summary>
    /// <see cref="IResult"/> object for a content item migration action.
    /// </summary>
    public interface IContentItemMigrationResult<TContent> : IResult
    {
        /// <summary>
        /// Gets whether or not the current migration batch should continue.
        /// </summary>
        bool ContinueBatch { get; }

        /// <summary>
        /// Gets whether the item migration was canceled.
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// Gets the manifest entry for the content item.
        /// </summary>
        IMigrationManifestEntry ManifestEntry { get; }

        /// <summary>
        /// Creates a new <see cref="IContentItemMigrationResult{TContent}"/> object while modifying the <see cref="ContinueBatch"/> value.
        /// </summary>
        /// <param name="continueBatch">Whether or not the current migration batch should continue.</param>
        /// <returns>The new <see cref="IContentItemMigrationResult{TContent}"/> object.</returns>
        IContentItemMigrationResult<TContent> ForContinueBatch(bool continueBatch);
    }
}
