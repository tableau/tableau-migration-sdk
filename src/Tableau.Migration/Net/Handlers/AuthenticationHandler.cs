using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Net.Handlers
{
    internal class AuthenticationHandler : DelegatingHandler
    {
        private readonly IAuthenticationTokenProvider _tokenProvider;

        public AuthenticationHandler(IAuthenticationTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.IsRest() && !request.RequestUri.IsRestSignIn())
            {
                // Use the current token
                if (_tokenProvider.Token is not null)
                    request.SetRestAuthenticationToken(_tokenProvider.Token);

                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await _tokenProvider.RequestRefreshAsync(cancellationToken).ConfigureAwait(false);

                    // Use the new token
                    if (_tokenProvider.Token is not null)
                        request.SetRestAuthenticationToken(_tokenProvider.Token);
                }
                else
                {
                    return response;
                }
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
