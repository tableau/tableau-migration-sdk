using System;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Default <see cref="IMigrationInput"/> and <see cref="IMigrationInputInitializer"/> implementation.
    /// </summary>
    public sealed class MigrationInput : IMigrationInput, IMigrationInputInitializer
    {
        private readonly ILogger<MigrationInput> _log;
        private readonly ISharedResourcesLocalizer _localizer;

        /// <summary>
        /// Creates a new <see cref="MigrationInput"/> object.
        /// </summary>
        /// <param name="log">A logger to log to.</param>
        /// <param name="localizer">A string localizer.</param>
        public MigrationInput(ILogger<MigrationInput> log, ISharedResourcesLocalizer localizer)
        {
            _log = log;
            _localizer = localizer;
            MigrationId = Guid.NewGuid();
        }

        /// <inheritdoc />
        public Guid MigrationId { get; }

        /// <inheritdoc />
        public IMigrationPlan Plan
        {
            get => _plan ?? throw new InvalidOperationException($"{nameof(MigrationInput)} must be initialized before it it used.");
            private set => _plan = value;
        }
        private IMigrationPlan? _plan;

        /// <inheritdoc />
        public IMigrationManifest? PreviousManifest { get; private set; }

        /// <inheritdoc />
        public void Initialize(IMigrationPlan plan, IMigrationManifest? previousManifest)
        {
            Plan = plan;
            PreviousManifest = previousManifest;

            if (PreviousManifest is not null && PreviousManifest.PlanId != Plan.PlanId)
            {
                _log.LogWarning(_localizer[SharedResourceKeys.PreviousManifestPlanMismatchWarning]);
            }
        }
    }
}
