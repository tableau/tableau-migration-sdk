using System;
using Tableau.Migration.Api;

namespace Tableau.Migration.TestApplication.Config
{
    public class EndpointOptions
    {
        public Uri ServerUrl { get; set; } = TableauSiteConnectionConfiguration.Empty.ServerUrl;

        public string SiteContentUrl { get; set; } = string.Empty;

        public string AccessTokenName { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;
    }
}
