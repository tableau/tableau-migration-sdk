using System.Net.Http;

namespace Tableau.Migration.Net.Rest
{
    internal static class HttpRequestMessageExtensions
    {
        public static void SetRestAuthenticationToken(this HttpRequestMessage request, string? token)
        {
            request.Headers.Remove(RestHeaders.AuthenticationToken);

            if (token is not null)
            {
                request.Headers.TryAddWithoutValidation(RestHeaders.AuthenticationToken, token);
            }
        }
    }
}
