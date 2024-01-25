using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Labels
{
    /// <summary>
    /// Interface for listing or updating labels of content items.
    /// </summary>
    public interface ILabelsApiClient<TContent>
        where TContent : IContentReference, IWithLabels
    {
        /// <summary>
        /// Get the labels for the content item.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>        
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <param name="categories">A list of label categories to filter by. 
        /// See <see cref="LabelCategories"/> for built-in categories.</param>
        /// <returns></returns>
        Task<IResult<ImmutableList<ILabel>>> GetLabelsAsync(
           Guid contentItemId,
           CancellationToken cancel,
           IEnumerable<string>? categories = null);

        /// <summary>
        /// Creates or updates the labels for the content item.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="labels">The create or update or update options for labels.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult<ImmutableList<ILabel>>> UpdateLabelsAsync(
            Guid contentItemId,
            IEnumerable<ILabelUpdateOptions> labels,
            CancellationToken cancel);
    }
}