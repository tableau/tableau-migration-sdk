using System;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class for an API client server info result model. 
    /// </summary>
    internal class ServerInfo : IServerInfo
    {
        /// <inheritdoc/>
        public string RestApiVersion { get; }

        /// <inheritdoc/>
        public string ProductVersion { get; }

        /// <inheritdoc/>
        public string BuildVersion { get; }

        public TableauServerVersion TableauServerVersion { get; }

        /// <summary>
        /// Creates a new <see cref="ServerInfo"/> instance.
        /// </summary>
        /// <param name="response">The REST API server info response.</param>
        public ServerInfo(ServerInfoResponse response)
        {
            Guard.AgainstNull(response.Item, () => response.Item);
            Guard.AgainstNull(response.Item.RestApiVersion, () => response.Item.RestApiVersion);
            Guard.AgainstNull(response.Item.ProductVersion, () => response.Item.ProductVersion);

            var restApiVersionValue = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.RestApiVersion.Version, () => response.Item.RestApiVersion.Version);
            var productVersionValue = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.ProductVersion.Version, () => response.Item.ProductVersion.Version);
            var buildVersionValue = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.ProductVersion.Build, () => response.Item.ProductVersion.Build);

            if (productVersionValue.Contains("near", StringComparison.OrdinalIgnoreCase))
                productVersionValue = productVersionValue.Replace("near", "9999.9", StringComparison.OrdinalIgnoreCase);

            if (buildVersionValue.Contains("near", StringComparison.OrdinalIgnoreCase))
                buildVersionValue = buildVersionValue.Replace("near", "99999", StringComparison.OrdinalIgnoreCase);

            RestApiVersion = restApiVersionValue;
            ProductVersion = productVersionValue;
            BuildVersion = buildVersionValue;

            TableauServerVersion = new(RestApiVersion, ProductVersion, BuildVersion);
        }
    }
}
