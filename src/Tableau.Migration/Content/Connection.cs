using System;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    /// <inheritdoc/>
    internal class Connection : IConnection
    {
        public Connection(IConnectionType response)
        {
            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);
            Type = Guard.AgainstNullOrEmpty(response.Type, () => response.Type);

            ServerAddress = response.ServerAddress;
            ServerPort = response.ServerPort;
            ConnectionUsername = response.ConnectionUsername;

            if (response.QueryTaggingEnabled is not null &&
                bool.TryParse(response.QueryTaggingEnabled, out var queryTaggingEnabled))
            {
                QueryTaggingEnabled = queryTaggingEnabled;
            }
        }

        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <inheritdoc/>
        public string Type { get; set; }

        /// <inheritdoc/>
        public string? ServerAddress { get; set; }

        /// <inheritdoc/>
        public string? ServerPort { get; set; }

        /// <inheritdoc/>
        public string? ConnectionUsername { get; set; }

        /// <inheritdoc/>
        public bool? QueryTaggingEnabled { get; set; }
    }
}
