using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Labels
{
    /// <summary>
    /// Interface for an API client that gets or modifies content item's labels.
    /// </summary>
    public interface ILabelsContentApiClient<TContent>
        where TContent : IContentReference, IWithLabels
    {
        /// <summary>
        /// Gets the labels API client.
        /// </summary>
        ILabelsApiClient<TContent> Labels { get; }
    }
}
