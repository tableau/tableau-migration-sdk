using Tableau.Migration.Api.Rest;

namespace Tableau.Migration.Api.Permissions
{
    /// <summary>
    /// Interface for permissions API client factories.
    /// </summary>
    public interface IPermissionsApiClientFactory
    {
        /// <summary>
        /// Creates an <see cref="IPermissionsApiClient"/> instance.
        /// </summary>
        /// <param name="uriBuilder">The permission URI builder.</param>
        /// <returns>The created <see cref="IPermissionsApiClient"/>.</returns>
        IPermissionsApiClient Create(IPermissionsUriBuilder uriBuilder);

        /// <summary>
        /// Creates an <see cref="IPermissionsApiClient"/> instance.
        /// </summary>
        /// <param name="contentApiClient">The content API client to use to determine the URL prefix.</param>
        /// <returns>The created <see cref="IPermissionsApiClient"/>.</returns>
        IPermissionsApiClient Create(IContentApiClient contentApiClient);

        /// <summary>
        /// Creates an <see cref="IDefaultPermissionsApiClient"/> instance.
        /// </summary>
        IDefaultPermissionsApiClient CreateDefaultPermissionsClient();
    }
}