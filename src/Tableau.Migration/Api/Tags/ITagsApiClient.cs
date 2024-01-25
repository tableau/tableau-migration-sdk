using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Tags
{
    /// <summary>
    /// Interface for an API client that modifies content's tags.
    /// </summary>
    public interface ITagsApiClient
    {
        /// <summary>
        /// Adds tags to the content item.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="tags">The tags to add.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>All tags on content item.</returns>
        Task<IResult<IImmutableList<ITag>>> AddTagsAsync(Guid contentItemId, IEnumerable<ITag> tags, CancellationToken cancel);


        /// <summary>
        /// Remove tags from the content item.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="tags">The tags to remove.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult> RemoveTagsAsync(Guid contentItemId, IEnumerable<ITag> tags, CancellationToken cancel);

        /// <summary>
        /// Adds and removes tags from the content item to match the new tags.
        /// </summary>
        /// <param name="contentItemId">The ID of the content ite.</param>
        /// <param name="tags">The tags to update to match.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult> UpdateTagsAsync(Guid contentItemId, IEnumerable<ITag> tags, CancellationToken cancel);
    }
}