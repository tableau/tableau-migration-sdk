using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an object that contains the input given for a <see cref="IApiClient"/>, 
    /// used to bootstrap api client dependency injection.
    /// </summary>
    /// <remarks>
    /// In almost all cases it is preferrable to inject the <see cref="IApiClient"/> object, 
    /// this interface is only intended to be used to build <see cref="IApiClient"/> object.
    /// </remarks>
    public interface IApiClientInput
    {
        /// <summary>
        /// Gets the site connection configuration to initialize the <see cref="IApiClient"/> with.
        /// </summary>
        TableauSiteConnectionConfiguration SiteConnectionConfiguration { get; }

        /// <summary>
        /// Gets the factory to access content reference finders with.
        /// </summary>
        IContentReferenceFinderFactory ContentReferenceFinderFactory { get; }

        /// <summary>
        /// Gets the file store to use.
        /// </summary>
        IContentFileStore FileStore { get; }
    }
}
