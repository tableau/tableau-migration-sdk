using System;
using System.Collections.Generic;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Interface for a collection of <see cref="IMigrationManifestEntry"/> objects, partitioned for a specific content type.
    /// </summary>
    public interface IMigrationManifestContentTypePartition : IReadOnlyCollection<IMigrationManifestEntry>, IEquatable<IMigrationManifestContentTypePartition>
    {
        /// <summary>
        /// Gets the content type the partition holds manifest entries for.
        /// </summary>
        Type ContentType { get; }
    }
}
