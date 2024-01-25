using System;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Endpoints.Search;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Default <see cref="IMigrationEndpointFactory"/> implementation.
    /// </summary>
    public class MigrationEndpointFactory : IMigrationEndpointFactory
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ManifestDestinationContentReferenceFinderFactory _destinationFinderFactory;
        private readonly ManifestSourceContentReferenceFinderFactory _sourceFinderFactory;
        private readonly IContentFileStore _fileStore;

        /// <summary>
        /// Creates a new <see cref="MigrationEndpointFactory"/> object.
        /// </summary>
        /// <param name="serviceScopeFactory">A service scope factory to define an API client scope with.</param>
        /// <param name="sourceFinderFactory">A source content reference finder factory.</param>
        /// <param name="destinationFinderFactory">A destination content reference finder factory.</param>
        /// <param name="fileStore">The file store to use.</param>
        public MigrationEndpointFactory(IServiceScopeFactory serviceScopeFactory,
            ManifestSourceContentReferenceFinderFactory sourceFinderFactory,
            ManifestDestinationContentReferenceFinderFactory destinationFinderFactory,
            IContentFileStore fileStore)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _destinationFinderFactory = destinationFinderFactory;
            _sourceFinderFactory = sourceFinderFactory;
            _fileStore = fileStore;
        }

        /// <inheritdoc />
        public IDestinationEndpoint CreateDestination(IMigrationPlan plan)
        {
            if (plan.Destination is ITableauApiEndpointConfiguration apiConfig)
            {
                return new TableauApiDestinationEndpoint(_serviceScopeFactory, apiConfig, _destinationFinderFactory, _fileStore);
            }

            throw new ArgumentException($"Cannot create a destination endpoint for type {plan.Source.GetType()}");
        }

        /// <inheritdoc />
        public ISourceEndpoint CreateSource(IMigrationPlan plan)
        {
            if (plan.Source is ITableauApiEndpointConfiguration apiConfig)
            {
                return new TableauApiSourceEndpoint(_serviceScopeFactory, apiConfig, _sourceFinderFactory, _fileStore);
            }

            throw new ArgumentException($"Cannot create a source endpoint for type {plan.Source.GetType()}");
        }
    }
}
