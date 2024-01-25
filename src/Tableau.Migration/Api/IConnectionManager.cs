using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for listing or updating embedded connections of content items.
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// List the connection on the content item.
        /// </summary>
        /// <param name="urlPrefix">The URL prefix of the content item to get connections for.</param>
        /// <param name="contentItemId">The ID of the content item to get connections for.</param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        Task<IResult<ImmutableList<IConnection>>> ListConnectionsAsync(
            string urlPrefix,
            Guid contentItemId,
            CancellationToken cancel);

        /// <summary>
        /// Update the connection on the content item.
        /// </summary>
        /// <param name="urlPrefix">The URL prefix of the content item to update connections for.</param>            
        /// <param name="contentItemId">The ID of the content item to update connections for.</param>
        /// <param name="connectionId">The ID of the connection to be updated.</param>
        /// <param name="options">The update connetion options.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult<IConnection>> UpdateConnectionAsync(
            string urlPrefix,
            Guid contentItemId,
            Guid connectionId,
            IUpdateConnectionOptions options,
            CancellationToken cancel);
    }
}