using System;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api
{
    internal sealed class ScopedApiClientFactory : IScopedApiClientFactory
    {
        private readonly IServiceProvider _services;

        public ScopedApiClientFactory(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc />
        public IApiClient Initialize(TableauSiteConnectionConfiguration scopedSiteConnection,
            IContentReferenceFinderFactory? finderFactoryOverride = null,
            IContentFileStore? fileStoreOverride = null)
        {
            var apiClientInput = _services.GetRequiredService<IApiClientInputInitializer>();
            apiClientInput.Initialize(scopedSiteConnection, finderFactoryOverride, fileStoreOverride);

            var requestFactoryInput = _services.GetRequiredService<IRequestBuilderFactoryInputInitializer>();
            requestFactoryInput.Initialize(scopedSiteConnection.ServerUrl);

            return _services.GetRequiredService<IApiClient>();
        }
    }
}
