using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an object that can initialize a <see cref="IApiClientInput"/> object.
    /// </summary>
    /// <remarks>
    /// This interface is internal because it is only used to build a <see cref="IApiClientInput"/> object, 
    /// which in turn is only used to build a <see cref="IApiClientInput"/> object.
    /// End users are intended to inject the final <see cref="IApiClientInput"/> result and not bootstrap objects.
    /// </remarks>
    internal interface IApiClientInputInitializer : IApiClientInput
    {
        /// <summary>
        /// Gets whether or not the API client input has been initialized yet.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initializes the <see cref="IApiClientInput"/> object.
        /// </summary>
        /// <param name="siteConnectionConfig">The site connection configuration to initialize the <see cref="IApiClient"/> with.</param>
        /// <param name="finderFactoryOverride">A content reference finder factory to use in place of the default.</param>
        /// <param name="fileStoreOverride">The file store to use in place of the default.</param>
        void Initialize(TableauSiteConnectionConfiguration siteConnectionConfig,
            IContentReferenceFinderFactory? finderFactoryOverride = null,
            IContentFileStore? fileStoreOverride = null);
    }
}
