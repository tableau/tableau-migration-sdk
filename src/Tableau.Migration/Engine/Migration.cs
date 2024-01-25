using System;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Default <see cref="IMigration"/> implementation.
    /// </summary>
    public class Migration : IMigration
    {
        /// <summary>
        /// Creates a new <see cref="Migration"/> object.
        /// </summary>
        /// <param name="input">The migration input to initialize plan and previous manifest from.</param>
        /// <param name="pipelineFactory">An object to create pipelines with.</param>
        /// <param name="endpointFactory">An object to create endpoints with.</param>
        /// <param name="manifestFactory">An object to create manifests with.</param>
        public Migration(IMigrationInput input, IMigrationPipelineFactory pipelineFactory,
            IMigrationEndpointFactory endpointFactory, IMigrationManifestFactory manifestFactory)
        {
            Id = input.MigrationId;
            Plan = input.Plan;

            Pipeline = pipelineFactory.Create(Plan);

            Source = endpointFactory.CreateSource(Plan);
            Destination = endpointFactory.CreateDestination(Plan);

            Manifest = manifestFactory.Create(input, Id);
        }

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public ISourceEndpoint Source { get; }

        /// <inheritdoc />
        public IDestinationEndpoint Destination { get; }

        /// <inheritdoc />
        public IMigrationPlan Plan { get; }

        /// <inheritdoc />
        public IMigrationManifestEditor Manifest { get; }

        /// <inheritdoc />
        public IMigrationPipeline Pipeline { get; }
    }
}
