using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net.Handlers
{
    /// <summary>
    /// Handler that will add the SDK user agent to all requests
    /// </summary>
    internal class UserAgentHttpMessageHandler : DelegatingHandler
    {
        private readonly string _userAgent;

        public UserAgentHttpMessageHandler(IMigrationSdk sdk) => _userAgent = sdk.UserAgent;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.UserAgent.Clear();
            request.Headers.UserAgent.TryParseAdd(_userAgent);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
