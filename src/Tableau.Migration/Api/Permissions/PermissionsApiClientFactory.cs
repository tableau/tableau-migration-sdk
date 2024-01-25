using Tableau.Migration.Api.Rest;
using Tableau.Migration.Config;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Permissions
{
    internal sealed class PermissionsApiClientFactory : IPermissionsApiClientFactory
    {
        private readonly IRestRequestBuilderFactory _restRequestBuilderFactory;
        private readonly IHttpContentSerializer _serializer;
        private readonly ISharedResourcesLocalizer _sharedResourcesLocalizer;
        private readonly IConfigReader _configReader;

        public PermissionsApiClientFactory(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IHttpContentSerializer serializer,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IConfigReader configReader)
        {
            _restRequestBuilderFactory = restRequestBuilderFactory;
            _serializer = serializer;
            _sharedResourcesLocalizer = sharedResourcesLocalizer;
            _configReader = configReader;
        }

        /// <inheritdoc />
        public IPermissionsApiClient Create(IContentApiClient contentApiClient)
            => Create(new PermissionsUriBuilder(contentApiClient.UrlPrefix));

        /// <inheritdoc />
        public IPermissionsApiClient Create(IPermissionsUriBuilder uriBuilder)
            => new PermissionsApiClient(_restRequestBuilderFactory, _serializer, uriBuilder, _sharedResourcesLocalizer);

        /// <inheritdoc />
        public IDefaultPermissionsApiClient CreateDefaultPermissionsClient()
            => new DefaultPermissionsApiClient(this, _configReader.Get().DefaultPermissionsContentTypes);
    }
}
