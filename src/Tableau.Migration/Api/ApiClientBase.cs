using Microsoft.Extensions.Logging;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal abstract class ApiClientBase
    {
        protected readonly IRestRequestBuilderFactory RestRequestBuilderFactory;
        protected readonly ILogger Logger;
        protected readonly ISharedResourcesLocalizer SharedResourcesLocalizer;

        public ApiClientBase(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
        {
            RestRequestBuilderFactory = restRequestBuilderFactory;
            Logger = loggerFactory.CreateLogger(GetType());
            SharedResourcesLocalizer = sharedResourcesLocalizer;
        }
    }
}
