using Tableau.Migration.Api;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Endpoint configuration for connecting to a Tableau Server/Cloud site API.
    /// </summary>
    public interface ITableauApiEndpointConfiguration : IMigrationPlanEndpointConfiguration
    {
        /// <summary>
        /// Gets the configuration to use to sign into the Tableau site.
        /// </summary>
        TableauSiteConnectionConfiguration SiteConnectionConfiguration { get; }
    }
}
