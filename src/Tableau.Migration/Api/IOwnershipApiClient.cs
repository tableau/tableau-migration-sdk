using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an API client that modifies content item's ownership.
    /// </summary>    
    public interface IOwnershipApiClient
    {
        /// <summary>
        /// Change the owner of a content item.
        /// </summary>
        /// <param name="contentItemId">The ID of content item to change the owner.</param>
        /// <param name="newOwnerId">The new owner identificator.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The result of changing the item ownership.</returns>
        Task<IResult> ChangeOwnerAsync(Guid contentItemId, Guid newOwnerId, CancellationToken cancel);
    }
}
