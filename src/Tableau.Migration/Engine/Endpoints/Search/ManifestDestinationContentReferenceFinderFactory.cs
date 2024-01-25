using System;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// <see cref="IContentReferenceFinderFactory"/> implementation that finds destination references
    /// from the migration manifest.
    /// </summary>
    public class ManifestDestinationContentReferenceFinderFactory : IContentReferenceFinderFactory
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Creates a new <see cref="ManifestDestinationContentReferenceFinderFactory"/> object.
        /// </summary>
        /// <param name="services">A service provider.</param>
        public ManifestDestinationContentReferenceFinderFactory(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc />
        public IContentReferenceFinder<TContent> ForContentType<TContent>()
            where TContent : IContentReference
            => _services.GetRequiredService<ManifestDestinationContentReferenceFinder<TContent>>();
    }
}
