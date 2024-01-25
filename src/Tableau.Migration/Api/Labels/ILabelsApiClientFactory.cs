using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Labels
{
    /// <summary>
    /// Interface for an object that can create <see cref="ILabelsApiClient{TContent}"/> objects.
    /// </summary>
    public interface ILabelsApiClientFactory
    {
        /// <summary>
        /// Returns a <see cref="ILabelsApiClient{TContent}"/>.
        /// </summary>        
        /// <returns></returns>
        ILabelsApiClient<TContent> Create<TContent>()
            where TContent : IContentReference, IWithLabels;
    }
}
