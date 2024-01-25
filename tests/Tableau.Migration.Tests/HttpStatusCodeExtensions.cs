using System.Net;

namespace Tableau.Migration.Tests
{
    public static class HttpStatusCodeExtensions
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
            => ((int)statusCode >= 200) && ((int)statusCode <= 299);
    }
}
