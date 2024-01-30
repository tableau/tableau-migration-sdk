using System;
using Tableau.Migration.Api;

#region namespace

namespace Csharp.ExampleApplication.Config
{
    public class EndpointOptions
    {
        public Uri ServerUrl { get; set; } = TableauSiteConnectionConfiguration.Empty.ServerUrl;

        public string SiteContentUrl { get; set; } = string.Empty;

        public string AccessTokenName { get; set; } = string.Empty;

        // Access token configuration should use a secure configuration system.
        public string AccessToken { get; set; } = string.Empty;
    }
}

#endregion