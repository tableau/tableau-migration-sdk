using System;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest;

namespace Tableau.Migration.Tests.Unit.Api
{
    public abstract class ApiClientTestBase<TApiClient> : ApiTestBase
        where TApiClient : IContentApiClient
    {
        public string UrlPrefix { get; } = RestUrlPrefixes.GetUrlPrefix<TApiClient>();

        protected readonly Guid SiteId = Guid.NewGuid();
        protected readonly Guid UserId = Guid.NewGuid();
        protected string SiteContentUrl => SiteConnectionConfiguration.SiteContentUrl;

        protected TApiClient ApiClient => _apiClient ??= CreateClient();

        private TApiClient? _apiClient;

        public ApiClientTestBase()
        {
            MockSessionProvider.SetupGet(p => p.SiteContentUrl).Returns(() => SiteContentUrl);
            MockSessionProvider.SetupGet(p => p.SiteId).Returns(() => SiteId);
            MockSessionProvider.SetupGet(p => p.UserId).Returns(() => UserId);
        }

        protected TApiClient CreateClient() => Dependencies.CreateClient<TApiClient>();

        protected TApiClientImpl GetApiClient<TApiClientImpl>()
            where TApiClientImpl : TApiClient
            => (TApiClientImpl)ApiClient;
    }
}
