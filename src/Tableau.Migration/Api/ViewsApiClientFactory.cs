using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class ViewsApiClientFactory : IViewsApiClientFactory
    {
        private readonly IRestRequestBuilderFactory _restRequestBuilderFactory;
        private readonly IPermissionsApiClientFactory _permissionsClientFactory;
        private readonly IContentReferenceFinderFactory _finderFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISharedResourcesLocalizer _sharedResourcesLocalizer;
        private readonly ITagsApiClientFactory _tagsClientFactory;

        public ViewsApiClientFactory(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IPermissionsApiClientFactory permissionsClientFactory,
            IContentReferenceFinderFactory finderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            ITagsApiClientFactory tagsClientFactory)
        {
            _restRequestBuilderFactory = restRequestBuilderFactory;
            _permissionsClientFactory = permissionsClientFactory;
            _finderFactory = finderFactory;
            _loggerFactory = loggerFactory;
            _sharedResourcesLocalizer = sharedResourcesLocalizer;
            _tagsClientFactory = tagsClientFactory;
        }

        public IViewsApiClient Create()
            => new ViewsApiClient(_restRequestBuilderFactory, _permissionsClientFactory, _finderFactory, _loggerFactory,
                _sharedResourcesLocalizer, _tagsClientFactory);
    }
}