using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// Interface for an object that can find equivalent destination content 
    /// given source content information, applying mapping rules.
    /// </summary>
    public interface IMappedContentReferenceFinder
    {
        /// <summary>
        /// Finds the equivalent destination content reference for the source reference.
        /// </summary>
        /// <param name="sourceLocation">The source content reference location.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The found destination content reference, or null if no equivalent content exists.</returns>
        Task<IContentReference?> FindDestinationReferenceAsync(ContentLocation sourceLocation, CancellationToken cancel);

        /// <summary>
        /// Finds the destination content reference for a mapped destination location.
        /// </summary>
        /// <param name="sourceLocation">The mapped content reference location.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The found destination content reference, or null if no content was found.</returns>
        Task<IContentReference?> FindMappedDestinationReferenceAsync(ContentLocation sourceLocation, CancellationToken cancel);
        
        /// <summary>
        /// Finds the equivalent destination content reference for the source reference.
        /// </summary>
        /// <param name="sourceId">The source content ID.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The found destination content reference, or null if no equivalent content exists.</returns>
        Task<IContentReference?> FindDestinationReferenceAsync(Guid sourceId, CancellationToken cancel);

        /// <summary>
        /// Finds the equivalent destination content reference for the source reference.
        /// </summary>
        /// <param name="contentUrl">The source content URL.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The found destination content reference, or null if no equivalent content exists.</returns>
        Task<IContentReference?> FindDestinationReferenceAsync(string contentUrl, CancellationToken cancel);
    }

    /// <summary>
    /// Interface for an object that can find equivalent destination content 
    /// given source content information, applying mapping rules.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IMappedContentReferenceFinder<TContent> : IMappedContentReferenceFinder
    { }
}
