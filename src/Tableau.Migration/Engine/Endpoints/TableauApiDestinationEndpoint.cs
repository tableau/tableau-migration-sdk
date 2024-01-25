using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Endpoints.Search;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// <see cref="ISourceEndpoint"/> implementation that uses Tableau Server/Cloud APIs.
    /// </summary>
    public class TableauApiDestinationEndpoint : TableauApiEndpointBase, IDestinationApiEndpoint
    {
        /// <summary>
        /// Creates a new <see cref="TableauApiDestinationEndpoint"/> object.
        /// </summary>
        /// <param name="serviceScopeFactory">A service scope factory to define an API client scope with.</param>
        /// <param name="config">The configuration options for connecting to the destination endpoint APIs.</param>
        /// <param name="finderFactory">A destination finder factory.</param>
        /// <param name="fileStore">The file store to use.</param>
        public TableauApiDestinationEndpoint(IServiceScopeFactory serviceScopeFactory,
            ITableauApiEndpointConfiguration config,
            ManifestDestinationContentReferenceFinderFactory finderFactory,
            IContentFileStore fileStore)
            : base(serviceScopeFactory, config, finderFactory, fileStore)
        { }

        /// <inheritdoc />
        public async Task<IResult<TPublishResult>> PublishAsync<TPublish, TPublishResult>(TPublish publishItem, CancellationToken cancel)
            where TPublishResult : class, IContentReference
        {
            var publishApiClient = SiteApi.GetPublishApiClient<TPublish, TPublishResult>();
            return await publishApiClient.PublishAsync(publishItem, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult> PublishBatchAsync<TPublish>(IEnumerable<TPublish> publishItems, CancellationToken cancel)
        {
            var publishApiClient = SiteApi.GetBatchPublishApiClient<TPublish>();
            return await publishApiClient.PublishBatchAsync(publishItems, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<IPermissions>> UpdatePermissionsAsync<TContent>(IContentReference contentItem, IPermissions permissions, CancellationToken cancel)
            where TContent : IPermissionsContent
        {
            return await UpdatePermissionsAsync(typeof(TContent), contentItem, permissions, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<IPermissions>> UpdatePermissionsAsync(Type type, IContentReference contentItem, IPermissions permissions, CancellationToken cancel)
        {
            var apiClient = SiteApi.GetPermissionsApiClient(type);
            return await apiClient.UpdatePermissionsAsync(contentItem.Id, permissions, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult> UpdateTagsAsync<TContent>(IContentReference contentItem, IEnumerable<ITag> tags, CancellationToken cancel)
            where TContent : IWithTags
        {
            var apiClient = SiteApi.GetTagsApiClient<TContent>();
            return await apiClient.UpdateTagsAsync(contentItem.Id, tags, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult> UpdateOwnerAsync<TContent>(IContentReference contentItem, IContentReference owner, CancellationToken cancel)
            where TContent : IWithOwner
        {
            var apiClient = SiteApi.GetOwnershipApiClient<TContent>();
            return await apiClient.ChangeOwnerAsync(contentItem.Id, owner.Id, cancel).ConfigureAwait(false);
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

        /// <inheritdoc />
        public async Task<IResult<IConnection>> UpdateConnectionAsync<TContent>(
            Guid contentItemId,
            Guid connectionId,
            IUpdateConnectionOptions options,
            CancellationToken cancel) where TContent : IWithConnections
        {
            var apiClient = SiteApi.GetConnectionsApiClient<TContent>();
            return await apiClient.UpdateConnectionAsync(contentItemId, connectionId, options, cancel).ConfigureAwait(false);
        }
    }
}
