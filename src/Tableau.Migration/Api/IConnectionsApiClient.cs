using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an API client that gets or modifies content item's connections.
    /// </summary>    
    public interface IConnectionsApiClient
    {
        /// <summary>
        /// List the content item's connections.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>An immutable list of connections.</returns>
        Task<IResult<ImmutableList<IConnection>>> GetConnectionsAsync(
            Guid contentItemId,
            CancellationToken cancel);

        /// <summary>
        /// Update the connection on the data source.
        /// </summary>       
        /// <param name="contentItemId">The ID of the content item to update connections for.</param>
        /// <param name="connectionId">The ID of the connection to be updated.</param>
        /// <param name="options">The update connetion options.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult<IConnection>> UpdateConnectionAsync(
            Guid contentItemId,
            Guid connectionId,
            IUpdateConnectionOptions options,
            CancellationToken cancel);
    }
}