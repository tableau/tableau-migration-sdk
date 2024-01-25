using System;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// <see cref="IContentReferenceFinder{TContent}"/> implementation that finds source references
    /// from the migration manifest.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class ManifestSourceContentReferenceFinder<TContent> : IContentReferenceFinder<TContent>
        where TContent : IContentReference
    {
        private readonly IMigrationManifestEditor _manifest;

        /// <summary>
        /// Creates a new <see cref="ManifestSourceContentReferenceFinder{TContent}"/> object.
        /// </summary>
        /// <param name="manifest">The manifest.</param>
        public ManifestSourceContentReferenceFinder(IMigrationManifestEditor manifest)
        {
            _manifest = manifest;
        }

        /// <inheritdoc />
        public Task<IContentReference?> FindByIdAsync(Guid id, CancellationToken cancel)
        {
            var partition = _manifest.Entries.GetOrCreatePartition<TContent>();

            if (partition.BySourceId.TryGetValue(id, out var entry))
            {
                return Task.FromResult<IContentReference?>(entry.Source);
            }

            return Task.FromResult<IContentReference?>(null);
        }
    }
}
