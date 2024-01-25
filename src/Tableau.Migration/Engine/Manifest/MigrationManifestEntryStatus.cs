namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// An enumeration of the various migration statuses states of a content item.
    /// </summary>
    public enum MigrationManifestEntryStatus
    {
        /// <summary>
        /// The content item has not yet been processed.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// The content item was not migrated due to filtering.
        /// </summary>
        Skipped,

        /// <summary>
        /// The content item was migrated successfully.
        /// </summary>
        Migrated,

        /// <summary>
        /// An attempt was made to migrate the content item, but it resulted in one or more errors.
        /// The content item may be missing on the destination or may be partially migrated.
        /// </summary>
        Error,

        /// <summary>
        /// An attempt was made to migrate the content item, but the process was canceled mid-migration.
        /// The content item may be missing on the destination or may be partially migrated.
        /// </summary>
        Canceled
    }
}
