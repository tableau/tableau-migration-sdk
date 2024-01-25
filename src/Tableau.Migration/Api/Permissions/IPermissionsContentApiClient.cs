namespace Tableau.Migration.Api.Permissions
{
    /// <summary>
    /// Interface for an API client 
    /// for a content type that has permissions operations.
    /// </summary>
    public interface IPermissionsContentApiClient
    {
        /// <summary>
        /// Gets the permissions API client.
        /// </summary>
        IPermissionsApiClient Permissions { get; }
    }
}
