using System;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Default <see cref="IMigrationManifestFactory"/> implementation.
    /// </summary>
    public class MigrationManifestFactory : IMigrationManifestFactory
    {
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Creates a new <see cref="MigrationManifestFactory"/> object.
        /// </summary>
        /// <param name="localizer">A localizer.</param>
        /// <param name="loggerFactory">A logger factory.</param>
        public MigrationManifestFactory(ISharedResourcesLocalizer localizer, ILoggerFactory loggerFactory)
        {
            _localizer = localizer;
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public IMigrationManifestEditor Create(IMigrationInput input, Guid migrationId)
        {
            return new MigrationManifest(_localizer, _loggerFactory, input.Plan.PlanId, migrationId, input.PreviousManifest);
        }

        /// <inheritdoc />
        public IMigrationManifestEditor Create(Guid planId, Guid migrationId)
        {
            return new MigrationManifest(_localizer, _loggerFactory, planId, migrationId, null);
        }
    }
}
