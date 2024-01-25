using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// <see cref="IMigrationEndpoint"/> impelementation that uses Tableau Server/Cloud APIs.
    /// </summary>
    public abstract class TableauApiEndpointBase : IMigrationApiEndpoint
    {
        private IAsyncDisposableResult<ISitesApiClient>? _signInResult;

        /// <summary>
        /// The per-endpoint dependency injection scope.
        /// </summary>
        protected readonly AsyncServiceScope EndpointScope;

        /// <summary>
        /// The server-level API client.
        /// </summary>
        protected readonly IApiClient ServerApi;

        /// <summary>
        /// Gets the site-level API client.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the endpoint has not been initialized or site sign in failed.</exception>
        public ISitesApiClient SiteApi
        {
            get
            {
                if (_signInResult is null)
                {
                    throw new InvalidOperationException("API endpoint is not initialized.");
                }
                else if (_signInResult.Value is null)
                {
                    throw new InvalidOperationException("API endpoint does not have a valid site API client.");
                }

                return _signInResult.Value;
            }
        }

        /// <summary>
        /// Creates a new <see cref="TableauApiEndpointBase"/> object.
        /// </summary>
        /// <param name="serviceScopeFactory">A service scope factory to define an API client scope with.</param>
        /// <param name="config">The configuration options for connecting to the endpoint APIs.</param>
        /// <param name="finderFactory">The content finder factory to supply to the API client.</param>
        /// <param name="fileStore">The file store to use.</param>
        public TableauApiEndpointBase(IServiceScopeFactory serviceScopeFactory,
            ITableauApiEndpointConfiguration config,
            IContentReferenceFinderFactory finderFactory,
            IContentFileStore fileStore)
        {
            EndpointScope = serviceScopeFactory.CreateAsyncScope();

            var apiClientFactory = EndpointScope.ServiceProvider.GetRequiredService<IScopedApiClientFactory>();

            ServerApi = apiClientFactory.Initialize(config.SiteConnectionConfiguration, finderFactory, fileStore);
        }

        #region - IAsyncDisposable Implementation -

        /// <summary>
        /// Disposes the result's value.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_signInResult is not null)
                await _signInResult.DisposeAsync().ConfigureAwait(false);

            await EndpointScope.DisposeAsync().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region - IMigrationEndpoint Implementation -

        /// <inheritdoc />
        public async Task<IResult> InitializeAsync(CancellationToken cancel)
        {
            _signInResult = await ServerApi.SignInAsync(cancel).ConfigureAwait(false);
            return _signInResult;
        }

        /// <inheritdoc />
        public IPager<TContent> GetPager<TContent>(int pageSize)
        {
            var listApi = SiteApi.GetListApiClient<TContent>();
            return listApi.GetPager(pageSize);
        }

        #endregion
    }
}
