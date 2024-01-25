using System;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// <see cref="IMappedContentReferenceFinder{TContent}"/> implementation 
    /// that uses the mapped manifest information to find destination content, 
    /// falling back to API loading.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class ManifestDestinationContentReferenceFinder<TContent>
        : IMappedContentReferenceFinder<TContent>, IContentReferenceFinder<TContent>
        where TContent : IContentReference
    {
        private readonly IMigrationManifestEditor _manifest;
        private readonly IContentReferenceCache _destinationCache;

        /// <summary>
        /// Creates a new <see cref="ManifestDestinationContentReferenceFinder{TContent}"/> object.
        /// </summary>
        /// <param name="manifest">The migration manifest.</param>
        /// <param name="pipeline">The pipeline to get a destination cache from.</param>
        public ManifestDestinationContentReferenceFinder(IMigrationManifestEditor manifest, IMigrationPipeline pipeline)
        {
            _manifest = manifest;
            _destinationCache = pipeline.CreateDestinationCache<TContent>();
        }

        #region - IMappedContentReferenceFinder<TContent> Implementation -

        /// <inheritdoc />
        public async Task<IContentReference?> FindDestinationReferenceAsync(ContentLocation sourceLocation, CancellationToken cancel)
        {
            //Get the DESTINATION reference for the SOURCE location.
            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (manifestEntries.BySourceLocation.TryGetValue(sourceLocation, out var entry))
            {
                if(entry.Destination is not null)
                {
                    return entry.Destination;
                }

                return await _destinationCache.ForLocationAsync(entry.MappedLocation, cancel).ConfigureAwait(false);
            }
             
            return null;
        }

        /// <inheritdoc />
        public async Task<IContentReference?> FindMappedDestinationReferenceAsync(ContentLocation mappedLocation, CancellationToken cancel)
        {
            //Get the DESTINATION reference for the DESTINATION location.
            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (manifestEntries.ByMappedLocation.TryGetValue(mappedLocation, out var entry) && entry.Destination is not null)
            {
                return entry.Destination;
            }

            return await _destinationCache.ForLocationAsync(mappedLocation, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IContentReference?> FindDestinationReferenceAsync(Guid sourceId, CancellationToken cancel)
        {
            //Get the DESTINATION reference for the SOURCE ID.
            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (manifestEntries.BySourceId.TryGetValue(sourceId, out var entry))
            {
                return await FindDestinationReferenceAsync(entry.Source.Location, cancel).ConfigureAwait(false);
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<IContentReference?> FindDestinationReferenceAsync(string contentUrl, CancellationToken cancel)
        {
            //Get the DESTINATION reference for the SOURCE content URL.
            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (manifestEntries.BySourceContentUrl.TryGetValue(contentUrl, out var entry))
            {
                return await FindDestinationReferenceAsync(entry.Source.Location, cancel).ConfigureAwait(false);
            }

            return null;
        }

        #endregion

        #region - IContentReferenceFinder<TContent> Implementation -

        /// <inheritdoc />
        public async Task<IContentReference?> FindByIdAsync(Guid id, CancellationToken cancel)
        {
            //Get the DESTINATION reference for the DESTINATION ID.
            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            if (manifestEntries.ByDestinationId.TryGetValue(id, out var entry) && entry.Destination is not null)
            {
                return entry.Destination;
            }

            return await _destinationCache.ForIdAsync(id, cancel).ConfigureAwait(false);
        }

        #endregion
    }
}
