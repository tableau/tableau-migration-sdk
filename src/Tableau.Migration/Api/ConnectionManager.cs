using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal class ConnectionManager : ApiClientBase, IConnectionManager
    {
        public ConnectionManager(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
            : base(restRequestBuilderFactory, loggerFactory, sharedResourcesLocalizer)
        { }

        /// <inheritdoc/>
        public async Task<IResult<ImmutableList<IConnection>>> ListConnectionsAsync(
            string urlPrefix,
            Guid contentItemId,
            CancellationToken cancel)
        {
            var getResult = await RestRequestBuilderFactory
                .CreateUri($"{urlPrefix}/{contentItemId.ToUrlSegment()}/connections")
                .ForGetRequest()
                .SendAsync<ConnectionsResponse>(cancel)
                .ToResultAsync((response) =>
                {
                    var connections = response.Items;

                    if (!connections.Any())
                    {
                        return new List<IConnection>().ToImmutableList();
                    }

                    return connections.ToList().Select(c => (IConnection)new Connection(c)).ToImmutableList();
                },
                SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return getResult;
        }

        /// <inheritdoc/>
        public async Task<IResult<IConnection>> UpdateConnectionAsync(
            string urlPrefix,
            Guid contentItemId,
            Guid connectionId,
            IUpdateConnectionOptions options,
            CancellationToken cancel)
        {
            var updateResult = await RestRequestBuilderFactory
                .CreateUri($"{urlPrefix}/{contentItemId.ToUrlSegment()}/connections/{connectionId.ToUrlSegment()}")
                .ForPutRequest()
                .WithXmlContent(new UpdateConnectionRequest(options))
                .SendAsync<ConnectionResponse>(cancel)
                .ToResultAsync<ConnectionResponse, IConnection>(
                (response) => new Connection(response.Item!),
                SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return updateResult;
        }
    }
}
