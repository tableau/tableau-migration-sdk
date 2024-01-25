using System.Net.Http;
using Tableau.Migration.Net;

namespace Tableau.Migration.Tests.Unit
{
    public static class IHttpClientExtensions
    {
        public static HttpClient GetInnerClient(this IHttpClient httpClient)
            => (httpClient.GetFieldValue("_innerHttpClient") as HttpClient)!;
    }
}
