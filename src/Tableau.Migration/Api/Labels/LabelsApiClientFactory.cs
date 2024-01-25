using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Labels
{
    internal sealed class LabelsApiClientFactory : ILabelsApiClientFactory
    {
        private readonly IRestRequestBuilderFactory _requestBuilderFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISharedResourcesLocalizer _localizer;

        public LabelsApiClientFactory(IRestRequestBuilderFactory requestBuilderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer localizer)
        {
            _requestBuilderFactory = requestBuilderFactory;
            _loggerFactory = loggerFactory;
            _localizer = localizer;
        }

        ///<inheritdoc/>
        public ILabelsApiClient<TContent> Create<TContent>()
             where TContent : IContentReference, IWithLabels
            => new LabelsApiClient<TContent>(_requestBuilderFactory, _loggerFactory, _localizer);
    }
}
