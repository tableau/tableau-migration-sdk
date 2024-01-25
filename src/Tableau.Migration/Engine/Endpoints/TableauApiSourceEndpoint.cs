using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Endpoints.Search;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// <see cref="ISourceEndpoint"/> implementation that uses Tableau Server/Cloud APIs.
    /// </summary>
    public class TableauApiSourceEndpoint : TableauApiEndpointBase, ISourceApiEndpoint
    {
        /// <summary>
        /// Creates a new <see cref="TableauApiSourceEndpoint"/> object.
        /// </summary>
        /// <param name="serviceScopeFactory">A service scope factory to define an API client scope with.</param>
        /// <param name="config">The configuration options for connecting to the source endpoint APIs.</param>
        /// <param name="finderFactory">A source manifest finder factory.</param>
        /// <param name="fileStore">The file store to use.</param>
        public TableauApiSourceEndpoint(IServiceScopeFactory serviceScopeFactory,
            ITableauApiEndpointConfiguration config,
            ManifestSourceContentReferenceFinderFactory finderFactory,
            IContentFileStore fileStore)
            : base(serviceScopeFactory, config, finderFactory, fileStore)
        { }

        /// <inheritdoc />
        public async Task<IResult<TPublish>> PullAsync<TContent, TPublish>(TContent contentItem, CancellationToken cancel)
            where TPublish : class
        {
            var apiClient = SiteApi.GetPullApiClient<TContent, TPublish>();
            return await apiClient.PullAsync(contentItem, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<IPermissions>> GetPermissionsAsync<TContent>(IContentReference contentItem, CancellationToken cancel)
            where TContent : IPermissionsContent
        {
            return await GetPermissionsAsync(typeof(TContent), contentItem, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<IPermissions>> GetPermissionsAsync(Type type, IContentReference contentItem, CancellationToken cancel)
        {
            var apiClient = SiteApi.GetPermissionsApiClient(type);
            return await apiClient.GetPermissionsAsync(contentItem.Id, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<ImmutableList<IConnection>>> ListConnectionsAsync<TContent>(
            Guid contentItemId,
            CancellationToken cancel)
            where TContent : IWithConnections
        {
            var apiClient = SiteApi.GetConnectionsApiClient<TContent>();
            return await apiClient.GetConnectionsAsync(contentItemId, cancel).ConfigureAwait(false);
        }
    }
}
