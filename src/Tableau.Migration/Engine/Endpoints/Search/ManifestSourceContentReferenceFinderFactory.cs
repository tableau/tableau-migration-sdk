using System;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// <see cref="IContentReferenceFinderFactory"/> implementation that finds source references
    /// from the migration manifest.
    /// </summary>
    public class ManifestSourceContentReferenceFinderFactory : IContentReferenceFinderFactory
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Creates a new <see cref="ManifestSourceContentReferenceFinderFactory"/> object.
        /// </summary>
        /// <param name="services">The service provider.</param>
        public ManifestSourceContentReferenceFinderFactory(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc />
        public IContentReferenceFinder<TContent> ForContentType<TContent>()
            where TContent : IContentReference
            => _services.GetRequiredService<ManifestSourceContentReferenceFinder<TContent>>();
    }
}
