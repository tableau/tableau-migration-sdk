using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Record containing in-progress migration state for a content item.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <param name="SourceItem">The content item's source data.</param>
    /// <param name="ManifestEntry">The manifest entry that describes the content item's overall migration status.</param>
    public record ContentMigrationItem<TContent>(TContent SourceItem, IMigrationManifestEntryEditor ManifestEntry)
    { }
}
