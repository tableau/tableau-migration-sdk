using System;
using Tableau.Migration.Api;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for an object that represents a location to move Tableau data to or from.
    /// </summary>
    public interface IMigrationApiEndpoint : IMigrationEndpoint
    {
        /// <summary>
        /// Gets the site-level API client.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the endpoint has not been initialized or site sign in failed.</exception>
        ISitesApiClient SiteApi { get; }
    }
}
