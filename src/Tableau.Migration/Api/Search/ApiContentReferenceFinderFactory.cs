using System;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Api.Search
{
    /// <summary>
    /// <see cref="IContentReferenceFinderFactory"/> implementation that makes API calls, 
    /// intended for use when the API client is used without a migration engine.
    /// </summary>
    public class ApiContentReferenceFinderFactory : IContentReferenceFinderFactory
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Creates a new <see cref="ApiContentReferenceFinderFactory"/> object.
        /// </summary>
        /// <param name="services">A service provider.</param>
        public ApiContentReferenceFinderFactory(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc />
        public IContentReferenceFinder<TContent> ForContentType<TContent>()
            where TContent : IContentReference
        {
            var cache = _services.GetRequiredService<BulkApiContentReferenceCache<TContent>>();

            return new CachedContentReferenceFinder<TContent>(cache);
        }
    }
}
