using Microsoft.Extensions.Logging;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Tags
{
    internal sealed class TagsApiClientFactory : ITagsApiClientFactory
    {
        private readonly IRestRequestBuilderFactory _requestBuilderFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly IHttpContentSerializer _serializer;

        public TagsApiClientFactory(IRestRequestBuilderFactory requestBuilderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer localizer,
            IHttpContentSerializer serializer)
        {
            _requestBuilderFactory = requestBuilderFactory;
            _loggerFactory = loggerFactory;
            _localizer = localizer;
            _serializer = serializer;
        }

        public ITagsApiClient Create(IContentApiClient contentApiClient)
            => new TagsApiClient(_requestBuilderFactory, _loggerFactory, _localizer, new(contentApiClient.UrlPrefix), _serializer);
    }
}
