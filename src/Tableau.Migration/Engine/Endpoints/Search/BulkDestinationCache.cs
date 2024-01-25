using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// <see cref="DestinationManifestCacheBase{TContent}"/> implementation
    /// that falls back to bulk API listing when destination information is not found in the manifest.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class BulkDestinationCache<TContent> : DestinationManifestCacheBase<TContent>
        where TContent : IContentReference
    {
        private readonly IMigrationManifestEditor _manifest;
        private readonly IDestinationEndpoint _endpoint;
        private readonly IConfigReader _configReader;

        private bool _loaded;

        /// <summary>
        /// Creates a new <see cref="BulkDestinationCache{TContent}"/>
        /// </summary>
        /// <param name="manifest">A migration manifest.</param>
        /// <param name="endpoint">A destination endpoint.</param>
        /// <param name="configReader">A config reader.</param>
        public BulkDestinationCache(IMigrationManifestEditor manifest, IDestinationEndpoint endpoint, IConfigReader configReader)
            : base(manifest)
        {
            _manifest = manifest;
            _endpoint = endpoint;
            _configReader = configReader;
        }

        /// <summary>
        /// Gets the configured batch size.
        /// </summary>
        protected int BatchSize => _configReader.Get().BatchSize;

        /// <summary>
        /// Called after an item is loaded into the cache from the store.
        /// </summary>
        /// <param name="item">The item that was loaded.</param>
        protected virtual void ItemLoaded(TContent item) { }

        /// <summary>
        /// Ensures that the cache is loaded.
        /// </summary>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The loaded items, or an empty value if the store has already been loaded.</returns>
        protected async ValueTask<IEnumerable<ContentReferenceStub>> LoadStoreAsync(CancellationToken cancel)
        {
            //Only load content a single time (unless we expire the cache)
            //This is so failed lookups don't cause us to re-list
            //everything just to fail the lookup again.
            if (_loaded)
            {
                return Enumerable.Empty<ContentReferenceStub>();
            }

            var manifestEntries = _manifest.Entries.GetOrCreatePartition<TContent>();
            var pager = _endpoint.GetPager<TContent>(BatchSize);

            cancel.ThrowIfCancellationRequested();

            int loadedCount = 0;

            var page = await pager.NextPageAsync(cancel).ConfigureAwait(false);
            var results = ImmutableArray.CreateBuilder<ContentReferenceStub>(page.TotalCount);
            while (!page.Value.IsNullOrEmpty())
            {
                foreach (var item in page.Value)
                {
                    var destinationInfo = new ContentReferenceStub(item);

                    //Assign this info to the manifest if there's an entry with our mapped location.
                    //This updates any ID/other information that may have changed since last run.
                    if (manifestEntries.ByMappedLocation.TryGetValue(item.Location, out var manifestEntry))
                    {
                        manifestEntry.DestinationFound(destinationInfo);
                    }

                    results.Add(destinationInfo);

                    ItemLoaded(item);
                    loadedCount++;
                }

                if (loadedCount >= page.TotalCount)
                    break;

                cancel.ThrowIfCancellationRequested();

                page = await pager.NextPageAsync(cancel).ConfigureAwait(false);
            }

            _loaded = true;
            return results.ToImmutable();
        }

        /// <inheritdoc />
        protected override async ValueTask<IEnumerable<ContentReferenceStub>> SearchStoreAsync(ContentLocation searchLocation, CancellationToken cancel)
            => await LoadStoreAsync(cancel).ConfigureAwait(false);

        /// <inheritdoc />
        protected override async ValueTask<IEnumerable<ContentReferenceStub>> SearchStoreAsync(Guid searchId, CancellationToken cancel)
            => await LoadStoreAsync(cancel).ConfigureAwait(false);
    }
}
