using System.Text.RegularExpressions;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API site methods.
    /// </summary>
    public sealed class SitesRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated site query by ID API method.
        /// </summary>
        public MethodSimulator QuerySiteById { get; }

        /// <summary>
        /// Gets the simulated site query by content URL API method.
        /// </summary>
        public MethodSimulator QuerySiteByContentUrl { get; }

        /// <summary>
        /// Creates a new <see cref="SitesRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public SitesRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            QuerySiteById = simulator.SetupRestGetById<SiteResponse, SiteResponse.SiteType>(RestApiUrl($"sites/{SiteId}"), d => d.Sites);
            QuerySiteByContentUrl = simulator.SetupRestGetByContentUrl<SiteResponse, SiteResponse.SiteType>(RestApiUrl($"sites/{ContentUrlPattern}"), d => d.Sites,
                queryStringPatterns: new[] { ("key", new Regex("contentUrl")) });
        }
    }
}
