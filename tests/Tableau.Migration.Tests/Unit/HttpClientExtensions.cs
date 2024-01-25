using System.Net.Http;

namespace Tableau.Migration.Tests.Unit
{
    public static class HttpClientExtensions
    {
        public static HttpMessageHandler GetMessageHandler(this HttpClient httpClient)
            => (httpClient.GetFieldValue(typeof(HttpMessageInvoker), "_handler") as HttpMessageHandler)!;
    }
}
