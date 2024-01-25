using System.Linq;
using System.Net.Http;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal static class IRestApiResponseBuilderExtensions
    {
        public static bool IsUnauthorizedRequest(this IRestApiResponseBuilder builder, HttpRequestMessage request, TableauData data)
        {
            if (!builder.RequiresAuthentication)
                return false;

            if (data.SignIn?.Token is null)
                return false;

            request.Headers.TryGetValues(RestHeaders.AuthenticationToken, out var tokenValues);

            if (tokenValues.IsNullOrEmpty() || tokenValues.First() != data.SignIn.Token)
                return true;

            return false;
        }
    }
}
