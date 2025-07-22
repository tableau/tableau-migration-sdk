using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// Interface for an object that can find destination views 
    /// for given source view information, applying mapping rules.
    /// </summary>
    public interface IDestinationViewReferenceFinder
    {
        /// <summary>
        /// Finds the destination view reference for the source view ID.
        /// </summary>
        /// <param name="sourceViewId">The source view ID.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The result containing the found destination view reference, or a failed result if no destination view was found.</returns>
        Task<IResult<IContentReference>> FindBySourceIdAsync(Guid sourceViewId, CancellationToken cancel);
    }
}
