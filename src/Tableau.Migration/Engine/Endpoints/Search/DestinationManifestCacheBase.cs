using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// Abstract base class for <see cref="IContentReferenceCache"/> implementations that
    /// first make use of the manifest's destination information, 
    /// falling back to a data store lookup of some kind.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public abstract class DestinationManifestCacheBase<TContent> : ContentReferenceCacheBase
    {
        private readonly IMigrationManifestEditor _manifest;

        /// <summary>
        /// Creates a new <see cref="DestinationManifestCacheBase{TContent}"/> object.
        /// </summary>
        /// <param name="manifest">The current migration manifest.</param>
        protected DestinationManifestCacheBase(IMigrationManifestEditor manifest)
        {
            _manifest = manifest;
        }

        /// <summary>
        /// Searches the data store for at least the given location, 
        /// possibly with more items returned for opportunistic caching.
        /// </summary>
        /// <param name="searchLocation">The primary search location to search for.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The search results.</returns>
        protected abstract ValueTask<IEnumerable<ContentReferenceStub>> SearchStoreAsync(ContentLocation searchLocation, CancellationToken cancel);

        /// <summary>
        /// Searches the data store for at least the given ID, 
        /// possibly with more items returned for opportunistic caching.
        /// </summary>
        /// <param name="searchId">The primary ID to search for.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The search results.</returns>
        protected abstract ValueTask<IEnumerable<ContentReferenceStub>> SearchStoreAsync(Guid searchId, CancellationToken cancel);

        /// <inheritdoc />
        protected override async ValueTask<IEnumerable<ContentReferenceStub>> SearchAsync(ContentLocation searchLocation, CancellationToken cancel)
        {
            var entries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (entries.ByMappedLocation.TryGetValue(searchLocation, out var entry))
            {
                if (entry.Destination is not null)
                {
                    return new[] { new ContentReferenceStub(entry.Destination) };
                }
            }

            return await SearchStoreAsync(searchLocation, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override async ValueTask<IEnumerable<ContentReferenceStub>> SearchAsync(Guid searchId, CancellationToken cancel)
        {
            var entries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (entries.ByDestinationId.TryGetValue(searchId, out var entry))
            {
                if (entry.Destination is not null)
                {
                    return new[] { new ContentReferenceStub(entry.Destination) };
                }
            }

            return await SearchStoreAsync(searchId, cancel).ConfigureAwait(false);
        }
    }
}
