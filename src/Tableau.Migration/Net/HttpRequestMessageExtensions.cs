using System.Net.Http;

namespace Tableau.Migration.Net
{
    internal static class HttpRequestMessageExtensions
    {
        internal static string GetPolicyRequestKey(
            this HttpRequestMessage httpRequest)
        {
            return $"{httpRequest.RequestUri?.AbsolutePath}_{httpRequest.Method}";
        }
    }
}
