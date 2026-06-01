using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// Record that represents searching for a destination content reference.
    /// </summary>
    /// <param name="Status">The manifest entry status of the source item.</param>
    /// <param name="Destination">The found destination item.</param>
    public record DestinationContentReferenceResult(MigrationManifestEntryStatus Status, IContentReference? Destination)
    {
        /// <summary>
        /// And empty result, indicates that there wasn't enough source information to make a destination search.
        /// </summary>
        public static readonly DestinationContentReferenceResult Empty = new(MigrationManifestEntryStatus.Pending, null);

        /// <summary>
        /// Creates a new <see cref="DestinationContentReferenceResult"/> object.
        /// </summary>
        /// <param name="entry">A manifest entry.</param>
        public DestinationContentReferenceResult(IMigrationManifestEntry entry)
            : this(entry.Status, entry.Destination) 
        { }
    }
}
